using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reversi.Engine.Core;
using Reversi.Engine.Interfaces;
using Reversi.Engine.Tests.Extensions;
using Moq;
using Xunit;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;

namespace Reversi.Engine.Tests.Core
{
    public class GameEngineTests
    {
        private IGameEngine _engine;
        
        public GameEngineTests()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Inject<IGameContext>(new GameContext());
            fixture.Inject<IGameOptions>(new GameOptions());
            var mockLocationHelper = fixture.Freeze<Mock<ILocationHelper>>();

            mockLocationHelper.Setup(lh => lh.GetLocationsGroups(It.IsAny<int>()))
                .Returns(new[] { new int[] { 0, 1 } });

            var strategyInfo = fixture.Create<StrategyInfo>();
            var mockStrategy = fixture.Freeze<Mock<IMoveStrategy>>();            
            mockStrategy.Setup(s => s.StrategyInfo).Returns(strategyInfo);

            var mockStrategyProvider = fixture.Freeze<Mock<IStrategyProvider>>();
            mockStrategyProvider.Setup(sp => sp.GetStrategy(It.IsAny<string>()))
                .Returns(mockStrategy.Object);
            mockStrategyProvider.Setup(sp => sp.GetStrategyInfoCollection())
                .Returns(new[] { strategyInfo });

            _engine = fixture.Create<GameEngine>();
        }

        [Fact]
        public void ShouldCreateResponseWithNewBoardLayoutWhenNewGameCreated()
        {
            //Arrange - see constructor

            //Act
            var response = _engine.CreateNewGame();

            //Assert
            Assert.Equal(4, response.Squares.Count(s => s.Piece != Piece.None));
            Assert.True(response.Squares.ElementAt(27).Piece == Piece.White);
            Assert.True(response.Squares.ElementAt(28).Piece == Piece.Black);
            Assert.True(response.Squares.ElementAt(35).Piece == Piece.Black);
            Assert.True(response.Squares.ElementAt(36).Piece == Piece.White);
        }

        [Theory]
        [InlineData(true, 2, 1)]
        [InlineData(false, 1, 0)]
        public void ShouldExpectEngineToMakeFirstMoveWhenUserPlaysAsWhite(
            bool userPlaysWhite, int expectedNumMoves, int expectedNumMoveChoices)
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Inject<IGameContext>(new GameContext());
            fixture.Inject<IGameOptions>(new GameOptions() { UserPlaysAsBlack = !userPlaysWhite });

            var mockMoveFinder = fixture.Freeze<Mock<IValidMoveFinder>>();
            mockMoveFinder.Setup(mf => mf.FindAllValidMoves(It.IsAny<IGameContext>()))
                .Returns(new[] { 0 });

            var mockMoveStrategy = SetUpStrategy(fixture, fixture.Create<StrategyInfo>());
            mockMoveStrategy.Setup(ms => ms.ChooseMove(It.IsAny<IGameContext>(), It.IsAny<IGameEngine>()))
                .Returns(Move.PassMove);

            var mockStatusExaminer = fixture.Freeze<Mock<IGameStatusExaminer>>();
            mockStatusExaminer.Setup(se => se.DetermineGameStatus(It.IsAny<IGameContext>()))
                .Returns(GameStatus.InProgress);

            var engine = fixture.Create<GameEngine>();
            
            //Act
            var response = engine.CreateNewGame();

            //Assert
            Assert.Equal(expectedNumMoves, engine.MoveNumber);
            mockMoveStrategy.Verify(ms => ms.ChooseMove(It.IsAny<IGameContext>(), engine), 
                Times.Exactly(expectedNumMoveChoices));

        }

        [Fact]
        public async void ShouldSetAllSquaresAsInvalidAfterUpdatingBoardWithMove()
        {
            //Arrange
            _engine.CreateNewGame();
            Move move = new Move(19);

            //Act
            var response = await _engine.UpdateBoardWithMoveAsync(move);

            //Assert
            Assert.True(response.Squares.All(s => !s.IsValidMove));
        }

        [Fact]
        public async void ShouldHavePlayersPieceInSquareAfterUpdatingBoardWithMove()
        {
            //Arrange
            _engine.CreateNewGame();
            Move move = new Move(19);

            //Act
            var response = await _engine.UpdateBoardWithMoveAsync(move);

            //Assert
            Assert.Equal(Piece.Black, response.Squares[19].Piece);
        }

        [Fact]
        public async void ShouldHaveEnginesPieceInSquareAfterUpdatingBoardWithReply()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var context = fixture.Create<GameContext>();
            context.SetPiece(Piece.Black, 1).SetPiece(Piece.White, 2);
            context.SetMovePlayed(); // indicate that it's the engine's turn to play
            fixture.Inject<IGameContext>(context);

            //engine should play in cell 0:

            var mockMoveStrategy = SetUpStrategy(fixture, fixture.Create<StrategyInfo>());
            mockMoveStrategy.Setup(ms => ms.ChooseMove(It.IsAny<IGameContext>(), It.IsAny<IGameEngine>()))
                .Returns(new Move(0));

            //engine should capture cell 1
            var mockCaptureHelper = fixture.Freeze<Mock<ICaptureHelper>>();
            mockCaptureHelper.Setup(ch => ch.CaptureEnemyPieces(It.IsAny<IGameContext>(), 0))
                .Callback(() => context.SetPiece(1, Piece.White));

            var engine = fixture.Create<GameEngine>();

            //Act
            var response = await engine.MakeReplyMoveAsync();

            //Assert
            Assert.Equal(Piece.White, response.Squares[0].Piece);
            Assert.Equal(Piece.White, response.Squares[1].Piece);
            Assert.Equal(Piece.White, response.Squares[2].Piece);
        }

        [Fact]
        public async void ShouldUpdateMoveNumberAfterUpdatingBoardWithOpponentsMove()
        {
            //Arrange
            _engine.CreateNewGame();
            Move move = new Move(19);

            //Act
            var response = await _engine.UpdateBoardWithMoveAsync(move);

            //Assert
            Assert.Equal(2, _engine.MoveNumber);
        }

        [Fact]
        public async void ShouldUpdateMoveNumberAfterReplyingWithEnginesMove()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Inject<IGameContext>(new GameContext());

            var mockMoveStrategy = SetUpStrategy(fixture, fixture.Create<StrategyInfo>());
            mockMoveStrategy.Setup(ms => ms.ChooseMove(It.IsAny<IGameContext>(),
                It.IsAny<IGameEngine>()))
                .Returns(Move.PassMove);

            var engine = fixture.Create<GameEngine>();

            Move move = new Move(19); // represents user's move
            var response = await engine.UpdateBoardWithMoveAsync(move); // opponent's move

            //Act
            await engine.MakeReplyMoveAsync(); // engine's reply

            //Assert
            Assert.Equal(3, engine.MoveNumber);
        }

        [Theory]
        [InlineData(GameStatus.InProgress)]
        [InlineData(GameStatus.BlackWins)]
        [InlineData(GameStatus.WhiteWins)]
        [InlineData(GameStatus.Draw)]
        public async void ShouldReportCorrectStatusInReponse(GameStatus gameStatus)
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var mockStatusExaminer = fixture.Freeze<Mock<IGameStatusExaminer>>();
            mockStatusExaminer.Setup(se => se.DetermineGameStatus(It.IsAny<IGameContext>()))
                .Returns(gameStatus);

            var mockMoveStrategy = SetUpStrategy(fixture, fixture.Create<StrategyInfo>());
            mockMoveStrategy.Setup(ms => ms.ChooseMove(It.IsAny<IGameContext>(), It.IsAny<IGameEngine>()))
                .Returns(fixture.Create<Move>());

            var engine = fixture.Create<GameEngine>();
            engine.CreateNewGame();
            Move move = new Move(19);

            //Act
            var response = await engine.UpdateBoardWithMoveAsync(move);

            //Assert
            Assert.Equal(gameStatus, response.Status);
        }
        
        public async void ShouldAutoSkipOpponentsMoveIfNoSuchMoveIsAvailable()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var context = new GameContext();
            context.SetMovePlayed();//update context to be white's turn:
            fixture.Inject<IGameContext>(context);

            // black articifially has no valid moves:
            var mockValidMoveFinder = fixture.Freeze<Mock<IValidMoveFinder>>();
            mockValidMoveFinder.Setup(vmf => vmf.FindAllValidMoves(It.IsAny<IGameContext>()))
                .Returns(new int[0]);

            // pre-determine whites moves, capturing 1 black piece each time:
            Func<IGameContext, Move> getWhiteMove = (c) =>
            {
                switch (c.MoveNumber)
                {
                    case 2: return new Move(26);
                    case 4: return new Move(37);
                    default: return Move.PassMove;
                }
            };

            var mockMoveStrategy = fixture.Freeze<Mock<IMoveStrategy>>();
            mockMoveStrategy.Setup(mc => mc.ChooseMove(It.IsAny<IGameContext>(),
                It.IsAny<IGameEngine>()))
                .Returns(getWhiteMove(context));

            var mockStatusExaminer = fixture.Freeze<Mock<IGameStatusExaminer>>();
            mockStatusExaminer.Setup(se => se.DetermineGameStatus(It.IsAny<IGameContext>()))
                .Returns(context.MoveNumber == 6 ? GameStatus.WhiteWins : GameStatus.InProgress);

            //set up engine
            var engine = fixture.Create<GameEngine>();

            //Act            
            await engine.MakeReplyMoveAsync(); // engine's reply

            //Assert - should have been called twice
            mockMoveStrategy.Verify(ms => 
                ms.ChooseMove(It.IsAny<IGameContext>(),
                    It.IsAny<IGameEngine>()), 
                Times.Exactly(2));

            mockValidMoveFinder.Verify(vmf =>
                vmf.FindAllValidMoves(It.IsAny<IGameContext>()), 
                Times.Exactly(2));
        }

        [Fact]
        public void ShouldUpdateContextSuppliedWhenUpdatingBoardWithMove()
        {
            //Arrange
            var externalContext = new GameContext();
            //move1:
            externalContext.SetPiece(0, Piece.Black);
            externalContext.SetMovePlayed();
            //move2:
            externalContext.SetPiece(1, Piece.White);
            externalContext.SetMovePlayed();
            //move3 - to be played in the call to the engine:
            var move = new Move(2);

            //Act
            _engine.UpdateBoardWithMove(move, externalContext);

            //Assert 
            // external context should now be on move 4
            Assert.Equal(4, externalContext.MoveNumber);
            Assert.Equal(Piece.Black, externalContext[0].Piece);
            // engine's internal context should be unchanged from start of game
            Assert.Equal(1, _engine.Context.MoveNumber);
            Assert.Equal(Piece.None, _engine.Context[0].Piece);
        }

        [Fact]
        public void ShouldUpdateContextSuppliedWhenReplyingWithEnginesMove()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var externalContext = new GameContext();
            //move1:
            externalContext.SetPiece(0, Piece.Black);
            externalContext.SetMovePlayed();

            fixture.Inject<IGameContext>(new GameContext());
            var mockMoveStrategy = SetUpStrategy(fixture, fixture.Create<StrategyInfo>());
            mockMoveStrategy.Setup(ms => ms.ChooseMove(It.IsAny<IGameContext>(), It.IsAny<IGameEngine>()))
                .Returns(new Move(1));
            

            var engine = fixture.Create<GameEngine>();

            //Act
            engine.MakeReplyMove(externalContext); // this will be move 2

            //Assert 
            // external context should now be on move 3
            Assert.Equal(3, externalContext.MoveNumber);
            Assert.Equal(Piece.Black, externalContext[0].Piece);
            // engine's internal context should be unchanged from start of game
            Assert.Equal(1, engine.Context.MoveNumber);
            Assert.Equal(Piece.None, engine.Context[0].Piece);
        }

        [Fact]
        public void ShouldReturnAvailableStrategiesIncludingStrategySupplied()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            fixture.Inject<IGameContext>(new GameContext());
            var strategyInfo = fixture.Create<StrategyInfo>();
            var mockMoveStrategy = SetUpStrategy(fixture, strategyInfo);

            //Act
            var engine = fixture.Create<GameEngine>();
            var allAvailableStrategies = engine.AvailableStrategies;

            //Assert 
            Assert.Equal(strategyInfo.Name, allAvailableStrategies.First().Name);
            Assert.Equal(1, allAvailableStrategies.Count());
        }

        [Fact]
        public void ShouldApplyNewStrategyLevelWhenOptionsAreChanged()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            fixture.Inject<IGameContext>(new GameContext());
            var strategyInfo = fixture.Create<StrategyInfo>();
            Mock<IMoveStrategy> mockMoveStrategy = SetUpStrategy(fixture, strategyInfo);

            var engine = fixture.Create<GameEngine>();

            //Act            
            engine.GameOptions = new GameOptions() { StrategyLevel = 99 };

            //Assert 
            mockMoveStrategy.Verify(ms => ms.SetLevel(99), Times.Once);
        }



        [Fact]
        public void ShouldReturnMultipleStrategyInfosWhenMultipleStrategiesAvailable()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            fixture.Inject<IGameContext>(new GameContext());
            var altStrategy = fixture.Create<Mock<IMoveStrategy>>();
            var altStrategyInfo = fixture.Create<StrategyInfo>();
            altStrategy.Setup(s => s.StrategyInfo).Returns(altStrategyInfo);

            var mockMoveStrategy = fixture.Freeze<Mock<IMoveStrategy>>();
            var strategyInfo = fixture.Create<StrategyInfo>();
            mockMoveStrategy.Setup(s => s.StrategyInfo).Returns(strategyInfo);

            var mockStrategyProvider = SetupStrategyProvider(fixture, strategyInfo, mockMoveStrategy);
            mockStrategyProvider.Setup(sp => sp.GetStrategyInfoCollection())
                .Returns(new[] { strategyInfo, altStrategyInfo });

            var engine = fixture.Create<GameEngine>();

            //Act            
            var strategyInfos = engine.AvailableStrategies;

            //Assert 
            Assert.True(strategyInfos.Count() == 2);
            Assert.Contains(strategyInfo, strategyInfos);
            Assert.Contains(altStrategyInfo, strategyInfos);
        }

        [Fact]
        public void ShouldApplyNewStrategyChoiceWhenOptionsAreChanged()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            fixture.Inject<IGameContext>(new GameContext());
            var altStrategy = fixture.Create<Mock<IMoveStrategy>>();
            var altStrategyInfo = new StrategyInfo(fixture.Create<string>(), true, 2);
            altStrategy.Setup(s => s.StrategyInfo).Returns(altStrategyInfo);            

            var mockMoveStrategy = fixture.Freeze<Mock<IMoveStrategy>>();
            var strategyInfo = fixture.Create<StrategyInfo>();
            mockMoveStrategy.Setup(s => s.StrategyInfo).Returns(strategyInfo);
            
            var mockStrategyProvider = fixture.Freeze<Mock<IStrategyProvider>>();
            mockStrategyProvider.Setup(sp => sp.GetStrategy(strategyInfo.Name))
                .Returns(mockMoveStrategy.Object);
            mockStrategyProvider.Setup(sp => sp.GetStrategy(altStrategyInfo.Name))
                .Returns(altStrategy.Object);
            mockStrategyProvider.Setup(sp => sp.GetStrategyInfoCollection())
                .Returns(new[] { strategyInfo, altStrategyInfo});

            var mockOptions = fixture.Freeze<Mock<IGameOptions>>();
            mockOptions.Setup(o => o.StrategyName).Returns(strategyInfo.Name);
            
            var engine = fixture.Build<GameEngine>() //don't override options..
                                .Without(e => e.GameOptions) // ..with dummy values
                                .Create();

            var options = new GameOptions()
            {
                StrategyName = altStrategyInfo.Name,
                StrategyLevel = 5
            };

            //Act            
            engine.GameOptions = options;

            //Assert 
            mockStrategyProvider.Verify(sp => sp.GetStrategy(options.StrategyName), Times.Once);
            altStrategy.Verify(ms => ms.SetLevel(5), Times.Once);
            mockMoveStrategy.Verify(ms => ms.SetLevel(5), Times.Never);
        }

        private static Mock<IMoveStrategy> SetUpStrategy(IFixture fixture, StrategyInfo strategyInfo)
        {
            var mockMoveStrategy = fixture.Freeze<Mock<IMoveStrategy>>();
            mockMoveStrategy.Setup(s => s.StrategyInfo).Returns(strategyInfo);
            SetupStrategyProvider(fixture, strategyInfo, mockMoveStrategy);
            return mockMoveStrategy;
        }

        private static Mock<IStrategyProvider> SetupStrategyProvider(IFixture fixture, StrategyInfo strategyInfo, Mock<IMoveStrategy> mockMoveStrategy)
        {
            var mockStrategyProvider = fixture.Freeze<Mock<IStrategyProvider>>();
            mockStrategyProvider.Setup(sp => sp.GetStrategy(It.IsAny<string>()))
                .Returns(mockMoveStrategy.Object);
            mockStrategyProvider.Setup(sp => sp.GetStrategyInfoCollection())
                .Returns(new[] { strategyInfo });
            return mockStrategyProvider;
        }
    }


}


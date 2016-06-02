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
        private Mock<IMovePlayer> _mockMovePlayer;

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

            _mockMovePlayer = fixture.Freeze<Mock<IMovePlayer>>();
            
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
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void ShouldExpectEngineToMakeFirstMoveWhenUserPlaysAsWhite(
            bool userPlaysWhite, int expectedNumMoveChoices)
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Inject<IGameContext>(new GameContext());
            fixture.Inject<IGameOptions>(new GameOptions() { UserPlaysAsBlack = !userPlaysWhite });

            var mockMoveFinder = fixture.Freeze<Mock<IValidMoveFinder>>();
            mockMoveFinder.Setup(mf => mf.FindAllValidMoves(It.IsAny<IGameContext>()))
                .Returns(new[] { 0 });

            var mockMovePlayer = fixture.Freeze<Mock<IMovePlayer>>();
            mockMovePlayer.Setup(mp => mp.PlayMove(It.IsAny<Move>(), It.IsAny<IGameContext>()))
                .Returns(new MoveResult(GameStatus.Draw, new GameContext()));

            var mockMoveStrategy = SetUpStrategy(fixture, fixture.Create<StrategyInfo>());
            mockMoveStrategy.Setup(ms => ms.ChooseMove(It.IsAny<IGameContext>(), It.IsAny<IMovePlayer>()))
                .Returns(Move.PassMove);

            var engine = fixture.Create<GameEngine>();
            
            //Act
            var response = engine.CreateNewGame();

            //Assert
            mockMoveStrategy.Verify(ms => ms.ChooseMove(It.IsAny<IGameContext>(), It.IsAny<IMovePlayer>()), 
                Times.Exactly(expectedNumMoveChoices));
        }

        [Fact]
        public async void ShouldProvideSquaresFromResultInMoveReponse()
        {
            //Arrange
            var contextToReturn = new GameContext();
            contextToReturn.SetPiece(19, Piece.Black);
            _mockMovePlayer.Setup(mp => mp.PlayMove(It.IsAny<Move>(), It.IsAny<IGameContext>()))
                .Returns(new MoveResult(GameStatus.InProgress, contextToReturn));
            _engine.CreateNewGame();
            Move move = new Move(19);

            //Act
            var response = await _engine.UpdateBoardWithMoveAsync(move);

            //Assert
            Assert.Equal(Piece.Black, response.Squares[19].Piece);
        }

        [Fact]
        public async void ShouldProvideSquaresFromResultInReplyReponse()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var contextToReturn = new GameContext();
            contextToReturn.SetPiece(19, Piece.Black);
            var mockMovePlayer = fixture.Freeze<Mock<IMovePlayer>>();
            mockMovePlayer.Setup(mp => mp.PlayMove(It.IsAny<Move>(), It.IsAny<IGameContext>()))
                .Returns(new MoveResult(GameStatus.InProgress, contextToReturn));

            var mockMoveStrategy = SetUpStrategy(fixture, fixture.Create<StrategyInfo>());
            mockMoveStrategy.Setup(ms => ms.ChooseMove(It.IsAny<IGameContext>(), It.IsAny<IMovePlayer>()))
                .Returns(new Move(0));

            var engine = fixture.Create<GameEngine>();

            //Act
            var response = await engine.MakeReplyMoveAsync();

            //Assert
            Assert.Equal(Piece.Black, response.Squares[19].Piece);
        }

        [Fact]
        public async void ShouldProvideStatusFromResultInMoveResponse()
        {
            //Arrange
            _mockMovePlayer.Setup(mp => mp.PlayMove(It.IsAny<Move>(), It.IsAny<IGameContext>()))
                .Returns(new MoveResult(GameStatus.Draw, new GameContext()));
            _engine.CreateNewGame();
            Move move = new Move(19);

            //Act
            var response = await _engine.UpdateBoardWithMoveAsync(move);

            //Assert
            Assert.Equal(GameStatus.Draw, response.Status);
        }

        [Fact]
        public async void ShouldProvideStatusFromResultInReplyReponse()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var mockMovePlayer = fixture.Freeze<Mock<IMovePlayer>>();
            mockMovePlayer.Setup(mp => mp.PlayMove(It.IsAny<Move>(), It.IsAny<IGameContext>()))
                .Returns(new MoveResult(GameStatus.Draw, new GameContext()));

            var mockMoveStrategy = SetUpStrategy(fixture, fixture.Create<StrategyInfo>());
            mockMoveStrategy.Setup(ms => ms.ChooseMove(It.IsAny<IGameContext>(), It.IsAny<IMovePlayer>()))
                .Returns(new Move(0));

            var engine = fixture.Create<GameEngine>();

            //Act
            var response = await engine.MakeReplyMoveAsync();

            //Assert
            Assert.Equal(GameStatus.Draw, response.Status);
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
                It.IsAny<IMovePlayer>()))
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
                ms.ChooseMove(It.IsAny<IGameContext>(), It.IsAny<IMovePlayer>()), 
                Times.Exactly(2));

            mockValidMoveFinder.Verify(vmf =>
                vmf.FindAllValidMoves(It.IsAny<IGameContext>()), 
                Times.Exactly(2));
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


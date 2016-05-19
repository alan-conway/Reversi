using Moq;
using Reversi.Engine.Tests.Builders;
using Reversi.Engine.Tests.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reversi.Engine.Interfaces;
using Xunit;
using Reversi.Engine.Core;

namespace Reversi.Engine.Tests.Core
{
    public class GameEngineTests
    {
        private IGameEngine _engine;
        private GameEngineBuilder _builder;

        public GameEngineTests()
        {
            _builder = new GameEngineBuilder();

            var context = new GameContext();
            _engine = _builder.SetContext(context).Build();
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

        [Fact]
        public async void ShouldSetAllSquaresAsInvalidAfterUpdatingBoardWithMove()
        {
            //Arrange
            _engine.CreateNewGame();
            Move move = new Move(34);

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
            Move move = new Move(34);

            //Act
            var response = await _engine.UpdateBoardWithMoveAsync(move);

            //Assert
            Assert.Equal(Piece.Black, response.Squares[34].Piece);
        }

        [Fact]
        public async void ShouldHaveEnginesPieceInSquareAfterUpdatingBoardWithReply()
        {
            //Arrange
            var context = GameContextFactory.CreateGameContext(new[] { 1 }, new[] { 2 });
            context.SetMovePlayed(); // indicate that it's the engine's turn to play

            //engine should play in cell 0:
            var mockMoveStrategy = new Mock<IMoveStrategy>();
            mockMoveStrategy.Setup(ms => ms.ChooseMove(It.IsAny<IGameContext>(), 
                It.IsAny<IGameEngine>()))
                .Returns(new Move(0));
            
            //engine should capture cell 1
            var mockCaptureHelper = new Mock<ICaptureHelper>();
            mockCaptureHelper.Setup(ch => ch.CaptureEnemyPieces(It.IsAny<IGameContext>(), 0))
                .Callback(() => context.SetPiece(1, Piece.White));

            var engine = _builder
                .SetContext(context)
                .SetCaptureHelper(mockCaptureHelper.Object)
                .SetMoveStrategy(mockMoveStrategy.Object)
                .Build();

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
            Move move = new Move(34);

            //Act
            var response = await _engine.UpdateBoardWithMoveAsync(move);

            //Assert
            Assert.Equal(2, _engine.MoveNumber);
        }

        [Fact]
        public async void ShouldUpdateMoveNumberAfterReplyingWithEnginesMove()
        {
            //Arrange
            var mockMoveStrategy = new Mock<IMoveStrategy>(); //finds engine's replying move
            mockMoveStrategy.Setup(ms => ms.ChooseMove(It.IsAny<IGameContext>(),
                It.IsAny<IGameEngine>()))
                .Returns(Move.PassMove);
            var engine = _builder.SetMoveStrategy(mockMoveStrategy.Object).Build();
            Move move = new Move(34); // represents user's move
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
            var mockStatusExaminer = new Mock<IGameStatusExaminer>();
            mockStatusExaminer.Setup(se => se.DetermineGameStatus(It.IsAny<IGameContext>()))
                .Returns(gameStatus);

            var engine = _builder.SetStatusExaminer(mockStatusExaminer.Object).Build();
            engine.CreateNewGame();
            Move move = new Move(34);

            //Act
            var response = await engine.UpdateBoardWithMoveAsync(move);

            //Assert
            Assert.Equal(gameStatus, response.Status);
        }
        
        public async void ShouldAutoSkipOpponentsMoveIfNoSuchMoveIsAvailable()
        {
            //Arrange
            var context = new GameContext();

            //update context to be white's turn:
            context.SetMovePlayed();

            // black articifially has no valid moves:
            var mockValidMoveFinder = new Mock<IValidMoveFinder>();
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

            var mockMoveStrategy = new Mock<IMoveStrategy>();
            mockMoveStrategy.Setup(mc => mc.ChooseMove(It.IsAny<IGameContext>(),
                It.IsAny<IGameEngine>()))
                .Returns(getWhiteMove(context));

            var mockStatusExaminer = new Mock<IGameStatusExaminer>();
            mockStatusExaminer.Setup(se => se.DetermineGameStatus(It.IsAny<IGameContext>()))
                .Returns(context.MoveNumber == 6 ? GameStatus.WhiteWins : GameStatus.InProgress);

            //set up engine
            var engine = _builder
                .SetValidMoveFinder(mockValidMoveFinder.Object)
                .SetMoveStrategy(mockMoveStrategy.Object)
                .SetStatusExaminer(mockStatusExaminer.Object)
                .Build();

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
            var externalContext = new GameContext();
            //move1:
            externalContext.SetPiece(0, Piece.Black);
            externalContext.SetMovePlayed();

            var mockMoveStrategy = new Mock<IMoveStrategy>();
            mockMoveStrategy.Setup(ms => ms.ChooseMove(It.IsAny<IGameContext>(),
                It.IsAny<IGameEngine>()))
                .Returns(new Move(1));

            var engine = _builder
                .SetMoveStrategy(mockMoveStrategy.Object)
                .SetContext(new GameContext())
                .Build();

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


    }
}


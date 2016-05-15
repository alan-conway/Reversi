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

namespace Reversi.Engine.Tests
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
            Assert.True(response.Squares.ElementAt(27).Piece == Piece.Black);
            Assert.True(response.Squares.ElementAt(28).Piece == Piece.White);
            Assert.True(response.Squares.ElementAt(35).Piece == Piece.White);
            Assert.True(response.Squares.ElementAt(36).Piece == Piece.Black);
        }

        [Fact]
        public async void ShouldSetAllSquaresAsInvalidAfterUpdatingBoardWithMove()
        {
            //Arrange
            _engine.CreateNewGame();
            Move move = new Move(34);

            //Act
            var response = await _engine.UpdateBoardAsync(move);

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
            var response = await _engine.UpdateBoardAsync(move);

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
                It.IsAny<IValidMoveFinder>())).Returns(new Move(0));
            
            //engine should capture cell 1
            var mockCaptureHelper = new Mock<ICaptureHelper>();
            mockCaptureHelper.Setup(ch => ch.CaptureEnemyPieces(It.IsAny<IGameContext>(), 0))
                .Callback(() => context[1].Piece = Piece.White);

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
            var response = await _engine.UpdateBoardAsync(move);

            //Assert
            Assert.Equal(2, _engine.MoveNumber);
        }

        [Fact]
        public async void ShouldUpdateMoveNumberAfterReplyingWithEnginesMove()
        {
            //Arrange
            var mockMoveStrategy = new Mock<IMoveStrategy>(); //finds engine's replying move
            mockMoveStrategy.Setup(ms => ms.ChooseMove(It.IsAny<IGameContext>(),
                It.IsAny<IValidMoveFinder>())).Returns(Move.PassMove);
            var engine = _builder.SetMoveStrategy(mockMoveStrategy.Object).Build();
            Move move = new Move(34); // represents user's move
            var response = await engine.UpdateBoardAsync(move); // opponent's move

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
            var response = await engine.UpdateBoardAsync(move);

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
            mockMoveStrategy.Setup(mc => mc.ChooseMove(
                It.IsAny<IGameContext>(), It.IsAny<IValidMoveFinder>()))
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
                ms.ChooseMove(It.IsAny<IGameContext>(), It.IsAny<IValidMoveFinder>()), 
                Times.Exactly(2));

            mockValidMoveFinder.Verify(vmf =>
                vmf.FindAllValidMoves(It.IsAny<IGameContext>()), 
                Times.Exactly(2));

        }
        

    }
}


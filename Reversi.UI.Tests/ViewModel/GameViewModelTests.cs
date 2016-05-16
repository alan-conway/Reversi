using Moq;
using Prism.Events;
using Reversi.Engine;
using Reversi.Events;
using Reversi.UI.Tests.Extensions;
using Reversi.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reversi.Engine.Interfaces;
using Xunit;
using Reversi.MessageDialogs;

namespace Reversi.UI.Tests.ViewModel
{
    public class GameViewModelTests
    {
        private CellSelectedEvent _cellSelectedEvent;
        private GameViewModel _gameViewModel;
        private Mock<IGameEngine> _mockGameEngine;
        private Response _response;
        private Response _responseGameOver;
        private Mock<IMessageDialogService> _mockDialogService;

        public GameViewModelTests()
        {
            _cellSelectedEvent = new CellSelectedEvent();
            var mockEventAggregator = new Mock<IEventAggregator>();
            mockEventAggregator.Setup(ea => ea.GetEvent<CellSelectedEvent>())
                .Returns(_cellSelectedEvent);

            _mockGameEngine = new Mock<IGameEngine>();

            _mockGameEngine.Setup(ge => ge.CreateNewGame())
                .Returns(new Response(
                    Move.PassMove,
                    new Square[] 
                    {
                        new Square() { Piece = Piece.Black },
                        new Square() { Piece = Piece.None, IsValidMove = true}
                    },
                    GameStatus.NewGame));

            _mockDialogService = new Mock<IMessageDialogService>();
            _mockDialogService.Setup(ds => ds.ShowYesNoDialog(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(DialogChoice.Yes);

            _gameViewModel = new GameViewModel(mockEventAggregator.Object, 
                _mockGameEngine.Object, _mockDialogService.Object);

            _response = new Response(new Move(0), new Square[] { new Square() });
            _responseGameOver = new Response(new Move(0), new Square[] { new Square() }) { Status = GameStatus.Draw };
        }

        [Fact]
        public void ShouldUpdateBoardWhenNewGameIsStarted()
        {
            //Arrange
            _gameViewModel.Board.Cells[0].Piece = Piece.White;
            _gameViewModel.Board.Cells[1].Piece = Piece.White;
            
            //Act
            _gameViewModel.NewGameCommand.Execute(null);

            //Assert
            Assert.Equal(Piece.Black, _gameViewModel.Board.Cells[0].Piece);
            Assert.False(_gameViewModel.Board.Cells[0].IsValidMove);
            Assert.Equal(Piece.None, _gameViewModel.Board.Cells[1].Piece);
            Assert.True(_gameViewModel.Board.Cells[1].IsValidMove);
        }

        [Fact]
        public void ShouldHaveNoCellsSelectedWhenNewGameStarted()
        {
            //Arrange
            _gameViewModel.Board.Cells[1].IsSelected = true;

            //Act
            _gameViewModel.NewGameCommand.Execute(null);

            //Assert
            Assert.False(_gameViewModel.Board.Cells.Any(c => c.IsSelected));
        }
        
        [Fact]
        public void ShouldSubmitMoveToEngineWhenCellSelectedEventIsPublished()
        {
            //Arrange
            int cellId = 123;
            Move move = new Move(cellId);

            _mockGameEngine.Setup(ge => ge.UpdateBoardAsync(It.IsAny<Move>()))
                   .Returns(Task.FromResult(_response));

            _mockGameEngine.Setup(ge => ge.MakeReplyMoveAsync())
                   .Returns(Task.FromResult(_response));

            //Act
            _cellSelectedEvent.Publish(cellId);

            //Assert
            _mockGameEngine.Verify(ge => 
                ge.UpdateBoardAsync(
                    It.Is<Move>(m => m.LocationPlayed == move.LocationPlayed)),
                    Times.Once);
        }

        [Theory]
        [InlineData(false, 1)]
        [InlineData(true, 0)]
        public void ShouldWaitForResponseWhenCellSelectedEventIsPublished(bool isGameOver, int expectedCalls)
        {
            //Arrange
            int cellId = 123;
            Move move = new Move(cellId);

            _mockGameEngine.Setup(ge => ge.UpdateBoardAsync(It.IsAny<Move>()))
                .Returns(Task.FromResult(isGameOver ? _responseGameOver : _response));

            _mockGameEngine.Setup(ge => ge.MakeReplyMoveAsync())
                .Returns(Task.FromResult(_response));

            //Act
            _cellSelectedEvent.Publish(cellId);

            //Assert
            _mockGameEngine.Verify(ge => ge.MakeReplyMoveAsync(), Times.Exactly(expectedCalls));
        }

        [Fact]
        public void ShouldSetCellSelectedWhenEngineRespondsWithAMove()
        {
            //Arrange
            int cellId = 1;
            Move move = new Move(cellId);

            _mockGameEngine.Setup(ge => ge.UpdateBoardAsync(It.IsAny<Move>()))
                .Returns(Task.FromResult(new Response(move,new Square[0])));

            _mockGameEngine.Setup(ge => ge.MakeReplyMoveAsync())
                .Returns(Task.FromResult(new Response(
                    new Move(0), 
                    new Square[]{ new Square(),new Square() })));

            //Act
            _cellSelectedEvent.Publish(cellId);

            //Assert
            Assert.True(_gameViewModel.Board.Cells[_response.Move.LocationPlayed].IsSelected);
            Assert.Equal(1, _gameViewModel.Board.Cells.Count(c => c.IsSelected));
        }

        [Theory]
        [InlineData(true, 2, 2, GameStatus.NewGame, "")]
        [InlineData(false, 10, 10, GameStatus.InProgress, "Black: 10  White: 10")]
        [InlineData(false, 15, 5, GameStatus.InProgress, "Black: 15  White: 5")]
        [InlineData(false, 10, 10, GameStatus.Draw, "Game is a draw  (Black: 10  White: 10)")]
        [InlineData(false, 15, 5, GameStatus.BlackWins, "Black wins  (Black: 59  White: 5)")]
        [InlineData(false, 14, 20, GameStatus.WhiteWins, "White wins  (Black: 14  White: 50)")]
        public void ShouldDisplayCorrectStatusMessage(bool newGame,
            int numBlackCells, int numWhiteCells, 
            GameStatus status, string expectedMessage)
        {
            //System.Diagnostics.Debugger.Launch();
            //Arrange
            Move move = new Move(0);

            _mockGameEngine.Setup(ge => ge.UpdateBoardAsync(It.IsAny<Move>()))
                .Returns(Task.FromResult(new Response(move, new Square[0])));

            var squares = Enumerable.Range(0,64).Select(x => new Square()).ToArray();
            for(int i = 0; i < numBlackCells; i++)
            {
                squares[i].Piece = Piece.Black;
            }
            for (int i = 63; i > 63-numWhiteCells; i--)
            {
                squares[i].Piece = Piece.White;
            }

            _mockGameEngine.Setup(ge => ge.MakeReplyMoveAsync())
                .Returns(Task.FromResult(new Response(
                    new Move(63),
                    squares,
                    status)));

            //Act
            if (newGame)
            {
                _gameViewModel.NewGameCommand.Execute(null);
            }
            else
            {
                _cellSelectedEvent.Publish(move.LocationPlayed);
            }

            //Assert
            Assert.Equal(expectedMessage, _gameViewModel.StatusMessage);
        }


        [Theory]
        [InlineData(false, 1)]
        [InlineData(true, 0)]
        public void ShouldDisplayYesNoDialogOnlyWhenNewGameIsRequestedMidGame(
            bool isGameOver, int numExpectedCalls)
        {
            //Arrange
            int cellId = 0;
            Move move = new Move(cellId);

            _mockGameEngine.Setup(ge => ge.UpdateBoardAsync(It.IsAny<Move>()))
                .Returns(Task.FromResult(isGameOver ? _responseGameOver : _response));

            _mockGameEngine.Setup(ge => ge.MakeReplyMoveAsync())
                .Returns(Task.FromResult(isGameOver ? _responseGameOver : _response));

            _cellSelectedEvent.Publish(cellId);

            //Act
            _gameViewModel.NewGameCommand.Execute(null);

            //Assert
            _mockDialogService.Verify(ds => 
                ds.ShowYesNoDialog(It.IsAny<string>(), It.IsAny<string>()), 
                Times.Exactly(numExpectedCalls));
        }

        [Theory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void ShouldOnlyStartNewGameWhenUserRepliesYesToDialogPrompt(
            bool userClicksYes, int numExpectedCalls)
        {
            //Arrange - put game in progress
            int cellId = 0;
            Move move = new Move(cellId);

            _mockGameEngine.Setup(ge => ge.UpdateBoardAsync(It.IsAny<Move>()))
                .Returns(Task.FromResult(_response));

            _mockGameEngine.Setup(ge => ge.MakeReplyMoveAsync())
                .Returns(Task.FromResult(_response));

            _mockDialogService.Setup(ds => ds.ShowYesNoDialog(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(userClicksYes ? DialogChoice.Yes : DialogChoice.No);

            _cellSelectedEvent.Publish(cellId);

            //Act
            _gameViewModel.NewGameCommand.Execute(null);

            //Assert
            // Adding 1 since the first call was already made on construction
            _mockGameEngine.Verify(ge => ge.CreateNewGame(),
                Times.Exactly(1 + numExpectedCalls));
        }


    }
}

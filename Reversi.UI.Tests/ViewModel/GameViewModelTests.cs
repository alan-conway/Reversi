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
using Reversi.Services.MessageDialogs;
using Reversi.Engine.Core;
using Reversi.Services;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture;

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

            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var mockEventAggregator = fixture.Freeze<Mock<IEventAggregator>>();
            mockEventAggregator.Setup(ea => ea.GetEvent<CellSelectedEvent>())
                .Returns(_cellSelectedEvent);

            _mockGameEngine = fixture.Freeze<Mock<IGameEngine>>();

            _mockGameEngine.Setup(ge => ge.CreateNewGame())
                .Returns(new Response(
                    Move.PassMove,
                    new Square[]
                    {
                        new Square(Piece.Black, false),
                        new Square(Piece.None, true)
                    },
                    GameStatus.NewGame));

            _mockDialogService = fixture.Freeze<Mock<IMessageDialogService>>();
            _mockDialogService.Setup(ds => ds.ShowYesNoDialog(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(DialogChoice.Yes);
            _mockDialogService.Setup(ds => ds.ShowOptionsDialog(It.IsAny<IDialogViewModel>()))
                .Returns(DialogChoice.Ok);

            var mockDelayProvider = fixture.Freeze<Mock<IDelayProvider>>();
            mockDelayProvider.Setup(dp => dp.Delay(It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            _gameViewModel = fixture.Create<GameViewModel>();

            _response = new Response(new Move(0), new Square[] { new Square(Piece.None, true) });
            _responseGameOver = new Response(new Move(0), new Square[] { new Square(Piece.None, false) }) { Status = GameStatus.Draw };
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

            _mockGameEngine.Setup(ge => ge.UpdateBoardWithMoveAsync(It.IsAny<Move>()))
                   .Returns(Task.FromResult(_response));

            _mockGameEngine.Setup(ge => ge.MakeReplyMoveAsync())
                   .Returns(Task.FromResult(_response));

            //Act
            _cellSelectedEvent.Publish(cellId);

            //Assert
            _mockGameEngine.Verify(ge =>
                ge.UpdateBoardWithMoveAsync(
                    It.Is<Move>(m => m.LocationPlayed == move.LocationPlayed)),
                    Times.Once());
        }

        [Theory]
        [InlineData(false, 1)]
        [InlineData(true, 0)]
        public void ShouldWaitForResponseWhenCellSelectedEventIsPublished(bool isGameOver, int expectedCalls)
        {
            //Arrange
            int cellId = 123;
            Move move = new Move(cellId);

            _mockGameEngine.Setup(ge => ge.UpdateBoardWithMoveAsync(It.IsAny<Move>()))
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

            _mockGameEngine.Setup(ge => ge.UpdateBoardWithMoveAsync(It.IsAny<Move>()))
                .Returns(Task.FromResult(new Response(move,new Square[0])));

            _mockGameEngine.Setup(ge => ge.MakeReplyMoveAsync())
                .Returns(Task.FromResult(new Response(
                    new Move(0),
                    new Square[]{ new Square(Piece.None, true),new Square(Piece.None, false) })));

            //Act
            _cellSelectedEvent.Publish(cellId);

            //Assert
            Assert.True(_gameViewModel.Board.Cells[_response.Move.LocationPlayed].IsSelected);
            Assert.Equal(1, _gameViewModel.Board.Cells.Count(c => c.IsSelected));
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

            _mockGameEngine.Setup(ge => ge.UpdateBoardWithMoveAsync(It.IsAny<Move>()))
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

            _mockGameEngine.Setup(ge => ge.UpdateBoardWithMoveAsync(It.IsAny<Move>()))
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

        [Fact]
        public void ShouldDisplayOptionsDialogWhenOptionsCommandExecuted()
        {
            //Arrange
            var mockGameOptions = new Mock<IGameOptions>();

            _mockGameEngine.Setup(ge => ge.GameOptions)
                .Returns(mockGameOptions.Object);

            //Act
            _gameViewModel.ShowOptionsCommand.Execute(null);

            //Assert
            _mockDialogService.Verify(ds => ds.ShowOptionsDialog(It.IsAny<IDialogViewModel>()), Times.Once());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ShouldAssignGameOptionsWhenOptionsChangesAreSaved(bool userPlaysBlack)
        {
            //Arrange
            var gameOptions = new GameOptions()
            {
                UserPlaysAsBlack = userPlaysBlack
            };

            _mockGameEngine.Setup(ge => ge.GameOptions)
                .Returns(gameOptions);

            //Act
            _gameViewModel.ShowOptionsCommand.Execute(null);

            //Assert
            _mockGameEngine.VerifySet(ge =>
                ge.GameOptions = It.Is<IGameOptions>(go => go.UserPlaysAsBlack == userPlaysBlack),
            Times.Once());
        }

        [Fact]
        public void ShouldNotAssignGameOptionsWhenOptionsChangesAreNotSaved()
        {
            //Arrange
            var mockGameOptions = new Mock<IGameOptions>();

            _mockGameEngine.Setup(ge => ge.GameOptions)
                .Returns(mockGameOptions.Object);

            _mockDialogService.Setup(ds => ds.ShowOptionsDialog(It.IsAny<IDialogViewModel>()))
                .Returns(DialogChoice.Cancel);

            //Act
            _gameViewModel.ShowOptionsCommand.Execute(null);

            //Assert
            _mockGameEngine.VerifySet(ge => ge.GameOptions = It.IsAny<IGameOptions>(),
                Times.Never());
        }

        [Fact]
        public async void ShouldAutomaticallyPassUsersMoveIfThereAreNoValidMovesToPlay()
        {
            //Arrange
            int cellId = 1;
            Move move = new Move(cellId);

            _mockGameEngine.Setup(ge => ge.UpdateBoardWithMoveAsync(It.Is<Move>(m => !m.Pass)))
                .Returns(Task.FromResult(new Response(move, new Square[0])));

            _mockGameEngine.Setup(ge => ge.UpdateBoardWithMoveAsync(It.Is<Move>(m => m.Pass)))
                .Returns(Task.FromResult(_responseGameOver));

            _mockGameEngine.Setup(ge => ge.MakeReplyMoveAsync())
                .Returns(Task.FromResult(new Response(
                    new Move(0),
                    new Square[] { new Square(Piece.None, false), new Square(Piece.None, false) })));

            //Act
            await _gameViewModel.PlayMove(move); // invoking internal method

            //Assert
            _mockGameEngine.Verify(ge => ge.UpdateBoardWithMoveAsync(It.Is<Move>(m => m.Pass)), Times.Once());
        }

        [Theory]
        [InlineData(true, true, 2)]
        [InlineData(false, true, 2)]
        [InlineData(true, false, 1)]
        [InlineData(false, false, 1)]
        public void ShouldInitialiseNewGameIfFirstPlayerOptionChangesAtStartOfGame(
            bool userPlaysBlack, bool isStartOfGame, int expectedCalls)
        {
            //Arrange                        
            var initialOptions = new GameOptions() { UserPlaysAsBlack = userPlaysBlack };
            var altOptions = new GameOptions() { UserPlaysAsBlack = !userPlaysBlack };
            _mockGameEngine.Setup(ge => ge.GameOptions).Returns(initialOptions);

            //simulate changing game options to switch first player
            _mockGameEngine.SetupSet(ge => ge.GameOptions = It.IsAny<IGameOptions>())
                .Callback(() => _mockGameEngine.Setup(ge => ge.GameOptions)
                                .Returns(altOptions));

            //simulate being at start or later in the game
            var context = new GameContext();
            if (!userPlaysBlack) { context.SetMovePlayed(); }
            if (!isStartOfGame) { context.SetMovePlayed(); context.SetMovePlayed(); }
            _mockGameEngine.Setup(ge => ge.Context).Returns(context);
            _mockGameEngine.Setup(ge => ge.MoveNumber).Returns(context.MoveNumber);

            //Act
            _gameViewModel.ShowOptionsCommand.Execute(null);

            //Assert
            _mockGameEngine.Verify(ge => ge.CreateNewGame(), Times.Exactly(expectedCalls));
        }

    }
}

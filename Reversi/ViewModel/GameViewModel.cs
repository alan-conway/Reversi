using Prism.Commands;
using Prism.Events;
using Reversi.Engine;
using Reversi.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Reversi.Engine.Interfaces;
using Reversi.MessageDialogs;

namespace Reversi.ViewModel
{
    public class GameViewModel : ViewModelBase
    {
        private IGameEngine _engine;
        private GameStatus _gameStatus;
        private string _statusMessage;
        private IMessageDialogService _dialogService;

        public GameViewModel(IEventAggregator eventAggregator, IGameEngine engine, 
            IMessageDialogService dialogService)
        {
            _engine = engine;
            _dialogService = dialogService;
            _gameStatus = GameStatus.NewGame;
            eventAggregator.GetEvent<CellSelectedEvent>().Subscribe(OnCellSelected);
            Board = new BoardViewModel(eventAggregator);
            NewGameCommand = new DelegateCommand(InitialiseNewGame);
            InitialiseNewGame();
        }
        
        public BoardViewModel Board { get; set; }

        public ICommand NewGameCommand { get; }

        public string StatusMessage
        {
            get { return _statusMessage; }
            set
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;
                    Notify();
                }
            }
        }

        private void InitialiseNewGame()
        {
            if (_gameStatus == GameStatus.InProgress)
            {
                if (_dialogService.ShowYesNoDialog("New Game",
                    "Would you like to start a new game?") != DialogChoice.Yes)
                {
                    return;
                }
            }

            Board.Cells.ForEach(c =>
            {
                c.Piece = Piece.None;
                c.IsValidMove = false;
                c.IsSelected = false;
            });

            var response = _engine.CreateNewGame();
            ProcessResponseFromEngine(response);
        }

        private async void OnCellSelected(int cellId)
        {
            var move = new Move(cellId);
            var response = await _engine.UpdateBoardWithMoveAsync(move);
            ProcessResponseFromEngine(response);

            if (response.Status == GameStatus.InProgress)
            {
                response = await _engine.MakeReplyMoveAsync();
                ProcessResponseFromEngine(response);
            }            
        }

        private void ProcessResponseFromEngine(Response response)
        {
            _gameStatus = response.Status;
            for (int i = 0; i < response.Squares.Length; i++)
            {
                Board.Cells[i].Piece = response.Squares[i].Piece;
                Board.Cells[i].IsValidMove = response.Squares[i].IsValidMove;

                if (!response.Move.Pass)
                {
                    Board.Cells[i].IsSelected = (i == response.Move.LocationPlayed);
                }
            }
            StatusMessage = GetStatusMessage(response);
        }

        private string GetStatusMessage(Response response)
        {
            if (response.Status == GameStatus.NewGame)
            {
                return string.Empty;
            }

            int blackScore, whiteScore;
            CalculateScores(response, out blackScore, out whiteScore);
            var score = $"Black: {blackScore}  White: {whiteScore}";

            switch (response.Status)
            {
                case GameStatus.Draw: return $"Game is a draw  ({score})";
                case GameStatus.BlackWins: return $"Black wins  ({score})";
                case GameStatus.WhiteWins: return $"White wins  ({score})";
                default: return score;
            }
        }

        private void CalculateScores(Response response, out int blackScore, out int whiteScore)
        {
            blackScore = response.Squares.Count(s => s.Piece == Piece.Black);
            whiteScore = response.Squares.Count(s => s.Piece == Piece.White);

            // convention is to award the empty squares to the victor
            if (response.Status != GameStatus.InProgress)
            {
                int numEmptySquares = response.Squares.Count(s => s.Piece == Piece.None);
                if (blackScore > whiteScore)
                {
                    blackScore += numEmptySquares;
                }
                else if (whiteScore > blackScore)
                {
                    whiteScore += numEmptySquares;
                }
            }
        }
    }
}

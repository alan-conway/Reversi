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
using Reversi.Services.MessageDialogs;
using Reversi.Engine.Core;
using Reversi.Services;

namespace Reversi.ViewModel
{
    public class GameViewModel : ViewModelBase
    {
        private IGameEngine _engine;
        private GameStatus _gameStatus;
        private string _statusMessage;
        private IMessageDialogService _dialogService;
        private IStatusMessageFormatter _statusMsgFormatter;
        private IDelayProvider _delayProvider;
        private IConfigurationService _configService;

        public GameViewModel(IEventAggregator eventAggregator, IGameEngine engine, 
            IMessageDialogService dialogService, IStatusMessageFormatter statusMsgFormatter,
            IDelayProvider delayProvider, IConfigurationService configService)
        {
            _engine = engine;
            _dialogService = dialogService;
            _statusMsgFormatter = statusMsgFormatter;
            _delayProvider = delayProvider;
            _configService = configService;
            _gameStatus = GameStatus.NewGame;
            eventAggregator.GetEvent<CellSelectedEvent>().Subscribe(OnCellSelected);
            Board = new BoardViewModel(eventAggregator);
            NewGameCommand = new DelegateCommand(InitialiseNewGame);
            ShowOptionsCommand = new DelegateCommand(ShowOptionsWindow);

            Initialise();
        }

        public BoardViewModel Board { get; set; }

        public ICommand NewGameCommand { get; }

        public ICommand ShowOptionsCommand { get; }

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

        private void Initialise()
        {
            _engine.GameOptions = _configService.GameOptions;
            InitialiseNewGame();
        }

        private void InitialiseNewGame()
        {
            InitialiseNewGame(true);
        }

        private void InitialiseNewGame(bool allowPrompt)
        {
            if (allowPrompt && !ShowPromptIfGameInProgress())
            {
                return;
            }

            ResetCellsInBoard();
            var response = _engine.CreateNewGame();
            ProcessResponseFromEngine(response);
        }

        private bool ShowPromptIfGameInProgress()
        {
            if (_gameStatus == GameStatus.InProgress)
            {
                var title = "New Game";
                var message = "Would you like to start a new game?";
                return _dialogService.ShowYesNoDialog(title, message) == DialogChoice.Yes;
            }
            return true;
        }

        private void ResetCellsInBoard()
        {
            Board.Cells.ForEach(c =>
            {
                c.Piece = Piece.None;
                c.IsValidMove = false;
                c.IsSelected = false;
            });
        }

        private void ShowOptionsWindow()
        {
            var optionsViewModel = new OptionsViewModel(_engine.GameOptions, _engine.AvailableStrategies);
            if (_dialogService.ShowOptionsDialog(optionsViewModel) == DialogChoice.Ok)
            {
                var newGameOptions = optionsViewModel.ToGameOptions();
                _engine.GameOptions = newGameOptions;
                _configService.GameOptions = newGameOptions;
            }

            ApplyOptions(_engine.GameOptions);
        }

        /// <summary>
        /// Represents the user having played a move
        /// </summary>
        /// <param name="cellId">the location of the move played</param>
        /// <remarks>
        /// Requests the engine to respond with the result of the users move, to
        /// show the pieces captured. This async response is awaited so that the 
        /// application remains responsive in the meantime.
        /// Similarly, the engines replying move is requested in the same manner.
        /// </remarks>
        private async void OnCellSelected(int cellId)
        {
            var move = new Move(cellId);
            await PlayMove(move);
        }

        internal async Task PlayMove(Move move)
        {
            var response = await _engine.UpdateBoardWithMoveAsync(move);
            ProcessResponseFromEngine(response);

            if (response.Status == GameStatus.InProgress)
            {
                response = await FetchAndProcessReplyMove(response);
                await PassIfNoMovesAvailable(response);
            }
        }
        
        private async Task<Response> FetchAndProcessReplyMove(Response response)
        {
            // since the engine can reply 'too quickly' for our original
            // move to be seen, combine the replyMoveTask with a delay
            var pauseTask = _delayProvider.Delay(100);
            var replyMoveTask = _engine.MakeReplyMoveAsync();

            await Task.WhenAll(pauseTask, replyMoveTask);
            response = replyMoveTask.Result;
            ProcessResponseFromEngine(response);
            return response;
        }

        private async Task PassIfNoMovesAvailable(Response response)
        {
            if (response.Status == GameStatus.InProgress &&
                response.Squares.All(s => !s.IsValidMove))
            {
                await PassMove();
            }
        }

        private async Task PassMove()
        {
            // since the engine can reply too quickly add a delay 
            // when passing to deliniate its moves
            await _delayProvider.Delay(250)
                .ContinueWith(t => PlayMove(Move.PassMove));
        }

        /// <summary>
        /// Updates the board according to the response from the engine
        /// </summary>
        /// <param name="response"></param>
        private void ProcessResponseFromEngine(Response response)
        {
            _gameStatus = response.Status;
            UpdateBoardWithResponseData(response);
            StatusMessage = _statusMsgFormatter.GetStatusMessage(response.Status, response.Squares);
        }

        private void UpdateBoardWithResponseData(Response response)
        {
            for (int i = 0; i < response.Squares.Length; i++)
            {
                Board.Cells[i].Piece = response.Squares[i].Piece;
                Board.Cells[i].IsValidMove = response.Squares[i].IsValidMove;
                UpdateCellSelectedStatus(response, i);
            }
        }

        private void UpdateCellSelectedStatus(Response response, int cellId)
        {
            if (!response.Move.Pass)
            {
                Board.Cells[cellId].IsSelected = (cellId == response.Move.LocationPlayed);
            }
        }

        private void ApplyOptions(IGameOptions options)
        {
            bool firstPlayerHasChanged = (
                (options.UserPlaysAsBlack && _engine.Context.CurrentPiece == Piece.White) ||
                (!options.UserPlaysAsBlack && _engine.Context.CurrentPiece == Piece.Black));

            if (firstPlayerHasChanged && _engine.MoveNumber <=2)
            {
                InitialiseNewGame(false);
            }
        }

       
    }
}

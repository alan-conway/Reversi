﻿using Prism.Commands;
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

        public GameViewModel(IEventAggregator eventAggregator, IGameEngine engine, 
            IMessageDialogService dialogService, IStatusMessageFormatter statusMsgFormatter)
        {
            _engine = engine;
            _dialogService = dialogService;
            _statusMsgFormatter = statusMsgFormatter;
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
            var response = await _engine.UpdateBoardWithMoveAsync(move);
            ProcessResponseFromEngine(response);

            if (response.Status == GameStatus.InProgress)
            {
                response = await _engine.MakeReplyMoveAsync();
                ProcessResponseFromEngine(response);
            }
        }

        /// <summary>
        /// Updates the board according to the response from the engine
        /// </summary>
        /// <param name="response"></param>
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
            StatusMessage = _statusMsgFormatter.GetStatusMessage(response.Status, response.Squares);
        }

        

       
    }
}

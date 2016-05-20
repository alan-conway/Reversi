﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reversi.Engine.Interfaces;

namespace Reversi.Engine.Core
{
    /// <summary>
    /// The engine that processes moves made by the opponent and 
    /// generates moves in response
    /// </summary>
    public class GameEngine : IGameEngine
    {
        private IGameContext _context;
        private ICaptureHelper _captureHelper;
        private IValidMoveFinder _validMoveFinder;
        private IMoveStrategy _moveStrategy;
        private IGameStatusExaminer _statusExaminer;

        public GameEngine(IGameContext context,
            ICaptureHelper captureHelper, 
            IValidMoveFinder validMoveFinder,
            IMoveStrategy moveStrategy,
            IGameStatusExaminer statusExaminer)
        {
            _context = context;
            _captureHelper = captureHelper;
            _validMoveFinder = validMoveFinder;
            _moveStrategy = moveStrategy;
            _statusExaminer = statusExaminer;
        }

        public IGameContext Context
        {
            get { return _context; }
        }

        public Response CreateNewGame()
        {
            //Initialise all to be blank
            _context.Initialise();

            //now set the 2 black, 2 white pieces for the start of a new game
            _context.SetPiece(28, Piece.Black);
            _context.SetPiece(35, Piece.Black);
            _context.SetPiece(27, Piece.White);
            _context.SetPiece(36, Piece.White);

            MarkAnyValidMoves();
            return new Response(Move.PassMove, 
                _context.Squares,
                GameStatus.NewGame);
        }
               
        public int MoveNumber {  get { return _context.MoveNumber; } }

        /// <summary>
        /// Send the players move to the engine
        /// </summary>
        /// <param name="move">The move played</param>
        /// <returns>The updated board, having played the move submitted</returns>
        public async Task<Response> UpdateBoardWithMoveAsync(Move move)
        {
            return await Task.Run(() => UpdateBoardWithMove(move, _context));            
        }

        /// <summary>
        /// Update the game to reflect the opponents move
        /// </summary>
        /// <param name="move">The move made by the opponent</param>
        /// <param name="context">The context to update when making the move</param>
        /// <returns>An updated board which reflects pieces captured by the opponent</returns>
        public Response UpdateBoardWithMove(Move move, IGameContext context)
        {
            if (!move.Pass)
            {
                context.SetPiece(move.LocationPlayed, context.CurrentPiece);
                _captureHelper.CaptureEnemyPieces(context, move.LocationPlayed);
            }
            context.SetAllInvalid();
            context.SetMovePlayed();

            var status = _statusExaminer.DetermineGameStatus(context);
            return new Response(move, context.Squares, status);
        }

        /// <summary>
        /// Wait for the engine to respond with its own move
        /// </summary>
        /// <returns>The updated board containing the engine's move</returns>
        public async Task<Response> MakeReplyMoveAsync()
        {
            return await Task.Run(() => MakeReplyMove(_context));
        }

        /// <summary>
        /// Makes a move in reply to the opponent
        /// </summary>
        /// <param name="context">The context to update when making the replying move</param>
        /// <returns>An updated board which reflects any captured pieces</returns>
        public Response MakeReplyMove(IGameContext context)
        {
            var move = _moveStrategy.ChooseMove(context, this);
            if (!move.Pass)
            {
                _context.SetPiece(move.LocationPlayed, context.CurrentPiece);
                _captureHelper.CaptureEnemyPieces(context, move.LocationPlayed);
            }

            context.SetMovePlayed();

            var validSubsequentMoves = _validMoveFinder.FindAllValidMoves(context);
            MarkAnyValidMoves();

            var status = _statusExaminer.DetermineGameStatus(context);

            if (!validSubsequentMoves.Any() && status == GameStatus.InProgress)
            {
                // opponent has no valid moves but game is not over:
                _context.SetMovePlayed(); // auto-skip opponents move
                return MakeReplyMove(context); // play another move (recursively)                
            }
            else
            {
                return new Response(move, context.Squares, status);
            }
        }
        
        private static Piece GetEnemyPiece(Piece piece)
        {
            return piece == Piece.Black ? Piece.White : Piece.Black;
        }
        
        private void MarkAnyValidMoves(IEnumerable<int> validMoves = null)
        {
            if (validMoves == null)
            {
                validMoves = _validMoveFinder.FindAllValidMoves(_context);
            }
            foreach (var index in validMoves)
            {
                _context.SetValid(index, true);
            }
        }

        


    }
}

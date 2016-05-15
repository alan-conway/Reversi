using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.Engine
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
        private IMoveChooser _moveFinder;
        private IGameStatusExaminer _statusExaminer;

        public GameEngine(IGameContext context,
            ICaptureHelper captureHelper, 
            IValidMoveFinder validMoveFinder,
            IMoveChooser moveFinder,
            IGameStatusExaminer statusExaminer)
        {
            _context = context;
            _captureHelper = captureHelper;
            _validMoveFinder = validMoveFinder;
            _moveFinder = moveFinder;
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
            _context[27].Piece = _context[36].Piece = Piece.Black;
            _context[28].Piece = _context[35].Piece = Piece.White;
            
            MarkAnyValidMoves();
            return new Response(Move.PassMove, _context.Squares);
        }
               
        public int MoveNumber {  get { return _context.MoveNumber; } }

        /// <summary>
        /// Send the players move to the engine
        /// </summary>
        /// <param name="move">The move played</param>
        /// <returns>The updated board, having played the move submitted</returns>
        public async Task<Response> UpdateBoardAsync(Move move)
        {
            return await Task.Run(() => UpdateBoardWithMove(move));            
        }

        /// <summary>
        /// Wait for the engine to respond with its own move
        /// </summary>
        /// <returns>The updated board containing the engine's move</returns>
        public async Task<Response> MakeReplyMoveAsync()
        {
            return await Task.Run(() => MakeReplyMove());
        }

        /// <summary>
        /// Update the game to reflect the opponents move
        /// </summary>
        /// <param name="move">The move made by the opponent</param>
        /// <returns>An updated board which reflects pieces captured by the opponent</returns>
        private Response UpdateBoardWithMove(Move move)
        {
            if (!move.Pass)
            {
                _context[move.LocationPlayed].Piece = _context.CurrentPiece;
                _captureHelper.CaptureEnemyPieces(_context, move.LocationPlayed);
            }
            _context.SetAllInvalid();
            _context.SetMovePlayed();

            var status = _statusExaminer.DetermineGameStatus(_context);
            return new Response(move, _context.Squares, status);
        }

        /// <summary>
        /// Makes a move in reply to the opponent
        /// </summary>
        /// <returns>An updated board which reflects any captured pieces</returns>
        private Response MakeReplyMove()
        {
            var move = _moveFinder.ChooseMove(_context, _validMoveFinder);
            if (!move.Pass)
            {
                _context[move.LocationPlayed].Piece = _context.CurrentPiece;
                _captureHelper.CaptureEnemyPieces(_context, move.LocationPlayed);
            }

            _context.SetMovePlayed();

            var validSubsequentMoves = _validMoveFinder.FindAllValidMoves(_context);
            MarkAnyValidMoves();

            var status = _statusExaminer.DetermineGameStatus(_context);

            if (!validSubsequentMoves.Any() && status == GameStatus.InProgress)
            {
                // opponent has no valid moves but game is not over:
                _context.SetMovePlayed(); // auto-skip opponents move
                return MakeReplyMove(); // play another move (recursively)                
            }
            else
            {
                return new Response(move, _context.Squares, status);
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
                _context[index].IsValidMove = true;
            }
        }

        


    }
}


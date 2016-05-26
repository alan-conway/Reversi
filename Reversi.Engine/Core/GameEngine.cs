using System;
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
        private IGameOptions _options;
        private ICaptureHelper _captureHelper;
        private IValidMoveFinder _validMoveFinder;
        private IStrategyProvider _strategyProvider; 
        private IGameStatusExaminer _statusExaminer;
        private IMoveStrategy _moveStrategy;

        public GameEngine(IGameContext context,
            IGameOptions options,
            ICaptureHelper captureHelper,
            IValidMoveFinder validMoveFinder,
            IStrategyProvider strategyProvider,
            IGameStatusExaminer statusExaminer)
        {
            Context = context;
            _options = options;
            _captureHelper = captureHelper;
            _validMoveFinder = validMoveFinder;
            _strategyProvider = strategyProvider;
            _statusExaminer = statusExaminer;

            AvailableStrategies = _strategyProvider.GetStrategyInfoCollection();
            ApplyOptions(options);
        }

        public IGameContext Context { get; }

        public IGameOptions GameOptions
        {
            get { return _options.Clone(); }
            set
            {
                _options = value.Clone();
                ApplyOptions(_options);
            }
        }

        public IEnumerable<StrategyInfo> AvailableStrategies { get; }
        
        public Response CreateNewGame()
        {
            //Initialise all to be blank
            Context.Initialise();

            //now set the 2 black, 2 white pieces for the start of a new game
            Context.SetPiece(28, Piece.Black);
            Context.SetPiece(35, Piece.Black);
            Context.SetPiece(27, Piece.White);
            Context.SetPiece(36, Piece.White);

            if (GameOptions.UserPlaysAsBlack)
            {
                MarkAnyValidMoves();
                return new Response(Move.PassMove,
                    Context.Squares,
                    GameStatus.NewGame);
            }
            else
            {
                return MakeReplyMove(Context);
            }
        }
        
        public int MoveNumber {  get { return Context.MoveNumber; } }

        /// <summary>
        /// Send the players move to the engine
        /// </summary>
        /// <param name="move">The move played</param>
        /// <returns>The updated board, having played the move submitted</returns>
        public async Task<Response> UpdateBoardWithMoveAsync(Move move)
        {
            return await Task.Run(() => UpdateBoardWithMove(move, Context));            
        }

        /// <summary>
        /// Update the game to reflect the opponents move
        /// </summary>
        /// <param name="move">The move made by the opponent</param>
        /// <param name="moveContext">The context to update when making the move</param>
        /// <returns>An updated board which reflects pieces captured by the opponent</returns>
        public Response UpdateBoardWithMove(Move move, IGameContext moveContext)
        {
            if (!move.Pass)
            {
                moveContext.SetPiece(move.LocationPlayed, moveContext.CurrentPiece);
                _captureHelper.CaptureEnemyPieces(moveContext, move.LocationPlayed);
            }
            moveContext.SetAllInvalid();
            moveContext.SetMovePlayed();

            var status = _statusExaminer.DetermineGameStatus(moveContext);
            return new Response(move, moveContext.Squares, status);
        }

        /// <summary>
        /// Wait for the engine to respond with its own move
        /// </summary>
        /// <returns>The updated board containing the engine's move</returns>
        public async Task<Response> MakeReplyMoveAsync()
        {
            return await Task.Run(() => MakeReplyMove(Context));
        }

        /// <summary>
        /// Makes a move in reply to the opponent
        /// </summary>
        /// <param name="moveContext">The context to update when making the replying move</param>
        /// <returns>An updated board which reflects any captured pieces</returns>
        public Response MakeReplyMove(IGameContext moveContext)
        {
            var move = _moveStrategy.ChooseMove(moveContext, this);
            if (!move.Pass)
            {
                Context.SetPiece(move.LocationPlayed, moveContext.CurrentPiece);
                _captureHelper.CaptureEnemyPieces(moveContext, move.LocationPlayed);
            }

            moveContext.SetMovePlayed();

            MarkAnyValidMoves();

            var status = _statusExaminer.DetermineGameStatus(moveContext);

            return new Response(move, moveContext.Squares, status);
        }
        
        private static Piece GetEnemyPiece(Piece piece)
        {
            return piece == Piece.Black ? Piece.White : Piece.Black;
        }
        
        private void MarkAnyValidMoves(IEnumerable<int> validMoves = null)
        {
            if (validMoves == null)
            {
                validMoves = _validMoveFinder.FindAllValidMoves(Context);
            }
            foreach (var index in validMoves)
            {
                Context.SetValid(index, true);
            }
        }

        private void ApplyOptions(IGameOptions options)
        {
            _moveStrategy = _strategyProvider.GetStrategy(options.StrategyName);

            if (_moveStrategy.StrategyInfo.IsMultiLevel)
            {
                _moveStrategy.SetLevel(options.StrategyLevel);
            }
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.Engine
{
    /// <summary>
    /// The response from the engine, containing the updated state of the game
    /// </summary>
    public class Response
    {
        public Response(Move move, Square[] squares)
        {
            Move = move;
            Squares = squares;
            Status = GameStatus.InProgress;
        }

        public Response(Move move, Square[] squares, GameStatus status)
        {
            Move = move;
            Squares = squares;
            Status = status;
        }

        public Move Move { get; internal set; }

        public Square[] Squares { get; internal set; }

        public GameStatus Status { get; internal set; } 
    }
}

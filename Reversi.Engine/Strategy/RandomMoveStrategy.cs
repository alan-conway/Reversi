using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reversi.Engine.Interfaces;

namespace Reversi.Engine.Strategy
{
    public class RandomMoveStrategy : IMoveStrategy
    {
        /// <summary>
        /// Choose any random valid move
        /// </summary>
        public Move ChooseMove(IGameContext context, IValidMoveFinder validMoveFinder)
        {
            var allMoves = validMoveFinder.FindAllValidMoves(context);
            if (allMoves.Any())
            {
                var random = new Random();
                int randomIndex = random.Next(0, allMoves.Count());
                return new Move(allMoves.ElementAt(randomIndex));
            }
            else
            {
                return Move.PassMove;
            }
        }
    }
}

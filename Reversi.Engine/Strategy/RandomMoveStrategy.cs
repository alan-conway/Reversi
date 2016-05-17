using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reversi.Engine.Interfaces;

namespace Reversi.Engine.Strategy
{
    public class RandomMoveStrategy : IRandomMoveStrategy
    {
        private IRandomiser _random;

        public RandomMoveStrategy(IRandomiser random)
        {
            _random = random;
        }

        /// <summary>
        /// Choose any random valid move
        /// </summary>
        public Move ChooseMove(IGameEngine engine, IGameContext context, IValidMoveFinder validMoveFinder)
        {
            var allMoves = validMoveFinder.FindAllValidMoves(context);
            if (allMoves.Any())
            {
                int randomIndex = _random.Next(0, allMoves.Count());
                return new Move(allMoves.ElementAt(randomIndex));
            }
            else
            {
                return Move.PassMove;
            }
        }
    }
}

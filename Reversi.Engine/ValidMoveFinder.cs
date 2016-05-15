using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.Engine
{
    /// <summary>
    /// Determines which moves are valid for the current player
    /// </summary>
    public class ValidMoveFinder : IValidMoveFinder
    {
        private ILocationHelper _locationHelper;

        public ValidMoveFinder(ILocationHelper locationHelper)
        {
            _locationHelper = locationHelper;
        }

        public bool IsValidMove(IGameContext context, int location)
        {
            if (context[location].Piece == Piece.None)
            {
                foreach (var relatedSquares in _locationHelper.GetLocationsGroups(location))
                {
                    var enemySquares = relatedSquares.TakeWhile(l => context[l].Piece == context.EnemyPiece);
                    if (enemySquares.Any() &&
                        enemySquares.Count() < relatedSquares.Count() &&
                        context[relatedSquares.ElementAt(enemySquares.Count())].Piece == context.CurrentPiece)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public IEnumerable<int> FindAllValidMoves(IGameContext context)
        {
            return Enumerable.Range(0, 64).Where(
                loc => context[loc].Piece == Piece.None && IsValidMove(context, loc));
        }

        public bool IsAnyMoveValid(IGameContext context)
        {
            return Enumerable.Range(0, 64).Any(
                loc => context[loc].Piece == Piece.None && IsValidMove(context, loc));
        }
    }
}

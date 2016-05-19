using Reversi.Engine.Core;
using Reversi.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.Engine.Strategy.Minimax
{
    public class MoveOrdering : IMoveOrdering
    {
        private readonly int[] _corners;
        private readonly int[] _otherMoves;

        public MoveOrdering()
        {
            _corners = new[] { 0, 7, 56, 63 };
            _otherMoves = Enumerable.Range(0, 63).Except(_corners).ToArray();

        }

        /// <summary>
        /// Orders moves so that those with a better chance of a favourable outcome
        /// are returned first
        /// </summary>
        public IEnumerable<int> OrderMoves(IGameContext context, IEnumerable<int> moves)
        {
            if (moves.Count() == 0)
            {
                yield return -1;
            }
            else if (moves.Count() == 1)
            {
                yield return moves.First();
            }

            foreach (var preference in GetPreferredOrdering(context))
            {
                foreach (int move in moves.Intersect(preference))
                {
                    yield return move;
                }
            }
        }

        private List<IEnumerable<int>> GetPreferredOrdering(IGameContext context)
        {
            var badMovesNearCorners = new List<int>();

            if (context[0].Piece == Piece.None)
            {
                badMovesNearCorners.AddRange(new[] { 1, 8, 9 });
            }
            if (context[7].Piece == Piece.None)
            {
                badMovesNearCorners.AddRange(new[] { 6, 14, 15 });
            }
            if (context[56].Piece == Piece.None)
            {
                badMovesNearCorners.AddRange(new[] { 48, 49, 57 });
            }
            if (context[63].Piece == Piece.None)
            {
                badMovesNearCorners.AddRange(new[] { 54, 55, 62 });
            }

            var ordering = new List<IEnumerable<int>>();
            ordering.Add(_corners);
            ordering.Add(_otherMoves.Except(badMovesNearCorners));
            ordering.Add(badMovesNearCorners);

            return ordering;
        }
    }
}

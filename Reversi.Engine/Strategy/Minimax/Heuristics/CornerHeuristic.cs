using Reversi.Engine.Core;
using Reversi.Engine.Interfaces;
using Reversi.Engine.Strategy.Minimax.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.Engine.Strategy.Minimax.Heuristics
{
    public class CornerHeuristic : IHeuristic
    {
        public int GetScore(IGameContext context, Piece relativePiece)
        {
            var c1 = CheckCorner(0, new[] { 1, 8, 9 }, context, relativePiece);
            var c2 = CheckCorner(7, new[] { 6, 14, 15 }, context, relativePiece);
            var c3 = CheckCorner(56, new[] { 48, 49, 57 }, context, relativePiece);
            var c4 = CheckCorner(63, new[] { 54, 55, 62 }, context, relativePiece);
            var cornerScore = c1 + c2 + c3 + c4;

            return cornerScore;
        }

        /// <summary>
        /// If the corner is won then score +25 
        /// If the corner is empty then score -8 points for each occupied adjacent square
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cornerLoc">Location of corner</param>
        /// <param name="adjacentLocs">Location of squares adjacent to corner</param>
        /// <returns></returns>
        private int CheckCorner(int cornerLoc, int[] adjacentLocs, 
            IGameContext context, Piece relativePiece)
        {
            double score = 0.0;
            var cornerPiece = context[cornerLoc].Piece;

            if (cornerPiece != Piece.None)
            {
                score = ValueTheCornerSquare(relativePiece, cornerPiece);
            }
            else
            {
                score = ValueTheAdjacentSquares(adjacentLocs, context, relativePiece, score);
            }
            score *= 0.25; // 4 corners, so weight each by 0.25
            score = Math.Round(score);
            return (int)score;
        }

        private double ValueTheCornerSquare(Piece relativePiece, Piece cornerPiece)
        {
            return 100.0 * PieceValue(cornerPiece, relativePiece);
        }

        private double ValueTheAdjacentSquares(int[] adjacentLocs, IGameContext context, Piece relativePiece, double score)
        {
            // i.e. the corner is empty
            var total = 0.0;
            for (int i = 0; i < 3; i++)
            {
                var adjacentPiece = context[adjacentLocs[i]].Piece;
                total += (-33.33 * PieceValue(adjacentPiece, relativePiece));
            }
            return total;
        }

        private double PieceValue(Piece piece, Piece relativePiece)
        {
            if (piece == Piece.None) { return 0.0; }
            if (piece == relativePiece) { return 1.0; }
            return -1.0;
        }
    }
}

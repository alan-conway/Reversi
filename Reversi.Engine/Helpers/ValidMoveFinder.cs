using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reversi.Engine.Interfaces;
using Reversi.Engine.Core;

namespace Reversi.Engine.Helpers
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
            return IsValidMove(context, location, context.CurrentPiece);
        }

        public bool IsValidMove(IGameContext context, int location, Piece relativePiece)
        {
            if (IsLocationEmpty(context, location))
            {
                return AreEnemyPiecesSandwiched(context, location, relativePiece);
            }
            return false;
        }

        private bool AreEnemyPiecesSandwiched(IGameContext context, int location, Piece relativePiece)
        {
            Piece current = relativePiece;
            Piece enemy = relativePiece == Piece.Black ? Piece.White : Piece.Black;

            foreach (var relatedSquares in _locationHelper.GetLocationsGroups(location))
            {
                if (!IsAdjacentToEnemyPiece(context, enemy, relatedSquares))
                {
                    continue;
                }
                if (IsSandwichingPieceFound(context, relatedSquares, current))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsLocationEmpty(IGameContext context, int location)
        {
            return context[location].Piece == Piece.None;
        }

        private static bool IsAdjacentToEnemyPiece(IGameContext context, Piece enemy, int[] relatedSquares)
        {
            return relatedSquares.Length != 0 && context[relatedSquares[0]].Piece == enemy;
        }

        private bool IsSandwichingPieceFound(IGameContext context, int[] relatedSquares, Piece currentPiece)
        {
            // to make a valid sandwich, a string of enemy pieces
            // must have a 'currentPiece' piece at the other end, without
            // an empty square being encountered
            for (int i = 1; i < relatedSquares.Length; i++)
            {
                if (context[relatedSquares[i]].Piece == Piece.None)
                {
                    break;
                }
                else if (context[relatedSquares[i]].Piece == currentPiece)
                {
                    return true;
                }
            }
            return false;
        }

        public IEnumerable<int> FindAllValidMoves(IGameContext context)
        {
            return FindAllValidMoves(context, context.CurrentPiece);
        }

        public IEnumerable<int> FindAllValidMoves(IGameContext context, Piece relativePiece)
        {
            return Enumerable.Range(0, 64).Where(
                loc => context[loc].Piece == Piece.None && IsValidMove(context, loc, relativePiece));
        }

        public bool IsAnyMoveValid(IGameContext context)
        {
            return Enumerable.Range(0, 64).Any(
                loc => context[loc].Piece == Piece.None && IsValidMove(context, loc));
        }


    }
}

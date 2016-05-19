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
            if (context[location].Piece == Piece.None)
            {
                Piece current = relativePiece;
                Piece enemy = relativePiece == Piece.Black ? Piece.White : Piece.Black;

                foreach (var relatedSquares in _locationHelper.GetLocationsGroups(location))
                {
                    if (relatedSquares.Length == 0 || context[relatedSquares[0]].Piece != enemy)
                    {
                        continue;
                    }
                    
                    for(int i = 1; i < relatedSquares.Length; i++)
                    {
                        if (context[relatedSquares[i]].Piece == Piece.None)
                        {
                            break;
                        } 
                        else if (context[relatedSquares[i]].Piece == current)
                        {
                            return true;
                        }
                    }
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

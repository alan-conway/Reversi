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
    /// Captures enemy pieces after a move has been played
    /// </summary>
    public class CaptureHelper : ICaptureHelper
    {
        private ILocationHelper _locationHelper;

        public CaptureHelper(ILocationHelper locationHelper)
        {
            _locationHelper = locationHelper;
        }

        /// <summary>
        /// Turn over any pieces that are now sandwiched between the relevant pieces
        /// </summary>
        /// <param name="moveLocation">The location of the move just played</param>
        public void CaptureEnemyPieces(IGameContext context, int moveLocation)
        {
            CaptureEnemyPieces(context, moveLocation, context.CurrentPiece, context.EnemyPiece);
        }

        private void CaptureEnemyPieces(IGameContext context, int moveLocation, 
            Piece playedPiece, Piece enemyPiece)
        {
            foreach (var locationGroup in _locationHelper.GetLocationsGroups(moveLocation))
            {
                CaptureEnemyPieces(context, moveLocation, playedPiece, enemyPiece, locationGroup);
            }
        }

        /// <summary>
        /// Look at pieces in every direction from the location played and, if 
        /// appropriate, capture any enemy pieces that are now sandwiched
        /// </summary>
        /// <param name="moveLocation"></param>
        /// <param name="piece"></param>
        /// <param name="locations"></param>
        internal void CaptureEnemyPieces(IGameContext context, int moveLocation, 
            Piece piece, Piece enemyPiece, IEnumerable<int> locations)
        {
            var enemyLocations = locations.TakeWhile(l => context[l].Piece == enemyPiece);
            if (AreEnemyPiecesSandwiched(context, piece, locations, enemyLocations))
            {
                CaptureThePieces(context, piece, enemyLocations);
            }
        }

        private static bool AreEnemyPiecesSandwiched(IGameContext context, Piece piece, IEnumerable<int> locations, IEnumerable<int> enemyLocations)
        {
            if (enemyLocations.Any() && enemyLocations.Count() < locations.Count())
            {
                int nextLocation = locations.ElementAt(enemyLocations.Count());
                bool sandwiched = context[nextLocation].Piece == piece;
                return sandwiched;
            }
            return false;
        }

        private static void CaptureThePieces(IGameContext context, Piece piece, IEnumerable<int> enemyLocations)
        {
            foreach (int enemyLoc in enemyLocations)
            {
                context.SetPiece(enemyLoc, piece);
            }
        }
    }
}

using System.Collections.Generic;

namespace Reversi.Engine.Interfaces
{
    /// <summary>
    /// When a move is played at the given location, this object will
    /// capture any enemy pieces that are trapped by that move
    /// </summary>
    public interface ICaptureHelper
    {
        void CaptureEnemyPieces(IGameContext context, int moveLocation);
    }
}
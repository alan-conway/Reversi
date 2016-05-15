using System.Collections.Generic;

namespace Reversi.Engine
{
    public interface ICaptureHelper
    {
        void CaptureEnemyPieces(IGameContext context, int moveLocation);
    }
}
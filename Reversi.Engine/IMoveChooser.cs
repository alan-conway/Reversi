using System.Collections.Generic;

namespace Reversi.Engine
{
    public interface IMoveChooser
    {
        /// <summary>
        /// Choose a move to play (hopefully the best one it can, depending on the 
        /// algorithm) 
        /// </summary>
        Move ChooseMove(IGameContext context, IValidMoveFinder validMoveFinder);
    }
}
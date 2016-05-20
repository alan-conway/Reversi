using System.Collections.Generic;
using Reversi.Engine.Interfaces;

namespace Reversi.Engine.Strategy.Minimax.Interfaces
{
    /// <summary>
    /// Orders moves so that those most likely to lead to a better outcome
    /// are explored first, for efficiency in pruning the game search tree
    /// </summary>
    public interface IMoveOrdering
    {
        IEnumerable<int> OrderMoves(IGameContext context, IEnumerable<int> moves);
    }
}
using System.Collections.Generic;
using Reversi.Engine.Interfaces;

namespace Reversi.Engine.Strategy.Minimax.Interfaces
{
    /// <summary>
    /// Builds a node in the game search tree, or 
    /// all the children nodes from a given board
    /// </summary>
    public interface IReversiTreeNodeBuilder
    {
        IReversiTreeNode CreateRootTreeNode(IGameContext context, IMovePlayer movePlayer);

        List<IReversiTreeNode> CreateNextTreeNodes(IGameContext context, IMovePlayer movePlayer);
    }
}
using System.Collections.Generic;
using Reversi.Engine.Interfaces;

namespace Reversi.Engine.Strategy.Minimax
{
    /// <summary>
    /// Builds a node in the game search tree, or 
    /// all the children nodes from a given board
    /// </summary>
    public interface IReversiTreeNodeBuilder
    {
        IReversiTreeNode CreateSingleTreeNode(int moveLocation, IGameContext context, IGameEngine engine);

        List<IReversiTreeNode> CreateNextTreeNodes(IGameContext context, IGameEngine engine);
    }
}
using System.Collections.Generic;
using Game.Search.Interfaces;
using Reversi.Engine.Interfaces;

namespace Reversi.Engine.Strategy.Minimax.Interfaces
{
    /// <summary>
    /// A node in the game search tree 
    /// </summary>
    public interface IReversiTreeNode : ITreeNode
    {
        /// <summary>
        /// The move that was made
        /// </summary>
        int MoveLocation { get; }

        /// <summary>
        /// The situation in the game
        /// </summary>
        IGameContext Context { get; }

        /// <summary>
        /// The depth of the node in the tree
        /// </summary>
        int Depth { get; set; }
        
    }
}
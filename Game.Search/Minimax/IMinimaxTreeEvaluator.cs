using System;
using Game.Search.Interfaces;

namespace Game.Search.Minimax
{
    /// <summary>
    /// Performs an evaluation of the game tree from the root node supplied.
    /// Uses Minimax with Alpha/Beta pruning
    /// </summary>
    public interface IMinimaxTreeEvaluator
    {
        /// <summary>
        /// Evaluates the tree from the node supplied
        /// </summary>
        /// <param name="rootNode">The node in the tree representing the state of the game</param>
        /// <param name="isLeafNode">A function that tells the search to stop looking for child nodes</param>
        /// <param name="scoreProvider">Provides a score for a leaf node</param>
        /// <param name="isPlayer1">The scoring perspective - set this to True if the score should be positive when player1 is ahead</param>
        /// <returns></returns>
        MinimaxResult EvaluateTree(ITreeNode rootNode, 
            Func<ITreeNode, bool> isLeafNode, IScoreProvider scoreProvider, 
            bool isPlayer1);
    }
}
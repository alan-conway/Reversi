namespace Game.Search.Interfaces
{
    /// <summary>
    /// Provides a score for a node in the game tree
    /// </summary>
    /// <remarks>
    /// Expected to be implemented by the caller from outside of this project
    /// </remarks>
    public interface IScoreProvider
    {
        /// <summary>
        /// Evaluates the game represented by the tree node supplied
        /// </summary>
        /// <param name="isPlayer1">The score is from the perspective of player1 is this is True</param>
        int EvaluateScore(ITreeNode treeNode, bool isPlayer1);
    }
}
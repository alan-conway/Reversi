using Game.Search.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Search.MonteCarlo
{
    public class MonteCarloResult
    {
        public MonteCarloResult(int numGamesPlayed, int numWinsForNode, ITreeNode node)
        {
            NumGamesPlayed = numGamesPlayed;
            NumWinsForNode = numWinsForNode;
            TreeNode = node;
        }

        /// <summary>
        /// The number of games played in total on the tree
        /// </summary>
        public int NumGamesPlayed { get; }

        /// <summary>
        /// The number of wins that came from playing in the suggested node
        /// </summary>
        public int NumWinsForNode { get; }

        /// <summary>
        /// The node with the best score
        /// </summary>
        public ITreeNode TreeNode { get; }
    }
}

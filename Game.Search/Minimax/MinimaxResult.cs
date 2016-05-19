using Game.Search.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Search.Minimax
{
    public class MinimaxResult
    {
        public MinimaxResult(double score, ITreeNode node)
        {
            Score = score;
            TreeNode = node;
        }

        /// <summary>
        /// The score attributed to the node evaluated
        /// </summary>
        public double Score { get; }

        /// <summary>
        /// The node with the best score
        /// </summary>
        public ITreeNode TreeNode { get; }
    }
}

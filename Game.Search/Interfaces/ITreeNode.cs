using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Search.Interfaces
{
    /// <summary>
    /// A node in the game search tree which represents the status of the game 
    /// at that point
    /// </summary>
    /// <remarks>
    /// Expected to be implemented by the caller from outside of this project
    /// </remarks>
    public interface ITreeNode
    {
        /// <summary>
        /// Returns the games further down in the tree, in which another ply has 
        /// been made
        /// </summary>
        IEnumerable<ITreeNode> GetChildren();
    }
}

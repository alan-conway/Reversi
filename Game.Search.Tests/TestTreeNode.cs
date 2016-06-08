using Game.Search.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Search.Tests
{
    public class TestTreeNode : ITreeNode
    {
        private List<TestTreeNode> _children;

        public TestTreeNode(int id)
        {
            Id = id;
            _children = new List<TestTreeNode>();
        }

        public int Id { get; }
        public int Depth { get; private set; }

        public IEnumerable<ITreeNode> GetChildren()
        {
            return _children;
        }

        public void AddChild(TestTreeNode child)
        {
            _children.Add(child);
            child.Depth = Depth + 1;
        }

        public static TestTreeNode CreateNode(int id)
        {
            return new TestTreeNode(id);
        }
    }
}

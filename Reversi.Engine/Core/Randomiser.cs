using Reversi.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.Engine.Core
{
    public class Randomiser : IRandomiser
    {
        private Random _random;

        public Randomiser()
        {
            _random = new Random();
        }
        
        public int Next(int min, int max)
        {
            return _random.Next(min, max);
        }
    }
}

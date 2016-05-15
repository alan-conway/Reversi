﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.Engine
{
    public class Move
    {
        private static Move _pass = new Move(-1) { Pass = true };

        public static Move PassMove
        {
            get { return _pass; }
        }

        public Move(int cellId)
        {
            LocationPlayed = cellId;
        }

        public int LocationPlayed { get; set; }

        public bool Pass { get; private set; } = false;
        
    }
}

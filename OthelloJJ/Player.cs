using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OthelloJJ
{
    class Player
    {
        private readonly string symbol;
        public string Symbol
        {
            get { return symbol; }
        }
        private readonly int val;
        public int Val
        {
            get { return val; }
        }

        public Player(string symbol, int value)
        {
            this.symbol = symbol;
            this.val = value;
        }
    }
}

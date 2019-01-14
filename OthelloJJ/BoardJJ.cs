using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OthelloJJ
{
    class BoardJJ : IPlayable.IPlayable
    {
        private string name;
        private int[,] game;

        public int GetBlackScore()
        {
            return 0;
        }

        public int[,] GetBoard()
        {
            return game;
        }

        public string GetName()
        {
            return name;
        }

        public Tuple<int, int> GetNextMove(int[,] game, int level, bool whiteTurn)
        {
            return new Tuple<int, int>(-1, -1);
        }

        public int GetWhiteScore()
        {
            return 0;
        }

        public bool IsPlayable(int column, int line, bool isWhite)
        {
            return true;
        }

        public bool PlayMove(int column, int line, bool isWhite)
        {
            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OthelloJJ
{
    [Serializable]
    class BoardJJ : IPlayable.IPlayable
    {
        private string name;
        private int[,] game;
        private readonly int[,] possibleMove = { { -1, -1 }, { 1, 1 }, { -1, 1 }, { 1, -1 }, { 0, -1 }, { 0, 1 }, { 1, 0 }, { -1, 0 } };
        private int pWhite = 0;
        private int pBlack = 1;
        private int pEmpty = -1;
        private int actualVal = 0;
        private int width = 0;
        private int height = 0;

        public BoardJJ()
        {
            this.name = "BoardJJIA";
        }

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
            width = game.GetLength(0);
            height = game.GetLength(1);
            this.game = game;
            actualVal = (whiteTurn) ? pWhite : pBlack;
            for (int x = 0; x < width; ++x)
            {
                for(int y = 0; y<height; ++y)
                {
                    if (game[x, y] == -1 || game[x, y] == -2) // -2 = possible move in interface
                    {
                        if(IsPlayable(x, y, whiteTurn))
                        {
                            return new Tuple<int, int>(x, y);
                        }
                    }
                }
            }
            return new Tuple<int, int>(-1, -1);
        }

        public int GetWhiteScore()
        {
            return 0;
        }

        public bool IsPlayable(int column, int line, bool isWhite)
        {
            for(int i = 0; i<possibleMove.Length/2;++i)
            {
                if(isWayValid(column,line, possibleMove[i,0], possibleMove[i,1]))
                {
                    return true;
                }
            }
            return false;
        }

        public bool PlayMove(int column, int line, bool isWhite)
        {
            return true;
        }

        private bool isWayValid(int x, int y, int vx, int vy)
        {
            x += vx;
            y += vy;
            if(isInsideBoard(x,y) && game[x,y]!=1-actualVal)
            {
                return false;
            }
            while(isInsideBoard(x,y) && game[x,y] == 1-actualVal)
            {
                x += vx;
                y += vy;
            }
            return isInsideBoard(x, y) && game[x, y] == actualVal;
        }

        private bool isInsideBoard(int x, int y)
        {
            return x < width && x >= 0 && y < height && y >= 0;
        }
    }
}

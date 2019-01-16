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
        private readonly string name;
        private int[,] game;
        private readonly int[,] possibleMove = { { -1, -1 }, { 1, 1 }, { -1, 1 }, { 1, -1 }, { 0, -1 }, { 0, 1 }, { 1, 0 }, { -1, 0 } };
        private readonly int pWhite = 0;
        private readonly int pBlack = 1;
        private int actualVal;
        private int width;
        private int height;

        /// <summary>
        /// Default constructor
        /// </summary>
        public BoardJJ()
        { 
            this.name = "BoardJJ_IA";
        }

        /// <summary>
        /// Count number of black point in game
        /// </summary>
        /// <returns>black score</returns>
        public int GetBlackScore()
        {            
            return CptValue(pBlack);
        }

        /// <summary>
        /// Return the game
        /// </summary>
        /// <returns>game</returns>
        public int[,] GetBoard()
        {
            return game;
        }

        /// <summary>
        /// Return the name
        /// </summary>
        /// <returns>name</returns>
        public string GetName()
        {
            return name;
        }

        /// <summary>
        /// Return the next possible move
        /// In this IA it  return the first valid move
        /// It only serves to demonstrate that our UI work with an IA
        /// </summary>
        /// <param name="game"></param>
        /// <param name="level"></param>
        /// <param name="whiteTurn"></param>
        /// <returns>Tuple next move</returns>
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

        /// <summary>
        /// Count number of black point in game
        /// </summary>
        /// <returns>White score</returns>
        public int GetWhiteScore()
        {
            return CptValue(pWhite);
        }

        /// <summary>
        /// Return true if it's a valid point
        /// </summary>
        /// <param name="column"></param>
        /// <param name="line"></param>
        /// <param name="isWhite"></param>
        /// <returns>bool</returns>
        public bool IsPlayable(int column, int line, bool isWhite)
        {
            actualVal = (isWhite) ? pWhite : pBlack;
            for (int i = 0; i<possibleMove.Length/2;++i)
            {
                if(IsWayValid(column,line, possibleMove[i,0], possibleMove[i,1]))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Play the move if point is valid
        /// </summary>
        /// <param name="column"></param>
        /// <param name="line"></param>
        /// <param name="isWhite"></param>
        /// <returns>bool</returns>
        public bool PlayMove(int column, int line, bool isWhite)
        {
            if(IsPlayable(column, line, isWhite))
            {
                game[column, line] = actualVal;
                for(int i=0;i<possibleMove.Length/2;++i)
                {
                    var xTemp = column + possibleMove[i, 0];
                    var yTemp = line + possibleMove[i, 1];
                    var listVisited = new List<Tuple<int, int>>();
                    while(IsInsideBoard(xTemp, yTemp) && game[xTemp, yTemp] == 1-actualVal)
                    {
                        listVisited.Add(new Tuple<int, int>(xTemp, yTemp));
                        xTemp += possibleMove[i, 0];
                        yTemp += possibleMove[i, 1];
                    }
                    if(IsInsideBoard(xTemp, yTemp) && game[xTemp, yTemp] == actualVal)
                    {
                        foreach(var tuplePos in listVisited)
                        {
                            game[tuplePos.Item1, tuplePos.Item2] = actualVal;
                        }
                    }
                }
                return true;
            }
            return false;
        }

        private bool IsWayValid(int x, int y, int vx, int vy)
        {
            x += vx;
            y += vy;
            if(IsInsideBoard(x,y) && game[x,y]!=1-actualVal)
            {
                return false;
            }
            while(IsInsideBoard(x,y) && game[x,y] == 1-actualVal)
            {
                x += vx;
                y += vy;
            }
            return IsInsideBoard(x, y) && game[x, y] == actualVal;
        }

        private bool IsInsideBoard(int x, int y)
        {
            return x < width && x >= 0 && y < height && y >= 0;
        }

        private int CptValue(int v)
        {
            // Return number of v in game
            int iCpt = 0;
            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; j++)
                {
                    if (game[i, j] == v)
                    {
                        iCpt++;
                    }
                }
            }
            return iCpt;
        }
    }
}

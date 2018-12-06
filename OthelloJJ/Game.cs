using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace OthelloJJ
{
    class Game
    {
        public static int WIDTH = 9;
        public static int HEIGHT = 7;
        private Player[,] board = new Player[WIDTH, HEIGHT];

        private Player player1;
        private Player player2;
        private Player emptyPlayer;
        private Player possibleMovePlayer;

        private int round;
        private readonly int[,] possibleMove = { { -1, -1 }, {1,1 }, { -1, 1 }, { 1, -1 }, { 0, -1 }, { 0, 1 }, { 1, 0 }, { -1, 0 } };

       private struct Position
        {
            private int x;
            private int y;
        }

        private struct Cell
        {
            private int state;
            private List possibleMove;
        }

        public Game()
        {
            player1 = new Player("O", 1);
            player2 = new Player("X", 2);
            emptyPlayer = new Player("", 0);
            possibleMovePlayer = new Player("U", -1);
            round = 0;

            for (int i = 0; i < WIDTH; ++i)
            {
                for (int j = 0; j < HEIGHT; j++)
                {
                    if (i == 3 && j == 4 || i == 4 && j == 3)
                    {
                        board[i, j] = player1;
                    }
                    else if (i == 4 && j == 4 || i == 3 && j == 3)
                    {
                        board[i, j] = player2;
                    }
                    else
                    {
                        board[i, j] = emptyPlayer;
                    }
                }
            }
            Update();
        }

        public void Update()
        {
            round++;
            MainWindow.mainWindow.grid.Children.Clear();
            UpdatePossibleMove();
            Draw();
        }

        private void Draw()
        {
            for(int i=0; i < WIDTH; ++i)
            {
                for(int j=0;j<HEIGHT; j++)
                {
                    AddElement(board[i, j].Symbol, i, j);
                }
            }
        }

        private void AddElement(String text, int x, int y)
        {
            TextBlock txt0 = new TextBlock();
            txt0.Text = text;
            txt0.FontSize = 30;
            Grid.SetColumn(txt0, x);
            Grid.SetRow(txt0, y);
            MainWindow.mainWindow.grid.Children.Add(txt0);
        }

        private void UpdatePossibleMove()
        {
            for (int i = 0; i < WIDTH; ++i)
            {
                for (int j = 0; j < HEIGHT; j++)
                {
                    if(board[i,j] == possibleMovePlayer)
                    {
                        board[i, j] = emptyPlayer;
                    }
                    if (board[i, j] == emptyPlayer)
                    {
                        if(IsCellValid(i, j))
                        {
                            board[i, j] = possibleMovePlayer;
                        }
                    }
                }
            }
        }

        public void CellSelected(int x, int y)
        {
            if (board[x, y] == possibleMovePlayer)
            {
                board[x, y] = ActualPlayer();                
                Update();
            }
        }

        private bool IsCellValid(int x, int y)
        {
            for(int i = 0; i<possibleMove.Length/2;++i)
            {
                if(IsWayValid(x,y,possibleMove[i,0], possibleMove[i,1]))
                {
                    return true;
                }
            }
            return false;
        }


        private bool IsWayValid(int x, int y, int vx, int vy)
        {
            x += vx;
            y += vy;
            if (IsInsideBoard(x,y) && board[x,y] != OpponentPlayer())
{
                    return false;
            }
            while (IsInsideBoard(x,y) && board[x,y] == OpponentPlayer())
            {
                x += vx;
                y += vy;
            }
            return IsInsideBoard(x, y) && board[x, y] == ActualPlayer();
        }

        private Player ActualPlayer()
        {
            if (round % 2 == 0)
            {
                return player1;
            }
            return player2;
        }

        private Player OpponentPlayer()
        {
            if (round % 2 == 1)
            {
                return player1;
            }
            return player2;
        }

        private bool IsInsideBoard(int x, int y)
        {
            return x < WIDTH && x >= 0 && y < HEIGHT && y >= 0;
        }
    }
}

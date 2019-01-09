using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace OthelloJJ
{
    class Game
    {
        public static int WIDTH = 9;
        public static int HEIGHT = 7;
        private int ScoreChrome { get; set; }
        private int ScoreMozilla { get; set; }
        private Cell[,] board = new Cell[WIDTH, HEIGHT];

        private Player player1;
        private Player player2;
        private int emptyState = 0;
        private Player possibleMovePlayer;

        private int round;
        private readonly int[,] possibleMove = { { -1, -1 }, {1,1 }, { -1, 1 }, { 1, -1 }, { 0, -1 }, { 0, 1 }, { 1, 0 }, { -1, 0 } };

       private struct Position
        {
            public int X {get;set;}
            public int Y { get; set; }
            public Position(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }

        private class Cell
        {
            public int State { get; set; }
            public List<Position> possibleMove { get; }
            public Cell(int state)
            {
                this.State = state;
                possibleMove = new List<Position>();
            }
            
            public void addPossibleMove(Position p)
            {
                possibleMove.Add(p);
            }
        }

        public Game()
        {
            ScoreChrome = 0;
            ScoreMozilla = 0;
            player1 = new Player(ImageSourceForBitmap(Properties.Resources.chrome), 1);
            player2 = new Player(ImageSourceForBitmap(Properties.Resources.firefox), 2);
            possibleMovePlayer = new Player(ImageSourceForBitmap(Properties.Resources.possibleMove), -1);
            round = 0;

            for (int i = 0; i < WIDTH; ++i)
            {
                for (int j = 0; j < HEIGHT; j++)
                {
                    if (i == 3 && j == 4 || i == 4 && j == 3)
                    {
                        board[i, j] = new Cell(player1.Val);
                    }
                    else if (i == 4 && j == 4 || i == 3 && j == 3)
                    {
                        board[i, j] = new Cell(player2.Val);
                    }
                    else
                    {
                        board[i, j] = new Cell(emptyState);
                    }
                }
            }
            Update();
        }

        public void Update()
        {
            round++;
            MainWindow.mainWindow.gameGrid.Children.Clear();
            UpdatePossibleMove();
            Draw();
        }

        private void Draw()
        {
            ScoreMozilla = 0;
            ScoreChrome = 0;
            for(int i=0; i < WIDTH; ++i)
            {
                for(int j=0;j<HEIGHT; j++)
                {
                    switch(board[i,j].State)
                    {
                        case -1:
                            AddElement(possibleMovePlayer.Image, i, j);
                            break;
                        case 1:
                            ScoreChrome++;
                            AddElement(player1.Image, i, j);
                            break;
                        case 2:
                            ScoreMozilla++;
                            AddElement(player2.Image, i, j);
                            break;
                    }
                }
            }
            DrawScore();
        }

        private void DrawScore()
        {
            MainWindow.mainWindow.scoreChrome.Content = ScoreChrome;
            MainWindow.mainWindow.scoreMozilla.Content = ScoreMozilla;
        }
        private void AddElement(ImageSource image, int x, int y)
        {
            Image img = new Image { Source = image };
            Grid.SetColumn(img, x);
            Grid.SetRow(img, y);
            MainWindow.mainWindow.gameGrid.Children.Add(img);
        }

        private void UpdatePossibleMove()
        {
            for (int i = 0; i < WIDTH; ++i)
            {
                for (int j = 0; j < HEIGHT; j++)
                {
                    if(board[i,j].State == possibleMovePlayer.Val)
                    {
                        board[i, j].State = emptyState;
                    }
                    if (board[i, j].State == emptyState)
                    {
                        if(IsCellValid(i, j))
                        {
                            board[i, j].State = possibleMovePlayer.Val;
                        }
                    }
                }
            }
        }

        public void CellSelected(int x, int y)
        {
            if (board[x, y].State == possibleMovePlayer.Val)
            {
                board[x, y].State = ActualPlayer().Val;
                changeCells(x,y);
                Update();
            }
        }

        private void changeCells(int x, int y)
        {
            for (int i = 0; i < possibleMove.Length / 2; ++i)
            {
                int xTemp = x + possibleMove[i, 0];
                int yTemp = y + possibleMove[i, 1];
                var listVisited = new List<Tuple<int, int>>();
                while(IsInsideBoard(xTemp,yTemp) && board[xTemp, yTemp].State == OpponentPlayer().Val)
                {
                    listVisited.Add(new Tuple<int, int>(xTemp, yTemp));
                    xTemp += possibleMove[i, 0];
                    yTemp += possibleMove[i, 1];
                }
                if(IsInsideBoard(xTemp, yTemp) && board[xTemp, yTemp].State == ActualPlayer().Val)
                {
                    foreach(var tuplePos in listVisited)
                    {
                        board[tuplePos.Item1, tuplePos.Item2].State = ActualPlayer().Val;
                    }
                }
                
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
            if (IsInsideBoard(x,y) && board[x,y].State != OpponentPlayer().Val)
            {
                return false;
            }
            while (IsInsideBoard(x,y) && board[x,y].State == OpponentPlayer().Val)
            {
                x += vx;
                y += vy;
            }
            return IsInsideBoard(x, y) && board[x, y].State == ActualPlayer().Val;
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



        //Source : https://stackoverflow.com/a/51227400/7570047
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        public ImageSource ImageSourceForBitmap(System.Drawing.Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                ImageSource newSource = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

                DeleteObject(handle);
                return newSource;
            }
            catch (Exception ex)
            {
                DeleteObject(handle);
                return null;
            }
        }
    }
}

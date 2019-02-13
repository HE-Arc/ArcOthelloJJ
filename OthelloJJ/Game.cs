using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace OthelloJJ
{
    [Serializable]
    class Game : INotifyPropertyChanged
    {
        /// <summary>
        /// Class that manage logic of Othello game
        /// </summary>
        #region attributes
        public static int WIDTH = 9;
        public static int HEIGHT = 7;

        private static DispatcherTimer timer;

        private int[,] board;

        private Data player1;
        private Data player2;
        private Data possibleMovePlayer;
        private readonly int emptyState = -1;

        private bool isGameRunning;

        private int round;
        private readonly int[,] possibleMove = { { -1, -1 }, {1,1 }, { -1, 1 }, { 1, -1 }, { 0, -1 }, { 0, 1 }, { 1, 0 }, { -1, 0 } };

        private bool isLastPlayed;
        private int scoreChrome;
        private int scoreMozilla;
        #endregion

        #region properties
        public int ScoreChrome
        {
            get
            {
                return scoreChrome;
            }

            set
            {
                scoreChrome = value;
                OnPropertyChanged("ScoreChrome");
            }
        }

        public int ScoreMozilla
        {
            get
            {
                return scoreMozilla;
            }

            set
            {
                scoreMozilla = value;
                OnPropertyChanged("ScoreMozilla");
            }
        }

        public bool IsMouseOnGrid { get; set; }
        #endregion

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        [Serializable]
        private class Data
        {
            /// <summary>
            /// Small class that match a picture, a value, time and maybe an IA for a player
            /// </summary>
            public int Val { get; }
            [NonSerialized]
            public ImageSource img;
            public TimeSpan Time { get; set; }
            public IPlayable.IPlayable IA { get; set; }
            public Data(ImageSource img , int val, TimeSpan time, IPlayable.IPlayable ia = null)
            {
                this.Val = val;
                this.img = img;
                this.Time = time;
                this.IA = ia;
            }
        }

        #region constructor
        public Game()
        {
            board = new int[WIDTH, HEIGHT];
            ScoreChrome = 0;
            ScoreMozilla = 0;
            isLastPlayed = false;
            IsMouseOnGrid = false;
            player1 = new Data(ImageSourceForBitmap(Properties.Resources.chrome), 0,new TimeSpan(0,0,0));
            player2 = new Data(ImageSourceForBitmap(Properties.Resources.firefox), 1,new TimeSpan(0,0,0));
            player2.IA = new BoardJJ();
            possibleMovePlayer = new Data(ImageSourceForBitmap(Properties.Resources.possibleMove), -2,new TimeSpan(0,0,0));

            SetTimer();
            InitVars();
            Update();
        }
        #endregion

        #region public methods
        /// <summary>
        /// When a cell is selected from UI or from IA
        /// If it's -1 -1 IA can't play
        /// If inside board and valid, change value of cells
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void CellSelected(int x, int y)
        {
            if (x == -1 && y == -1)
            {
                isLastPlayed = false;
                Update();
            }
            else if (IsInsideBoard(x,y) && board[x, y] == possibleMovePlayer.Val)
            {
                if (!timer.IsEnabled)
                {
                    timer.Start();
                }
                board[x, y] = ActualPlayer().Val;
                ChangeCells(x, y);
                Update();
            }
        }

        /// <summary>
        /// Clean vars to restart a new game
        /// </summary>
        public void Clean()
        {
            timer.Stop();
            player1.Time = new TimeSpan(0, 0, 0);
            player2.Time = new TimeSpan(0, 0, 0);
            DrawTime();
            board = new int[WIDTH, HEIGHT];
            InitVars();
            Update();
        }

        /// <summary>
        /// Create 0, 1 or 2 IA
        /// </summary>
        /// <param name="nb"></param>
        public void SetIA(int nb)
        {
            if (nb == 0)
            {
                player2.IA = null;
                player1.IA = null;
            }
            else if (nb == 1)
            {
                player1.IA = new BoardJJ();
                player2.IA = null;
            }
            else
            {
                player1.IA = new BoardJJ();
                player2.IA = new Bidon();
            }
        }

        
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        /// <summary>
        /// Convert a bitmap to an ImageSource
        /// Source : https://stackoverflow.com/a/51227400/7570047
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public ImageSource ImageSourceForBitmap(System.Drawing.Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                var newSource = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                DeleteObject(handle);
                return newSource;
            }
            catch (Exception)
            {
                DeleteObject(handle);
                return null;
            }
        }
        /// <summary>
        /// Method that draw the grid content
        /// </summary>
        public void Draw()
        {

            ScoreMozilla = 0;
            ScoreChrome = 0;
            for (int i = 0; i < WIDTH; ++i)
            {
                for (int j = 0; j < HEIGHT; j++)
                {
                    if (board[i, j] == possibleMovePlayer.Val && IsMouseOnGrid)
                    {
                        AddElement(possibleMovePlayer.img, i, j);
                    }
                    else if (board[i, j] == player1.Val)
                    {
                        ScoreChrome++;
                        AddElement(player1.img, i, j);
                    }
                    else if (board[i, j] == player2.Val)
                    {
                        ScoreMozilla++;
                        AddElement(player2.img, i, j);
                    }
                }
            }
            DrawCurrentPlayer();
        }
        #endregion

        #region protected method
        protected virtual void OnPropertyChanged(String property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        #endregion

        #region private methods
        private void Update()
        {
            if(isGameRunning)
            {
                round++;
                MainWindow.mainWindow.gameGrid.Children.Clear();
                UpdatePossibleMove();
                Draw();
                if (ActualPlayer().IA != null)
                {
                    var move = ActualPlayer().IA.GetNextMove(board, 5, ActualPlayer().Val == player1.Val);
                    CellSelected(move.Item1, move.Item2);
                }
            }  
        }
        private void SetTimer()
        {
            // Create a timer with a one second interval.
            timer = new DispatcherTimer();
            // Hook up the Elapsed event for the timer. 
            timer.Tick += OnTimedEvent;
            timer.Interval = new TimeSpan(0, 0, 1);
        }
        private  void OnTimedEvent(Object sender, EventArgs e)
        {
            ActualPlayer().Time = ActualPlayer().Time.Add(new TimeSpan(0, 0, 1));
            DrawTime();
        }
        private void InitVars()
        {
            round = 0;
            isGameRunning = true;
            for (int i = 0; i < WIDTH; ++i)
            {
                for (int j = 0; j < HEIGHT; j++)
                {
                    if (i == 3 && j == 4 || i == 4 && j == 3)
                    {
                        board[i, j] = player1.Val;
                    }
                    else if (i == 4 && j == 4 || i == 3 && j == 3)
                    {
                        board[i, j] = player2.Val;
                    }
                    else
                    {
                        board[i, j] = emptyState;
                    }
                }
            }
        }
        
        private void DrawCurrentPlayer(){
            if(ActualPlayer().Val==player1.Val)
            {
                MainWindow.mainWindow.ImageCurrentPlayer.Source = player1.img;
            }
            else
            {
                MainWindow.mainWindow.ImageCurrentPlayer.Source = player2.img;
            }

        }
        private void DrawTime()
        {
            MainWindow.mainWindow.TimeChrome.Content = player1.Time.ToString("mm':'ss");
            MainWindow.mainWindow.TimeMozilla.Content = player2.Time.ToString("mm':'ss");

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
            var canPlay = false;
            for (int i = 0; i < WIDTH; ++i)
            {
                for (int j = 0; j < HEIGHT; j++)
                {
                    if(board[i,j] == possibleMovePlayer.Val)
                    {
                        board[i, j] = emptyState;
                    }
                    if (board[i, j] == emptyState)
                    {
                        if(IsCellValid(i, j))
                        {
                            canPlay = true;
                            board[i, j] = possibleMovePlayer.Val;
                        }
                    }
                }
            }
            if (canPlay == false && isLastPlayed == false)
            {
                Draw();
                string msg;
                if (ScoreChrome > ScoreMozilla)
                {
                    msg = $"Partie terminé, Chrome à gagné avec : {ScoreChrome} points\nFirefox a eu {ScoreMozilla}";
                }
                else if(ScoreChrome<ScoreMozilla)
                {
                    msg = $"Partie terminé, Firefox à gagné avec : {ScoreMozilla} points\nChrome a eu {ScoreChrome}";
                }
                else
                {
                    msg = $"Partie terminé, Chrome et Firefox sont à égalités avec : {ScoreChrome} points\n";
                }
                isGameRunning = false;
                MessageBox.Show(msg);
            }
            else if (canPlay == false)
            {
                isLastPlayed = false;
                timer.Stop();
                Update();
            }
            else
            {
                isLastPlayed = true;
            }
        }

        private void ChangeCells(int x, int y)
        {
            for (int i = 0; i < possibleMove.Length / 2; ++i)
            {
                var xTemp = x + possibleMove[i, 0];
                var yTemp = y + possibleMove[i, 1];
                var listVisited = new List<Tuple<int, int>>();
                while(IsInsideBoard(xTemp,yTemp) && board[xTemp, yTemp] == OpponentPlayer().Val)
                {
                    listVisited.Add(new Tuple<int, int>(xTemp, yTemp));
                    xTemp += possibleMove[i, 0];
                    yTemp += possibleMove[i, 1];
                }
                if(IsInsideBoard(xTemp, yTemp) && board[xTemp, yTemp] == ActualPlayer().Val)
                {
                    foreach(var tuplePos in listVisited)
                    {
                        board[tuplePos.Item1, tuplePos.Item2] = ActualPlayer().Val;
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
            if (IsInsideBoard(x,y) && board[x,y] != OpponentPlayer().Val)
            {
                return false;
            }
            while (IsInsideBoard(x,y) && board[x,y] == OpponentPlayer().Val)
            {
                x += vx;
                y += vy;
            }
            return IsInsideBoard(x, y) && board[x, y] == ActualPlayer().Val;
        }

        private Data ActualPlayer()
        {
            if (round % 2 == 0)
            {
                return player1;
            }
            return  player2;
        }

        private   Data OpponentPlayer()
        {
            if (round % 2 == 1)
            {
                return  player1;
            }
            return  player2;
        }

        private bool IsInsideBoard(int x, int y)
        {
            return x < WIDTH && x >= 0 && y < HEIGHT && y >= 0;
        }
#endregion

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            player1.img = ImageSourceForBitmap(Properties.Resources.chrome);
            player2.img = ImageSourceForBitmap(Properties.Resources.firefox);
            possibleMovePlayer.img = ImageSourceForBitmap(Properties.Resources.possibleMove);

            Draw();
            DrawTime();
            timer.Tick -= OnTimedEvent;
            timer.Tick += OnTimedEvent;
            timer.Start();

        }
    }

}

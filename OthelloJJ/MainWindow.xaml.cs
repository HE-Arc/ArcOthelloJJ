using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OthelloJJ
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static int WIDTH = 9;
        private static int HEIGHT = 7;
        private int[,] board = new int[WIDTH, HEIGHT];
        private ImageSource imgMozilla;
        private ImageSource imgChorme;

        private Game game;
        public static MainWindow mainWindow;

        public MainWindow()
        {
            InitializeComponent();
            mainWindow = this;
            game = new Game();
            imgChorme = game.ImageSourceForBitmap(Properties.Resources.chrome);
            imgMozilla = game.ImageSourceForBitmap(Properties.Resources.firefox);
            ImageChrome.Source = imgChorme;
            ImageMozilla.Source = imgMozilla;
            //Create column, row
            for (int i = 0; i < Game.HEIGHT; ++i)
            {
                gameGrid.RowDefinitions.Add(new RowDefinition());
            }
            for (int i = 0; i < Game.WIDTH; ++i)
            {
                gameGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            
        }

        private void Grid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Find indices of selected cell
            var point = Mouse.GetPosition(gameGrid);

            int row = 0;
            int col = 0;
            double accumulatedHeight = 0.0;
            double accumulatedWidth = 0.0;

            // calc row mouse was over
            foreach (var rowDefinition in gameGrid.RowDefinitions)
            {
                accumulatedHeight += rowDefinition.ActualHeight;
                if (accumulatedHeight >= point.Y)
                    break;
                row++;
            }

            // calc col mouse was over
            foreach (var columnDefinition in gameGrid.ColumnDefinitions)
            {
                accumulatedWidth += columnDefinition.ActualWidth;
                if (accumulatedWidth >= point.X)
                    break;
                col++;
            }
            game.CellSelected(col, row);   
        }
    }
}

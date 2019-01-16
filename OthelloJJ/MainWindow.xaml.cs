using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Serialization;
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

            DataContext = game;

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

        private void ButtonNettoyer_Click(object sender, RoutedEventArgs e)
        {
            
            MessageBoxResult result = ShowMessageBoxAndGetRespons("Redémarrer", "Voulez-vous vraiment redémarrer la partie");
            if (result == MessageBoxResult.Yes)
            {
                game.Clean();
            }
            
        }

        private void Button2Player_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = ShowMessageBoxAndGetRespons("2 Joueurs", "Voulez-vous vraiment lancer une partie 2 Joueurs");
            if (result == MessageBoxResult.Yes)
            {
                Button2Player.IsEnabled = false;
                Button1Player.IsEnabled = true;
                Button0Player.IsEnabled = true;
                game.SetIA(0);
                game.Clean();
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();

                saveFileDialog.Filter = "jj files (*.jj)|*.jj|All files (*.*)|*.*";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == true)
                {
                    BinarySerialization.WriteToBinaryFile<Game>(saveFileDialog.FileName, game);
                }
            }
            catch (SerializationException exp)
            {
                MessageBox.Show("Erreur veuillez reessayer",
                                "Erreur de sauvegarde",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        private void ButtonRestore_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = ShowMessageBoxAndGetRespons("Charger Partie", "Voulez-vous vraiement charger une partie ? \nLa partie démarre directement");
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog();

                    openFileDialog.Filter = "jj files (*.jj)|*.jj|All files (*.*)|*.*";
                    openFileDialog.FilterIndex = 1;
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == true)
                    {
                        game.Clean();
                        game = BinarySerialization.ReadFromBinaryFile<Game>(openFileDialog.FileName);
                        DataContext = game;

                    }
                }
                catch (SerializationException exp)
                {
                    MessageBox.Show("Erreur veuillez reessayer",
                                 "Le fichier n'est pas de type .jj",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Error);
                }

            }

        }

        public MessageBoxResult ShowMessageBoxAndGetRespons(string title,string text)
        {
            return MessageBox.Show(text,
                              title,
                              MessageBoxButton.YesNo,
                              MessageBoxImage.Warning);
        }

        private void Button1Player_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = ShowMessageBoxAndGetRespons("1 Joueur", "Voulez-vous vraiment lancer une partie 1 Joueur");
            if (result == MessageBoxResult.Yes)
            {
                Button1Player.IsEnabled = false;
                Button2Player.IsEnabled = true;
                Button0Player.IsEnabled = true;
                game.SetIA(1);
                game.Clean();
            }
        }

        private void Button0Player_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = ShowMessageBoxAndGetRespons("0 Joueur", "Voulez-vous vraiment lancer une partie 0 Joueur");
            if (result == MessageBoxResult.Yes)
            {
                Button0Player.IsEnabled = false;
                Button1Player.IsEnabled = true;
                Button2Player.IsEnabled = true;
                game.SetIA(2);
                game.Clean();
            }

        }
    }
}

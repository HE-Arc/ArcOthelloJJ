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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ImageSource imgMozilla;
        private ImageSource imgChorme;
        private Game game;
        public static MainWindow mainWindow;

        public MainWindow()
        {
            InitializeComponent();
            mainWindow = this; //So we can access UI component from Game
            game = new Game();

            DataContext = game;

            imgChorme = game.ImageSourceForBitmap(Properties.Resources.chrome);
            imgMozilla = game.ImageSourceForBitmap(Properties.Resources.firefox);
            //Draw player picture next to the score
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

        private Tuple<int,int> Getindex()
        {
            //Source : https://stackoverflow.com/a/20511247/7570047
            //Find mouse position inside the grid
            var point = Mouse.GetPosition(gameGrid);

            var row = 0;
            var col = 0;
            var accumulatedHeight = 0.0;
            var accumulatedWidth = 0.0;

            //Add height since we are bigger than mouse position
            foreach (var rowDefinition in gameGrid.RowDefinitions)
            {
                accumulatedHeight += rowDefinition.ActualHeight;
                if (accumulatedHeight >= point.Y)
                    break;
                row++;
            }

            //Add width since we are bigger than mouse position
            foreach (var columnDefinition in gameGrid.ColumnDefinitions)
            {
                accumulatedWidth += columnDefinition.ActualWidth;
                if (accumulatedWidth >= point.X)
                    break;
                col++;
            }
            return new Tuple<int, int>(col, row);
        }
        private void Grid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var pos=Getindex();
            game.CellSelected(pos.Item1, pos.Item2);
        }

        private void ButtonNettoyer_Click(object sender, RoutedEventArgs e)
        {

            var result = ShowMessageBoxAndGetRespons("Redémarrer", "Voulez-vous vraiment redémarrer la partie");
            if (result == MessageBoxResult.Yes)
            {
                game.Clean();
            }
            
        }

        private void Button2Player_Click(object sender, RoutedEventArgs e)
        {
            var result = ShowMessageBoxAndGetRespons("2 Joueurs", "Voulez-vous vraiment lancer une partie 2 Joueurs");
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
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "jj files (*.jj)|*.jj|All files (*.*)|*.*",
                    FilterIndex = 1,
                    RestoreDirectory = true
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    BinarySerialization.WriteToBinaryFile<Game>(saveFileDialog.FileName, game);
                }
            }
            catch (SerializationException)
            {
                MessageBox.Show("Erreur veuillez reessayer",
                                "Erreur de sauvegarde",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        private void ButtonRestore_Click(object sender, RoutedEventArgs e)
        {
            var result = ShowMessageBoxAndGetRespons("Charger Partie", "Voulez-vous vraiement charger une partie ? \nLa partie démarre directement");
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var openFileDialog = new OpenFileDialog
                    {
                        Filter = "jj files (*.jj)|*.jj|All files (*.*)|*.*",
                        FilterIndex = 1,
                        RestoreDirectory = true
                    };

                    if (openFileDialog.ShowDialog() == true)
                    {
                        game.Clean();
                        game = BinarySerialization.ReadFromBinaryFile<Game>(openFileDialog.FileName);
                        DataContext = game;

                    }
                }
                catch (SerializationException)
                {
                    MessageBox.Show("Erreur veuillez reessayer",
                                 "Le fichier n'est pas de type .jj",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Error);
                }

            }

        }

        private MessageBoxResult ShowMessageBoxAndGetRespons(string title,string text)
        {
            return MessageBox.Show(text,
                              title,
                              MessageBoxButton.YesNo,
                              MessageBoxImage.Warning);
        }

        private void Button1Player_Click(object sender, RoutedEventArgs e)
        {
            var result = ShowMessageBoxAndGetRespons("1 Joueur", "Voulez-vous vraiment lancer une partie 1 Joueur");
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
            var result = ShowMessageBoxAndGetRespons("0 Joueur", "Voulez-vous vraiment lancer une partie 0 Joueur");
            if (result == MessageBoxResult.Yes)
            {
                Button0Player.IsEnabled = false;
                Button1Player.IsEnabled = true;
                Button2Player.IsEnabled = true;
                game.SetIA(2);
                game.Clean();
            }

        }

        private void GameGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            gameGrid.Children.Clear();
            game.IsMouseOnGrid = true;
            game.Draw();
        }

        private void GameGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            gameGrid.Children.Clear();
            game.IsMouseOnGrid = false;
            game.Draw();
        }
    }
}

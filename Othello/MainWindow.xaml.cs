using Othello.GameLogic;
using Othello.Models;
using Othello.View.UserControls;
using System.Windows;
using System.Windows.Controls;

namespace Othello
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Tile[,] tiles = new Tile[8, 8];
        public MainWindow()
        {
            InitializeComponent();
            //InitializeGameGrid();
            //this.LayoutUpdated += Window_SizeChanged;
            GameEngine game =new GameEngine();
            Grid board = game.GetGameBoard();
            Grid.SetRow(board, 1);
            LayoutGrid.Children.Add(board);
        }

        private void LayoutGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }
        //private void InitializeGameGrid()
        //{
        //}
        //private void Window_SizeChanged(object? sender, EventArgs e)
        //{


        //    // Use the smaller dimension (width or height) to maintain square aspect ratio
        //    RowDefinition row = LayoutGrid.RowDefinitions[1];
        //    double size = Math.Min(row.ActualHeight, LayoutGrid.ActualWidth);
        //    size -= 5;

        //    // Set both width and height to the same size to maintain a square grid
        //    DebugTextBox.Text = "resized Size: " + size + " Height: " + board.ActualHeight + " width: " + board.ActualWidth + " row " + row.ActualHeight;
        //    board.Width = board.Height = size;
        //}



        //private bool isMoveValid(PlayerColor player, int row, int col)
        //{
        //    //123
        //    //4 6
        //    //789
        //    return false;
        //}

        //private void LayoutGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        //{

        //}
    }
}
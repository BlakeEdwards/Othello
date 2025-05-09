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
        GameEngine game;
        public MainWindow()
        {
            InitializeComponent();
            gameMenu.MenuItemClicked += onMenuAction;
            //InitializeGameGrid();
            //this.LayoutUpdated += Window_SizeChanged;
            game = new GameEngine();
            Grid GamePanel = game.GetGamePanel();
            Grid.SetRow(GamePanel, 1);
            LayoutGrid.Children.Add(GamePanel);
        }

        private void onMenuAction(MenuAction action)
        {
            switch(action)
            {
                case MenuAction.NewGame:
                    game.ResetGame();
                    break;
                case MenuAction.Undo:
                    game.UndoMove();
                    break;
                default:
                    break;

            }
            return;
        }
    }
}
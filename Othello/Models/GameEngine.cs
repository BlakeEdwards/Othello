using Othello;
using Othello.GameLogic;
using Othello.View.UserControls;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Othello.Models
{
    internal class GameEngine
    {
        Tile[,] board = new Tile[8, 8];
        private GameDashboard gameDashboard;
        private int player1Score = 2;
        private int player2Score = 2;
        public int Player1Score => player1Score;
        public int Player2Score => player2Score;

        private Players CurrentPlayer = Players.Player1;
        public GameEngine() 
        { 
            gameDashboard = new GameDashboard();

        }


        public Grid GetGamePanel()
        {
            //creat gamepanel
            Grid gamePanel = new Grid
            {
                Name = "wrapperBoard",
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            //define/add columns for  daashboard board
            ColumnDefinition col1 = new ColumnDefinition();
            col1.Width = new GridLength(2, GridUnitType.Star);
            ColumnDefinition col2 = new ColumnDefinition();
            col2.Width = new GridLength(8, GridUnitType.Star);
            gamePanel.ColumnDefinitions.Add(col1);
            gamePanel.ColumnDefinitions.Add(col2);

            //add dashboard
            Grid.SetColumn(gameDashboard, 0);
            gamePanel.Children.Add(gameDashboard);
            gameDashboard.updateDisplay(2, 2, CurrentPlayer);
            //add board
            Grid board = new Grid { Name = "board" };
            Grid.SetColumn(board, 1);
            Grid.SetZIndex(board, 0);
            gamePanel.Children.Add(board);
            gamePanel.SizeChanged += Grid_SizeChanged;

            //setup board
            int size = 8;
            // add all rows and columns
            for (int i = 0; i < size; i++)
            {
                ColumnDefinition col = new ColumnDefinition();
                RowDefinition row = new RowDefinition();
                board.ColumnDefinitions.Add(col);
                board.RowDefinitions.Add(row);
            }
            //add tiles to grid and gamestate
            for (int r = 0; r < size; r++)
            {
                for (int c = 0; c < size; c++)
                {
                    BaseTile gameTile = new BaseTile();
                    Tile tile = new Tile(gameTile, r, c);
                    Grid.SetRow(tile.tile, r);
                    Grid.SetColumn(tile.tile, c);
                    tile.TileClicked += MoveMade;
                    tile.TileEnter += Tile_TileEnter;
                    this.board[r, c] = tile;

                    board.Children.Add(tile.tile);
                }
            }
            return gamePanel;
        }

        private Players Tile_TileEnter(Tile sender)
        {
            return CurrentPlayer;
        }

        public void resetGame()
        {
            CurrentPlayer = Players.Player1;
            player1Score = 2;
            player2Score = 2;
            foreach (Tile tile in this.board)
            {
                tile.resetTile();
            }
            gameDashboard.updateDisplay(player1Score, player2Score, CurrentPlayer);
        }

        public void setDashboardDisplay()
        {
            gameDashboard.updateDisplay(player1Score, player2Score, CurrentPlayer);
        }


        private Players MoveMade(Tile tile, TileClickedEventArgs e)
        {
            if(tile.Owner != Players.None) { return Players.None; }
            Players newOwner = CurrentPlayer;
            List<Tile> flipableTiles = GetFlippableTiles(tile, CurrentPlayer);
            if (flipableTiles.Count > 0)
                {
                //Update related Tiles
                //update CurrentPlayer
                flipableTiles.ForEach(tl => tl.Owner = newOwner) ;
                tile.Owner = newOwner;
                if (CurrentPlayer == Players.Player1) 
                { 
                    CurrentPlayer = Players.Player2;
                    player1Score += 1 + flipableTiles.Count;
                    player2Score -= flipableTiles.Count;
                }
                else 
                { 
                    CurrentPlayer = Players.Player1;
                    player2Score += 1 + flipableTiles.Count;
                    player1Score -= flipableTiles.Count;
                }

                gameDashboard.updateDisplay(player1Score, player2Score, CurrentPlayer);
                return newOwner;
            }
            return Players.None;
        }
        private List<Tile> GetFlippableTiles(Tile origin, Players player)
        {
            var directions8 = new (int dr, int dc)[]
            {
                (-1,  0), // N
                (-1,  1), // NE
                ( 0,  1), // E
                ( 1,  1), // SE
                ( 1,  0), // S
                ( 1, -1), // SW
                ( 0, -1), // W
                (-1, -1)  // NW
            };
            var flippable = new List<Tile>();
            foreach (var dir in directions8)
            {
                var temp = new List<Tile>();
                int r = origin.Row + dir.dr, c = origin.Col + dir.dc;
                if (IsOnBoard(r, c) && board[r, c].Owner == player) { continue; }
                while (IsOnBoard(r, c))
                {
                    var tile = board[r, c];
                    if (tile.Owner == Players.None) break;
                    if (tile.Owner == player)
                    {
                        flippable.AddRange(temp);
                        break;
                    }
                    temp.Add(tile);
                    r += dir.dr;
                    c += dir.dc;
                }
            }
            return flippable;
        }

        private bool IsOnBoard(int r, int c)
        {
            if (r < 0 || c < 0) { return false; }
            if (r >= 8 || c >= 8) { return false; }
            return true;
        }

        //Auto Resize Grid
        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //FrameworkElement parent = sender.Parent as FrameworkElement;
            if (sender is Grid wrapperBoard)
            {
                Grid board = (Grid)wrapperBoard.Children[1];
                double size = Math.Min(wrapperBoard.ActualHeight, wrapperBoard.ActualWidth);
                board.Width = board.Height = size;
            }
        }
    }
}

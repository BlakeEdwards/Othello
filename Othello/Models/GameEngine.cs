using Othello;
using Othello.GameLogic;
using Othello.View.UserControls;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Othello.Models
{
    internal class GameEngine
    {
        Tile[,] board = new Tile[8, 8];

        private PlayerColor CurrentPlayer = PlayerColor.White;
        public GameEngine() { }
        public void StartGame() { }
        public Grid GetGameBoard()
        {
            Grid boardWrapper = new Grid
            {
                Name = "wrapperBoard",
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            Grid board = new Grid { Name = "board" };
            boardWrapper.Children.Add(board);
            boardWrapper.SizeChanged += Grid_SizeChanged;
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
                    this.board[r, c] = tile;

                    board.Children.Add(tile.tile);
                }
            }
            //starter squares (3,3)(3,4)(4,3)(4,4)

            this.board[3, 3].Owner = PlayerColor.White;
            this.board[4, 4].Owner = PlayerColor.White;
            this.board[3, 4].Owner = PlayerColor.Black;
            this.board[4, 3].Owner = PlayerColor.Black;
            return boardWrapper;
        }


        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //FrameworkElement parent = sender.Parent as FrameworkElement;
            if (sender is Grid wrapperBoard)
            {
                Grid board = (Grid)wrapperBoard.Children[0];
                double size = Math.Min(wrapperBoard.ActualHeight, wrapperBoard.ActualWidth);
                board.Width = board.Height = size;
            }
        }

        private PlayerColor MoveMade(Tile tile, TileClickedEventArgs e)
        {
            if(tile.Owner != PlayerColor.None) { return PlayerColor.None; }
            PlayerColor newOwner = CurrentPlayer;
            List<Tile> flipableTiles = GetFlippableTiles(tile, CurrentPlayer);
            if (flipableTiles.Count > 0)
                {
                //Update related Tiles
                //update CurrentPlayer
                foreach (Tile tl in flipableTiles)
                {
                    tl.Owner = newOwner;
                }
                tile.Owner = newOwner;
                if (CurrentPlayer == PlayerColor.White) { CurrentPlayer = PlayerColor.Black; }
                else { CurrentPlayer = PlayerColor.White; }

                return newOwner;
            }
            return PlayerColor.None;
        }
        private List<Tile> GetFlippableTiles(Tile origin, PlayerColor player)
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
                    if (tile.Owner == PlayerColor.None) break;
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
    }
}

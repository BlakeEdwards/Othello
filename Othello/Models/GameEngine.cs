using Othello;
using Othello.GameLogic;
using Othello.View.UserControls;
using System.Diagnostics;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Othello.Models
{
    //helper structs
    public struct Position
    {
        public int Row;
        public int Col;

        public Position(int row, int col) {  Row = row; Col = col; }
        public override bool Equals(object? obj)
        {
            if (obj is Position position)
            {
                return Row == position.Row && Col == position.Col;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Row, Col);
        }
    }

    internal struct LastMove
    {
        public int Row { get; }
        public int Col { get; }
        public int fliped {  get; }
        public Tile[] flippedTiles { get; }
        public LastMove(int row, int col, Tile[] flipped) { Row = row; Col = col; flippedTiles = flipped; fliped = flipped.Length; }
    }

    internal class GameEngine
    {
        Tile[,] board = new Tile[8, 8];
        private GameDashboard gameDashboard;
        private LastMove? lastMove;     // can be null
        private List<Position> movesMade = new List<Position>();
        private List<Position> emptyAdjacentTiles = new List<Position>();
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
            ResetGame();
            return gamePanel;
        }

        

        public void ResetGame()
        {
            CurrentPlayer = Players.Player1;
            player1Score = 2;
            player2Score = 2;
            movesMade = new List<Position>();
            foreach (Tile tile in this.board){ tile.resetTile(); }
            movesMade.Add(new Position(3,3));
            movesMade.Add(new Position(3, 4));
            movesMade.Add(new Position(4, 3));
            movesMade.Add(new Position(4, 4));
            gameDashboard.updateDisplay(player1Score, player2Score, CurrentPlayer);
            lastMove = null;
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
                lastMove = new LastMove(tile.Row, tile.Col, flipableTiles.ToArray());
                
                //log move get 
                movesMade.Add(new Position(tile.Row, tile.Col));
                //add new emptyAdjacentTiles
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

                foreach (var dir in directions8)
                {
                    int r = tile.Row + dir.dr, c = tile.Col + dir.dc;
                    Position position = new Position(r, c);
                    if (IsOnBoard(r, c) && board[r, c].Owner == Players.None && !emptyAdjacentTiles.Contains(position)) { emptyAdjacentTiles.Add(position); }
                }
                int scoreSum = player2Score + player1Score;
                if (player1Score == 0 || player2Score == 0) { scoreSum = 0; }
                if(scoreSum == 64 || scoreSum == 0 || !PlayerHasMove(CurrentPlayer))
                {
                    //winner
                    string msg;
                    Players winner = Players.None;
                    winner = player1Score > player2Score ? Players.Player1: Players.Player2;
                    if (player1Score == player2Score) { msg = "Draw"; }
                    else { msg = Players.Player1 == winner ? "Player 1 Wins" : "Player 2 Wins"; }
                    gameDashboard.updateMsgBoard(msg);
                }
                    return newOwner;
            }
            return Players.None;
        }
        //summary
        //takes list of moves starting with player1 and performs moves in the form A1, e8
        //
        private void autoGame(string movesCsv)
        {
            var tempPlayer = Players.Player1;
            var moves = movesCsv.Split(',');
            foreach (var move in moves)
            {
                var moveNotation = move.Trim().ToUpper();

                //access
                int col = moveNotation[0] - 'A'; //access move char and - 'A' char to shift alpahbet char int back to 0 
                int row = moveNotation[1] - 1;
                //switch tempPlayer
                tempPlayer = tempPlayer == Players.Player2 ? Players.Player1 : Players.Player2;
                MoveMade(board[row, col], null);
            }
        }
        private bool PlayerHasMove(Players player)
        {
            foreach (var move in emptyAdjacentTiles)
            {
                if(GetFlippableTiles(board[move.Row,move.Col],CurrentPlayer).Count > 0) { return true; }
            }
            return false;
        }

        public void UndoMove()
        {
            if (lastMove == null) { return; }
            int row = lastMove.Value.Row;
            int col = lastMove.Value.Col;
            Players lastPlayer = board[row, col].Owner;
            Players otherPlayer = Players.Player1;
            if (lastPlayer == Players.Player1) { otherPlayer = Players.Player2; }
            board[row, col].Owner = Players.None;

            foreach (var tile in lastMove.Value.flippedTiles)
            {
                row = tile.Row;
                col = tile.Col;
                board[row, col].Owner = otherPlayer;
            }
            
            CurrentPlayer = lastPlayer;
            int scorechange = lastMove.Value.fliped;
            if (lastPlayer == Players.Player1)
            {
                player1Score -= (1 + scorechange);
                player2Score += scorechange;
            }
            else
            {
                player2Score -= (1 + scorechange);
                player1Score += scorechange;
            }
            gameDashboard.updateDisplay(player1Score, player2Score, CurrentPlayer);
            lastMove = null;
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

        //Events
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

        private Players Tile_TileEnter(Tile sender)
        {
            return CurrentPlayer;
        }
    }
}

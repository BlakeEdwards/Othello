using Othello.View.UserControls;

namespace Othello.GameLogic
{
    internal class TileClickedEventArgs : EventArgs
    {
        public Tile BaseTile { get; }
        public int Row { get; }
        public int Col { get; }

        public TileClickedEventArgs(Tile baseTile, int row, int col)
        {
            BaseTile = baseTile;
            Row = row;
            Col = col;
        }
    }

    public enum Owner { Null, White, Black };
    internal class Tile
    {
        //event delegate and Event
        public delegate Players TileClickedHandler(Tile sender, TileClickedEventArgs e);
        public event TileClickedHandler? TileClicked;
        public delegate Players TileEnterHandler(Tile sender);
        public event TileEnterHandler? TileEnter;

        public BaseTile tile;
        private readonly int row;
        private readonly int col;
        
        public int Row => row;
        public int Col => col;
        private Players tileOwner;
        public Players Owner
        {
            get => tileOwner;
            set
            {
                tileOwner = value;
                tile.Owner = value;
            }

        }

        public Tile(Players owner)
        {
            this.tileOwner = owner;
            tile = new BaseTile();
        }

        

        public Tile()
        {
            Owner = Players.None;
            tile = new BaseTile();
        }
        public Tile(BaseTile tile, int row, int col)
        {
            this.tile = tile;
            this.tile.BaseTileClicked += Tile_Click;
            this.tile.BaseTileMouseEnter += Tile_BaseTileMouseEnter;
            this.row = row;
            this.col = col;
            resetTile();
        }

        private Players? Tile_BaseTileMouseEnter(BaseTile sender)
        {
            if(TileEnter?.Invoke(this) is Players player)
            { return player; }
            return null;
        }

        private void Tile_Click(BaseTile tile)
        {
            Players plColor = (Players?)TileClicked?.Invoke(this, new TileClickedEventArgs(this, row, col)) ?? Players.None;
            if(plColor != Players.None){ this.tileOwner = plColor; }
            return;
        }
        public void resetTile() 
        {
            if ((row == 3 && col == 3) || (row == 4 && col == 4)) { Owner = Players.Player1; }
            else if ((row == 4 && col == 3) || (row == 3 && col == 4)) { Owner = Players.Player2; }
            else { Owner = Players.None; }
        }
    }
}

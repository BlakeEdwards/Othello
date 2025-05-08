using Othello.View.UserControls;

namespace Othello.GameLogic
{
    public class TileClickedEventArgs : EventArgs
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
    public class Tile
    {
        //event delegate and Event
        public delegate PlayerColor TileClickedHandler(Tile sender, TileClickedEventArgs e);
        public event TileClickedHandler? TileClicked;

        public BaseTile tile;
        private int row;
        private int col;
        
        public int Row { get { return row; } }
        public int Col { get { return col; } }
        private PlayerColor tileOwner;
        public PlayerColor Owner
        {
            get => tileOwner;
            set
            {
                tileOwner = value;
                tile.Owner = value;
            }

        }

        public Tile(PlayerColor owner)
        {
            this.tileOwner = owner;
            tile = new BaseTile();
        }

        

        public Tile()
        {
            Owner = PlayerColor.None;
            tile = new BaseTile();
        }
        public Tile(BaseTile tile, int row, int col)
        {
            this.tile = tile;
            this.tile.BaseTileClicked += Tile_Click;
            this.tileOwner = PlayerColor.None;
            this.row = row;
            this.col = col;
        }
        private void Tile_Click(BaseTile tile)
        {
            PlayerColor plColor = (PlayerColor?)TileClicked?.Invoke(this, new TileClickedEventArgs(this, row, col)) ?? PlayerColor.None;
            if(plColor != PlayerColor.None){ this.tileOwner = plColor; }
            return;
            /*
            if (tile.Owner == PlayerColor.None) { return PlayerColor.White; }
            if (tile.Owner == PlayerColor.White) { return PlayerColor.Black; }
            if (tile.Owner == PlayerColor.Black) { return PlayerColor.White; }
            // validate move magic

            // getCurrentplayer
            //getTiles to update
            return PlayerColor.None;*/
        }
    }
}

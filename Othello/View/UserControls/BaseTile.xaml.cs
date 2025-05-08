using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Othello.View.UserControls
{
    /// <summary>
    /// Interaction logic for GameTile.xaml
    /// </summary>
    public partial class BaseTile : UserControl
    {
        //vars
        public PlayerColor owner;
        public PlayerColor Owner
        {
            get => owner;
            set
            {
                owner = value;
                if (Owner == PlayerColor.None) { Piece.Fill = null; }
                else if (Owner == PlayerColor.White) { Piece.Fill = Brushes.BlueViolet; }
                else if (Owner == PlayerColor.Black) { Piece.Fill = Brushes.SpringGreen; }
            }
        }


        //Init
        public BaseTile()
        {
            InitializeComponent();
        }

        //Event Def
        public delegate void TileClickedHandler(BaseTile sender);
        public event TileClickedHandler? BaseTileClicked;
        //Tile Click Event
        protected virtual void OnTileClicked(object sender, RoutedEventArgs e)
        {
            BaseTileClicked?.Invoke(this);
            //if (newOwner != PlayerColor.None) { Owner = newOwner; }
        }
    }
}

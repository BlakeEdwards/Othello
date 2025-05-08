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
        public Players owner;
        public Players Owner
        {
            get => owner;
            set
            {
                owner = value;
                //get colors

                if (Owner == Players.None) { Piece.Fill = null; }
                else if (Owner == Players.Player1) { Piece.Fill = (Brush)Application.Current.Resources["Player1Color"]; }
                else if (Owner == Players.Player2) { Piece.Fill = (Brush)Application.Current.Resources["Player2Color"]; }
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

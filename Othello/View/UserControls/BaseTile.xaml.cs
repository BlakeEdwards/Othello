using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

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
                changePieceColor(owner);

                AnimatePulse();
            }
        }

        //Init
        public BaseTile()
        {
            InitializeComponent();
        }
        private void changePieceColor(Players? player)
        {
            if (player == Players.Player1) { Piece.Fill = (Brush)Application.Current.Resources["Player1Color"]; }
            else if (player == Players.Player2) { Piece.Fill = (Brush)Application.Current.Resources["Player2Color"]; }
            // clear unowner tile when mouse leaves
            else { Piece.Fill = null; }
        }

        //Event Def
        public delegate void TileClickedHandler(BaseTile sender);
        public event TileClickedHandler? BaseTileClicked;

        public delegate Players? TileMouseEnterHandler(BaseTile sender);
        public event TileMouseEnterHandler? BaseTileMouseEnter;


        //Tile Click Event
        protected virtual void OnTileClicked(object sender, RoutedEventArgs e)
        {
            BaseTileClicked?.Invoke(this);
            //if (newOwner != PlayerColor.None) { Owner = newOwner; }
        }
        public void AnimatePulse()
        {
            var animation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = TimeSpan.FromSeconds(0.2),
                AutoReverse = true,
                RepeatBehavior = new RepeatBehavior(2)
            };

            var storyboard = new Storyboard();
            Storyboard.SetTarget(animation, Piece);
            Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));
            storyboard.Children.Add(animation);
            storyboard.Begin();
        }

        private void Border_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (this.Owner is not Players.None) { return; }
            if (BaseTileMouseEnter?.Invoke(this) is Players currentPlayer)
            {
                if (this.Owner == Players.None)
                {
                    changePieceColor(currentPlayer);
                }
            }
        }

        private void Border_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (this.Owner is not Players.None) { return; }
            changePieceColor(null);
        }
    }
}

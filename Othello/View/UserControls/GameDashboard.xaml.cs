using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Othello.View.UserControls
{
    /// <summary>
    /// Interaction logic for GameDashboard.xaml
    /// </summary>
    public partial class GameDashboard : UserControl
    {
        public GameDashboard()
        {
            InitializeComponent();
        }
        public delegate void AiChangedHandler(Players player, bool enabled);
        public event AiChangedHandler? onAiChanged;

        private void p1RadioButton_EnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            
            if (sender is not RadioButton){ return; }
            
            RadioButton radioButton = (RadioButton)sender;
            onAiChanged?.Invoke(Players.Player1, radioButton.IsEnabled);
            if (radioButton.IsEnabled) { ai2RadioButton.IsEnabled = false;}
        }
        private void p2RadioButton_EnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is not RadioButton) { return; }
            RadioButton radioButton = (RadioButton)sender;
            onAiChanged?.Invoke(Players.Player2, radioButton.IsEnabled);
            if (radioButton.IsEnabled) { ai1RadioButton.IsEnabled = false; }
        }
        public void updateDisplay(int player1Score, int player2Score, Players CurrentTrun)
        {

        }
        public void updateScores(int player1Score, int player2Score)
        {
            p1Score.Content = player1Score.ToString();
            p2Score.Content = player2Score.ToString();
        }
    }
}

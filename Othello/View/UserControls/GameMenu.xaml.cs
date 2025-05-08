using System.Windows;
using System.Windows.Controls;

namespace Othello.View.UserControls
{

    /// <summary>
    /// Interaction logic for GameMenu.xaml
    /// </summary>
    public enum MenuAction { PlaySolo, PlayAI, Quit, Settings }
    public partial class GameMenu : UserControl
    {
        public event Action<MenuAction>? MenuItemClicked;

        int darkTheme = -1;
        public GameMenu()
        {
            InitializeComponent();
        }

        private void ToogleTheme_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Resources.MergedDictionaries.Clear();
            string uriString = "Themes/DarkTheme.xaml";
            ThemeMenuItem.Header = "DarkTheme";
            if (darkTheme == 1)
            {
                uriString = "Themes/LightTheme.xaml";
                ThemeMenuItem.Header = "LightTheme";
            }
            Application.Current.Resources.MergedDictionaries.Add(
                    new ResourceDictionary { Source = new Uri(uriString, UriKind.Relative) });
            darkTheme *= -1;
            //DebugTextBox.Text = "Theme Changed: " + darkTheme;
        }

        private void PlaySolo_Click(object sender, RoutedEventArgs e)
        {
            MenuItemClicked?.Invoke(MenuAction.PlaySolo);
        }
    }
}

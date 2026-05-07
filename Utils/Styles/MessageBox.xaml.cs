using System.Windows;
using System.Windows.Input;

namespace Valorant_Instalocker
{
    public partial class CustomMessageBox : Window
    {
        public bool Result { get; set; } = false;



        public CustomMessageBox(string message, string title)
        {
            InitializeComponent();
            MessageText.Text = message; 
            TitleText.Text = title;   
        }

        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            Result = true; 
            this.Close(); 
        }

        private void No_Click(object sender, RoutedEventArgs e)
        {
            Result = false;
            this.Close();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
    }
}
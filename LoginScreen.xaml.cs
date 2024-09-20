using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF_screen
{
    public partial class LoginScreen : Window
    {
        private string userName = "";
        private string passWord = "";

        public LoginScreen()
        {
            InitializeComponent();
        }

        private void LogInButton_Click(object sender, RoutedEventArgs e)
        {
            userName = userNameInput.Text;
            passWord = passWordInput.Text;

            if (userName == "Username" || passWord == "Password")
            {
                warningMessage.Content = "Please enter both a username and a password to proceed.";
                return;
            }


            Console.WriteLine($"Hello {userName}");

        }
    }
}
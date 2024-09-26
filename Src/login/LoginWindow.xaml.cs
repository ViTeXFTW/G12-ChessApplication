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
using System.Windows.Shapes;

namespace G12_ChessApplication
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public event EventHandler LoginSuccess;
        public LoginWindow()
        {
            InitializeComponent();
        }
        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            // Authentication logic (you can connect it to your database)
            bool isAuthenticated = AuthenticateUser(UsernameBox.Text, PasswordBox.Password);

            if (isAuthenticated)
            {
                // Trigger the LoginSuccess event
                LoginSuccess?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                MessageBox.Show("Invalid credentials. Please try again.");
            }
        }

        private bool AuthenticateUser(string username, string password)
        {
            // Placeholder for actual authentication logic
            return username == "admin" && password == "password"; // Example logic
        }
    }
}
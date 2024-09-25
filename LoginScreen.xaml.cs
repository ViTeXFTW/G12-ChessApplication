using System.Windows;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;


namespace G12_ChessApplication
{
    public partial class LoginScreen : Window
    {
        private string userName = "";
        private string passWord = "";

        // In-memory user list for simplicity. Later, you can replace this with a DB.
        private User currentUser;

        // Database context
        private AppDbContext dbContext;

        // Current logged-in user
       
        
        public LoginScreen()
        {
            InitializeComponent();
            dbContext = new AppDbContext();
            dbContext.Database.EnsureCreated(); // Ensures the database is created if it doesn't exist.
            
        }

        private void UserNameInput_TextChanged(object sender, RoutedEventArgs e)
        // Hides the placeholder text when the user starts typing in the username field.
        {
            if (userNameInput.Text.Length > 0)
            {
                tbUsername.Visibility = Visibility.Collapsed;
            }
            else 
            {
                tbUsername.Visibility = Visibility.Visible;
            }
        }

        private void PassWordInput_PasswordChanged(object sender, RoutedEventArgs e)
        // Hides the placeholder text when the user starts typing in the password field.
        {
            if (passWordInput.Password.Length > 0)
            {
                tbPassword.Visibility = Visibility.Collapsed;
            }
            else
            {
                tbPassword.Visibility = Visibility.Visible;
            }
        }

        private async void LogInButton_Click(object sender, RoutedEventArgs e)
        {
            userName = userNameInput.Text;
            passWord = passWordInput.Password;

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(passWord))
            {
                warningMessage.Content = "Please enter both a username and a password to proceed.";
                return;
            }
    
            // Authenticate the user asynchronously
            currentUser = await AuthenticateUser(userName, passWord);

            if (currentUser != null)
            {
                Console.WriteLine("Login successful");
                Console.WriteLine($"Logged in as: {currentUser.Username}");

                // Handle admin vs. user logic
                
            }
            else
            {
                warningMessage.Content = "Invalid username or password.";
            }
        }
        
        private async Task<User> AuthenticateUser(string username, string password)
        {
            // Query the database asynchronously to find the user by username and password
            return await dbContext.Login.FirstOrDefaultAsync(user => user.Username == username && user.Passw == password);
        }
        
        
        private void CreateButton_Click(object sender, RoutedEventArgs e) // Eventhandler for CreateButton
        {
            userName = userNameInput.Text;
            passWord = passWordInput.Password;

            // Check if the fields are empty
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(passWord))
            {
                warningMessage.Content = "Please enter both a username and a password to create an account.";
                return;
            }

            // Check if the user already exists in the database
            var existingUser = dbContext.Login.FirstOrDefault(user => user.Username == userName);
    
            if (existingUser != null)
            {
                // User already exists
                warningMessage.Content = "Username already exists. Please choose a different username.";
            }
            else
            {
                // User doesn't exist, so create a new one
                var newUser = new User
                {
                    Username = userName,
                    Passw = passWord,
                    
                };

                // Add the new user to the database
                dbContext.Login.Add(newUser);
                dbContext.SaveChanges(); // Save changes to the database

                // Inform the user that the account has been created
                warningMessage.Content = "Account created successfully! You can now log in.";

                // Optionally clear the input fields after creation
                userNameInput.Clear();
                passWordInput.Clear();
            }
        }
        
        
        public class AppDbContext : DbContext
        {
            public DbSet<User> Login { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                // Correct MySQL connection string with actual database name
                var connectionString = "Server=mysql-makki12.alwaysdata.net;Database=makki12_chess;User=makki12;Password=ChessGame_;";
                optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                // Fluent API: Set UserName as the primary key
                modelBuilder.Entity<User>()
                    .HasKey(u => u.Username);

                // Seed the database with an admin and a test user
                modelBuilder.Entity<User>().HasData(
                    new User { Username = "admin", Passw = "admin" },
                    new User { Username = "test", Passw = "123"}
                );
            }
        }


        public class User
        {
            [Key]  // Marks UserName as the primary key
            public string Username { get; set; } 

            public string Passw { get; set; }
            
        }

        

        
    }
}
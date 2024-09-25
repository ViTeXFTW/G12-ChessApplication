using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

public class SQLConnector
{
    private AppDbContext dbContext;

    public SQLConnector()
    {
        dbContext = new AppDbContext();
    }

    public void InitializeDatabase()
    {
        dbContext.Database.EnsureCreated();
    }

    public async Task<User> AuthenticateUser(string username, string password)
    {
        // Query the database asynchronously to find the user by username and password
        return await dbContext.Login.FirstOrDefaultAsync(user => user.Username == username && user.Passw == password);
    }

    public string CreateUser(string username, string password)
    {
        // Check if the user already exists in the database
        var existingUser = dbContext.Login.FirstOrDefault(user => user.Username == username);

        if (existingUser != null)
        {
            return "Username already exists. Please choose a different username.";
        }

        // Create a new user
        var newUser = new User
        {
            Username = username,
            Passw = password
        };

        // Add the new user to the database
        dbContext.Login.Add(newUser);
        dbContext.SaveChanges();

        return "Account created successfully! You can now log in.";
    }

    public class User
    {
        [Key]
        public string Username { get; set; }
        public string Passw { get; set; }
    }

    public class AppDbContext : DbContext
    {
        public DbSet<User> Login { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = "Server=mysql-makki12.alwaysdata.net;Database=makki12_chess;User=makki12;Password=ChessGame_;";
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(u => u.Username);

            // Seed the database with an admin and a test user
            modelBuilder.Entity<User>().HasData(
                new User { Username = "admin", Passw = "admin" },
                new User { Username = "test", Passw = "123" }
            );
        }
    }
}

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using G12_ChessApplication;
using System.ComponentModel.DataAnnotations.Schema;
using Org.BouncyCastle.Security;
using static SQLConnector;

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

    //create match constructor:
    
    
    
    public async Task<User> AuthenticateUser(string username, string password)
    {
        using (var context = new AppDbContext())
        {
            // Query the database asynchronously to find the user by username and password
            return await context.Login.FirstOrDefaultAsync(user => user.Username == username && user.Passw == password);
        }
    }

    public string CreateUser(string username, string password)
    {
        using (var context = new AppDbContext())
        {
            // Check if the user already exists in the database
            var existingUser = context.Login.FirstOrDefault(user => user.Username == username);
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
            context.Login.Add(newUser);
            context.SaveChanges();

            return "Account created successfully! You can now log in.";
        }
    }


    public class User
    {
        [Key]
        public string Username { get; set; }
        public string Passw { get; set; }

        // Navigation property for matches as Player 1
        public ICollection<Match> MatchesAsP1 { get; set; }

        // Navigation property for matches as Player 2
        public ICollection<Match> MatchesAsP2 { get; set; }
    }

    public class Match
    {
        // Reference to User for P1
        public string P1_username { get; set; }
        public User P1 { get; set; }

        // Reference to User for P2
        public string P2_username { get; set; }
        public User P2 { get; set; }

        public int P1_wins { get; set; }
        public int P2_wins { get; set; }
        public int Draws { get; set; }
    }

    public class AppDbContext : DbContext
    {
        public DbSet<User> Login { get; set; }
        public DbSet<Match> Matches { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = "Server=mysql-makki12.alwaysdata.net;Database=makki12_chess;User=makki12;Password=ChessGame_;";
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure primary key for User
            modelBuilder.Entity<User>()
                .HasKey(u => u.Username);

            // Configure primary key for Match
            modelBuilder.Entity<Match>()
                .HasKey(m => new { m.P1_username, m.P2_username });

            // Configure foreign key relationship between Match.P1Username and User.Username
            modelBuilder.Entity<Match>()
                .HasOne(m => m.P1)
                .WithMany(u => u.MatchesAsP1)
                .HasForeignKey(m => m.P1_username)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete to avoid circular references

            // Configure foreign key relationship between Match.P2Username and User.Username
            modelBuilder.Entity<Match>()
                .HasOne(m => m.P2)
                .WithMany(u => u.MatchesAsP2)
                .HasForeignKey(m => m.P2_username)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed the database with an admin and a test user
            modelBuilder.Entity<User>().HasData(
                new User { Username = "admin", Passw = "admin" },
                new User { Username = "test", Passw = "123" }
            );
        }
    }


    public async Task AddOrUpdateMatchResult(string P1Username, string P2Username, int result)
    {
        using (var context = new AppDbContext())
        {
            // Check that both users exist
            var player1 = await context.Login.FindAsync(P1Username);
            var player2 = await context.Login.FindAsync(P2Username);

            if (player1 == null || player2 == null)
            {
                throw new Exception("One or both users do not exist.");
            }

            // Ensure usernames are ordered alphabetically
            P1Username = string.Compare(P1Username, P2Username) <= 0 ? P1Username : P2Username;
            P2Username = string.Compare(P1Username, P2Username) <= 0 ? P2Username : P1Username;

            // Try to find an existing match between the two players
            var match = await context.Matches
                .FirstOrDefaultAsync(m => (m.P1_username == P1Username && m.P2_username == P2Username) || 
                (m.P1_username == P2Username && m.P2_username == P1Username));

            if (match == null)
            {
                // If no match exists, create a new entry
                match = new Match
                {
                    P1_username = P1Username,
                    P2_username = P2Username,
                    P1_wins = result == 1 ? 1 : 0,
                    P2_wins = result == 2 ? 1 : 0,
                    Draws = result == 0 ? 1 : 0
                };
                context.Matches.Add(match);
            }
            else
            {
                // If match exists, update the result
                if (result == 1) match.P1_wins++;
                else if (result == 2) match.P2_wins++;
                else if (result == 0) match.Draws++;
            }

            // Save changes to the database
            await context.SaveChangesAsync();
        }
    }


    public async Task<List<LeaderboardEntry>> GetLeaderboard()
    {
        using (var context = new AppDbContext())
        {
            var leaderboardData = await context.Login
                .Select(user => new
                {
                    Username = user.Username,
                    Wins = user.MatchesAsP1.Sum(m => m.P1_wins) + user.MatchesAsP2.Sum(m => m.P2_wins),
                    Losses = user.MatchesAsP1.Sum(m => m.P2_wins) + user.MatchesAsP2.Sum(m => m.P1_wins),
                    Draws = user.MatchesAsP1.Sum(m => m.Draws) + user.MatchesAsP2.Sum(m => m.Draws)
                })
                .ToListAsync();
         
            // Convert the aggregated data to a list of LeaderboardEntry objects
            return leaderboardData.Select(entry => new LeaderboardEntry(
                entry.Username,
                entry.Wins,
                entry.Losses,
                entry.Draws
            )).ToList(); 
        }
    }

    public async Task<List<LeaderboardEntry>> GetMatchHistory(string username)
    {
        using (var context = new AppDbContext())
        {
            var matchHistory = await context.Matches
                .Where(m => m.P1_username == username || m.P2_username == username)
                .GroupBy(m => m.P1_username == username ? m.P2_username : m.P1_username) // Group by opponent's username
                .Select(g => new
                {
                    Opponent = g.Key,
                    Wins = g.Sum(m => m.P1_username == username ? m.P1_wins : m.P2_wins),
                    Losses = g.Sum(m => m.P1_username == username ? m.P2_wins : m.P1_wins),
                    Draws = g.Sum(m => m.Draws)
                })
                .ToListAsync();

            // Convert the aggregated data to a list of LeaderboardEntry objects
            return matchHistory.Select(entry => new LeaderboardEntry(
                entry.Opponent,
                entry.Wins,
                entry.Losses,
                entry.Draws
            )).ToList();
        }
    }
}

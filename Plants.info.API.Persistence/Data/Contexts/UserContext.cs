using Plants.info.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration; 
using Plants.info.API.Data.Models;
using Plants.info.API.Common.Data.Models;

namespace Plants.info.API.Data.Contexts
{
    public class UserContext: DbContext
    {
        private readonly IConfiguration _config;

        public UserContext(IConfiguration config)
        {
            _config = config;
        }


        public DbSet<User> Users { get; set; }
        public DbSet<Plant> Plants { get; set; }
        public DbSet<Genus> Genus { get; set; }
        public DbSet<PlantNote> PlantNotes { get; set; }
        public DbSet<GenusStat> GenusStat { get; set; }
        public DbSet<PlantImage> PlantImage { get; set; }
        public DbSet<AppAudit> AppAudit { get; set; }


        // Configure the context to a specific database = UserDb
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //var configuration = new ConfigurationBuilder()
            //   .SetBasePath(Directory.GetCurrentDirectory())
            //   .AddJsonFile("appsettings.json")
            //   .Build();
            //base.OnConfiguring(optionsBuilder);
            //optionsBuilder.UseSqlServer(_config["ConnectionStrings:UserContextDb"]);

            //var connectionString = _config["ConnectionStrings:UserContextDb"];
            var connectionString = ""; 
            if (System.Diagnostics.Debugger.IsAttached)
            {
                var userId = Environment.GetEnvironmentVariable("DB_ID_DEV") ?? "";
                var userPwd = Environment.GetEnvironmentVariable("DB_PSWD_DEV") ?? "";
                connectionString = $"Server=localhost;Database=PlantUsersDb;User Id={userId};Password={userPwd};MultipleActiveResultSets=true";
            }
            else
            {
                var userId = Environment.GetEnvironmentVariable("DB_ID") ?? "";
                var userPwd = Environment.GetEnvironmentVariable("DB_PSWD") ?? "";
                connectionString = $"Server=tcp:plantuserssqldb.database.windows.net,1433;Initial Catalog=PlantUsersDb;Persist Security Info=False;User ID={userId};Password={userPwd};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"; 
            }

            optionsBuilder.UseSqlServer(connectionString);  
        }
        // Specify how the mapping is going to happen between your entities and the db
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
    }
}

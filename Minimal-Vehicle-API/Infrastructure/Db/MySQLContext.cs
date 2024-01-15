using Microsoft.EntityFrameworkCore;
using Minimal_Vehicle_API.Domain.Entities;

namespace Minimal_Vehicle_API.Infrastructure.Db
{
    public class MySQLContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public MySQLContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DbSet<Admin> Admins { get; set; } = default!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var stringConexao = _configuration.GetConnectionString("mysql")?.ToString();
                if (!string.IsNullOrEmpty(stringConexao))
                {
                    optionsBuilder.UseMySql(
                        stringConexao,
                        ServerVersion.AutoDetect(stringConexao)
                    );
                }
            }
        }
    }
}

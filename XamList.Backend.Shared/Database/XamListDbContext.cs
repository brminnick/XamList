using System;
using Microsoft.EntityFrameworkCore;
using XamList.Shared;

namespace XamList.Backend.Shared
{
    class XamListDbContext : DbContext
    {
        readonly static string _connectionString = Environment.GetEnvironmentVariable("XamListDatabaseConnectionString") ?? string.Empty;

        public XamListDbContext() => Database.EnsureCreated();

        public DbSet<ContactModel> Contacts => Set<ContactModel>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlServer(_connectionString);
    }
}

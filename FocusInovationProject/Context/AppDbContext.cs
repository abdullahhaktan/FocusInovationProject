using FocusInovationProject.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FocusInovationProject.Context
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Sql Serverın yolu program.cs de ayarlandı 

        public DbSet<Product> Product { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<Stock> Stock { get; set; }
        public DbSet<Purchase> Purchase { get; set; }

    }
}

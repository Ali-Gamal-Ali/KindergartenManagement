using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WalaaKidsApp.DataAccess.Model;

namespace WalaaKidsApp.DataAccess
{
    public class AppDbContext :DbContext
    {
        public DbSet<Classroom> Classrooms { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Guardian> Guardians { get; set; }
        public DbSet<Subscription > Subscriptions { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<BusSubscription> BusSubscriptions { get; set; }

        public AppDbContext() : base(GetConnectionString(),true)
        {
            this.Database.Connection.StateChange += (sender, e) =>
            {
                if (e.CurrentState == System.Data.ConnectionState.Open)
                {
                    using (var command = this.Database.Connection.CreateCommand())
                    {
                        command.CommandText = "PRAGMA foreign_keys = ON;";
                        command.ExecuteNonQuery();
                    }
                }
            };

            this.Database.Log = Console.WriteLine;
        }

        private static SQLiteConnection GetConnectionString()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is missing in app.config.");
            }
            return new SQLiteConnection(connectionString);
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>().ToTable("Student");
            modelBuilder.Entity<Student>().HasOptional(s => s.Guardian)
                .WithRequired(bs => bs.Student);
            modelBuilder.Entity<Classroom>().ToTable("Classroom");
            modelBuilder.Entity<Guardian>().ToTable("Guardian").HasKey(g=>g.StudentId);
            modelBuilder.Entity<Subscription>().ToTable("Subscription");
            modelBuilder.Entity<Payment>().ToTable("Payment");
            modelBuilder.Entity<BusSubscription>().ToTable("BusSubscription");
            modelBuilder.Entity<BusSubscription>().HasKey(bs=>bs.StudentId);
            modelBuilder.Entity<BusSubscription>().HasRequired(bs => bs.Student)
               .WithOptional(s => s.BusSubscription);
        }
    }
}

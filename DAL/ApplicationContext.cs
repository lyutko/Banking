using DAL.Entities;
using System.Data.Entity;

namespace DAL
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext() : base("name=ApplicationContext")
        {
            //Database.SetInitializer(new DBInitializer());
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Operation> Operations { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<User>().HasRequired(x => x.Client).WithOptional(y => y.User);
            //modelBuilder.Entity<Client>().HasOptional(x => x.User).WithRequired(y => y.Client);
            base.OnModelCreating(modelBuilder);
        }
    }
}
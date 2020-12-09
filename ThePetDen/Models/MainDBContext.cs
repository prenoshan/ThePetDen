namespace ThePetDen.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class MainDBContext : DbContext
    {
        public MainDBContext()
            : base("name=MainDB")
        {
        }

        public virtual DbSet<Users> Users { get; set; }

        public virtual DbSet<Products> Products { get; set; }

        public virtual DbSet<Carts> Carts { get; set; }

        public virtual DbSet<Orders> Orders { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}

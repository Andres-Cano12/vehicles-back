using AccessData.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace AccessData.Context
{
    public class DBContext : IdentityDbContext<User>
    {
        public IConfiguration Configuration { get; }

        public DbSet<JobSeeker> JobSeekers { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }

        public DBContext(DbContextOptions<DBContext> options, IConfiguration configuration) : base(options)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Vehicle>();
            base.OnModelCreating(modelBuilder);
        }
    }
}

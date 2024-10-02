using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using PersonalizedLibraryAPI.Models;

namespace PersonalizedLibraryAPI.Data
{
    public class DBContext: IdentityDbContext<AppUser>
    {
        public DBContext(DbContextOptions<DBContext> options): base(options)
        {
            
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ReadingTracking> ReadingTrackings { get; set; }        
        public DbSet<Book> Books { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<BookCategory> BookCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //One-to-Many
            modelBuilder.Entity<Book>()
                .HasOne(b => b.AppUser)
                .WithMany(s => s.Books)
                .HasForeignKey(b => b.AppUserId);

            //Many-to-Many 
            modelBuilder.Entity<BookCategory>().HasKey(bc=> new {bc.BookId, bc.CategoryId});
            modelBuilder.Entity<BookCategory>().HasOne(b=>b.Book).WithMany(bc=>bc.BookCategories).HasForeignKey(b=>b.BookId);
            modelBuilder.Entity<BookCategory>().HasOne(b=>b.Category).WithMany(bc=>bc.BookCategories).HasForeignKey(b=>b.CategoryId);

            //One-to-One
            modelBuilder.Entity<Book>()
                .HasOne(b => b.ReadingTracking)
                .WithOne(rt => rt.Book)
                .HasForeignKey<ReadingTracking>(rt => rt.BookId);
                
            //One-to-One
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Review)
                .WithOne(rt => rt.Book)
                .HasForeignKey<Review>(rt => rt.BookId);

            //One-to-Many
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Status)
                .WithMany(s => s.Books)
                .HasForeignKey(b => b.StatusId);

            //identity role seeding
            List<IdentityRole> roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Name = "User",
                    NormalizedName = "USER"
                },
            };
            modelBuilder.Entity<IdentityRole>().HasData(roles);
            base.OnModelCreating(modelBuilder);
        }
    }
}
using Forum.Areas.Identity.Data;
using Forum.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Forum.Data;

public class ForumContext : IdentityDbContext<ForumUser>
{
    public ForumContext(DbContextOptions<ForumContext> options)
        : base(options)
    {
    }

    public DbSet<Models.Category> Category { get; set; } //lägg till Category i database

	public DbSet<Models.SubCategory> SubCategory { get; set; } //lägg till SubCategory i database

	public DbSet<Models.Post> Post { get; set; } //lägg till Post i database

    public DbSet<Models.Comment> Comment { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Your existing configurations

        builder.Entity<Post>()
            .HasOne(p => p.User)
            .WithMany(u => u.Posts)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict); 

        builder.Entity<Comment>()
            .HasOne(c => c.Post)
            .WithMany(p => p.Comments)
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.Cascade); 

        builder.Entity<Comment>()
            .HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

		

		

	}

}

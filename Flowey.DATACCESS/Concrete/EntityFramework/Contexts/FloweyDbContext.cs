
using Flowey.DOMAIN.Model.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Task = Flowey.DOMAIN.Model.Concrete.Task;

namespace Flowey.DATACCESS.Concrete.EntityFramework.Contexts
{
    public class FloweyDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Step> Steps { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<ProjectUserRole> ProjectUserRoles { get; set; }
        public DbSet<TaskHistory> TaskHistories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CommentAttachment> CommentAttachments { get; set; }

        public FloweyDbContext(DbContextOptions<FloweyDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FloweyDbContext).Assembly);

            modelBuilder.Entity<Task>()
               .HasOne(t => t.Step)
               .WithMany(s => s.Tasks)
               .HasForeignKey(t => t.CurrentStepId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaskHistory>(entity =>
            {
                entity.HasOne(th => th.Task)
                      .WithMany(t => t.TaskHistories) 
                      .HasForeignKey(th => th.TaskId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(th => th.Step)
                      .WithMany(s => s.TaskHistories)
                      .HasForeignKey(th => th.StepId)
                      .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(th => th.User)
                      .WithMany(u => u.TaskHistories)
                      .HasForeignKey(th => th.UserId)
                      .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin", Code = "ADMIN", IsActive = true, CreatedDate = DateTime.Now },
                new Role { Id = 2, Name = "Editor", Code = "EDITOR", IsActive = true, CreatedDate = DateTime.Now },
                new Role { Id = 3, Name = "Member", Code = "MEMBER", IsActive = true, CreatedDate = DateTime.Now }
            );

            modelBuilder.Entity<Comment>().HasQueryFilter(x => x.IsActive);
            modelBuilder.Entity<CommentAttachment>().HasQueryFilter(x => x.IsActive);
            modelBuilder.Entity<Project>().HasQueryFilter(x => x.IsActive);
            modelBuilder.Entity<ProjectUserRole>().HasQueryFilter(x => x.IsActive);
            modelBuilder.Entity<Role>().HasQueryFilter(x => x.IsActive);
            modelBuilder.Entity<Step>().HasQueryFilter(x => x.IsActive);
            modelBuilder.Entity<Task>().HasQueryFilter(x => x.IsActive);
            modelBuilder.Entity<TaskHistory>().HasQueryFilter(x => x.IsActive);
            modelBuilder.Entity<User>().HasQueryFilter(x => x.IsActive);
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Flowey.DOMAIN.Model.Concrete;
using Microsoft.EntityFrameworkCore;
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
        public DbSet<ProjectStep> ProjectSteps { get; set; }
        public DbSet<TaskStep> TaskSteps { get; set; }
        public DbSet<UserTask> UserTasks { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CommentAttachment> CommentAttachments { get; set; }

        public FloweyDbContext(DbContextOptions<FloweyDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FloweyDbContext).Assembly);

            // UserTask Relationships
            modelBuilder.Entity<UserTask>()
                .HasOne(ut => ut.User)
                .WithMany(u => u.UserTasks)
                .HasForeignKey(ut => ut.UserId);

            modelBuilder.Entity<UserTask>()
                .HasOne(ut => ut.Task)
                .WithMany(t => t.UserTasks)
                .HasForeignKey(ut => ut.TaskId);

            // ProjectStep Relationships
            modelBuilder.Entity<ProjectStep>()
                .HasOne(ps => ps.Project)
                .WithMany(p => p.ProjectSteps)
                .HasForeignKey(ps => ps.ProjectId);

            modelBuilder.Entity<ProjectStep>()
                .HasOne(ps => ps.Step)
                .WithMany(s => s.ProjectSteps)
                .HasForeignKey(ps => ps.StepId);

            // TaskStep Relationships
            modelBuilder.Entity<TaskStep>()
                .HasOne(ts => ts.Task)
                .WithMany(t => t.TaskSteps)
                .HasForeignKey(ts => ts.TaskId);

            modelBuilder.Entity<TaskStep>()
                .HasOne(ts => ts.Step)
                .WithMany(s => s.TaskSteps)
                .HasForeignKey(ts => ts.StepId);

            // ProjectUserRole Relationships
            modelBuilder.Entity<ProjectUserRole>()
                .HasOne(pur => pur.Project)
                .WithMany(p => p.ProjectUserRoles)
                .HasForeignKey(pur => pur.ProjectId);

            modelBuilder.Entity<ProjectUserRole>()
                .HasOne(pur => pur.User)
                .WithMany(u => u.ProjectUserRoles)
                .HasForeignKey(pur => pur.UserId);

            modelBuilder.Entity<ProjectUserRole>()
                .HasOne(pur => pur.Role)
                .WithMany(r => r.ProjectUserRoles)
                .HasForeignKey(pur => pur.RoleId);

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin", Code = "ADMIN", IsActive = true, CreatedDate = DateTime.Now },
                new Role { Id = 2, Name = "Editor", Code = "EDITOR", IsActive = true, CreatedDate = DateTime.Now },
                new Role { Id = 3, Name = "Member", Code = "MEMBER", IsActive = true, CreatedDate = DateTime.Now }
            );

            modelBuilder.Entity<Comment>().HasQueryFilter(x => x.IsActive);
            modelBuilder.Entity<CommentAttachment>().HasQueryFilter(x => x.IsActive);
            modelBuilder.Entity<Project>().HasQueryFilter(x => x.IsActive);
            modelBuilder.Entity<ProjectStep>().HasQueryFilter(x => x.IsActive);
            modelBuilder.Entity<ProjectUserRole>().HasQueryFilter(x => x.IsActive);
            modelBuilder.Entity<Role>().HasQueryFilter(x => x.IsActive);
            modelBuilder.Entity<Step>().HasQueryFilter(x => x.IsActive);
            modelBuilder.Entity<Task>().HasQueryFilter(x => x.IsActive);
            modelBuilder.Entity<TaskStep>().HasQueryFilter(x => x.IsActive);
            modelBuilder.Entity<User>().HasQueryFilter(x => x.IsActive);
            modelBuilder.Entity<UserTask>().HasQueryFilter(x => x.IsActive);
        }
    }
}

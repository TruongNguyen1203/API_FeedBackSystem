using System;
using Core.Entities;
using Core.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class StoreContext: IdentityDbContext<AppUser, Role, Guid>
    {
        public StoreContext(DbContextOptions<StoreContext> options) : base(options)
        {
            
        }   
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Answer>()
                .HasKey(tm => new { tm.ClassID,tm.ModuleID,tm.TraineeID });   
            modelBuilder.Entity<Trainee_Comment>()
                .HasKey(tm => new { tm.ClassID,tm.ModuleID,tm.TraineeID });      
            modelBuilder.Entity<Trainee_Assignment>()
                .HasKey(ta => new { ta.RegistrationCode,ta.TraineeID }); 
            modelBuilder.Entity<Module>()
                .HasKey(tm => new { tm.ModuleID });
             modelBuilder.Entity<Module>()
                .HasOne(pt => pt.Feedback)
                .WithMany(p => p.Modules)
                .HasForeignKey(pt => pt.FeedbackID)
                .OnDelete(DeleteBehavior.ClientCascade);
            modelBuilder.Entity<Module>()
                .HasOne(pt => pt.Admin)
                .WithMany(p => p.Modules)
                .OnDelete(DeleteBehavior.ClientCascade);
            // config many-to-many 
            modelBuilder.Entity<Feedback_Question>()
                .HasKey(fq=>new{fq.FeedbackID,fq.QuestionID});
            modelBuilder.Entity<Feedback_Question>()
                .HasOne(pt => pt.Feedback)
                .WithMany(p => p.Feedback_Questions)
                .HasForeignKey(pt => pt.FeedbackID);

            modelBuilder.Entity<Feedback_Question>()
                .HasOne(pt => pt.Question)
                .WithMany(t => t.Feedback_Questions)
                .HasForeignKey(pt => pt.QuestionID);
             modelBuilder.Entity<Enrollment>().HasKey(sc => new { sc.ClassID,sc.TraineeID}); 
           
             modelBuilder.Entity<Answer>().HasKey(aw => new {aw.ClassID,aw.ModuleID,aw.TraineeID,aw.QuestionID});
              modelBuilder.Entity<Assignment>().HasKey(aw => new {aw.ClassID,aw.ModuleID,aw.TrainerID});
        }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Trainee_Assignment> Trainee_Assignments { get; set; }
        public DbSet<Trainee_Comment> Trainee_Comments { get; set; }
        public DbSet<TypeFeedback> TypeFeedbacks { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Trainee> Trainees { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Enrollment> Enrollments {get; set;}
        public DbSet<Feedback_Question> Feedback_Questions {get; set;}
    }
}
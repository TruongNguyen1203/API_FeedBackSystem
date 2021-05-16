using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class StoreContext:DbContext
    {
        public StoreContext(DbContextOptions<StoreContext> options) : base(options)
        {
            
        }   
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Trainee_Comment>()
                .HasKey(tm => new { tm.ClassID,tm.ModuleID,tm.TraineeID });      
            modelBuilder.Entity<Trainee_Assignment>()
                .HasKey(ta => new { ta.RegistrationCode,ta.TraineeID }); 
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
        }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Trainee_Assignment> Trainee_Assignments { get; set; }
        public DbSet<Trainee_Comment> Trainee_Comments { get; set; }
        public DbSet<TypeFeedback> TypeFeedbacks { get; set; }
    }
}
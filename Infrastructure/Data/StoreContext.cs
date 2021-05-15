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
            modelBuilder.Entity<Trainee_Comment>().HasKey(tm => new { tm.ClassID,tm.ModuleID,tm.TraineeID });      
            modelBuilder.Entity<Feedback_Question>().HasKey(fq => new { fq.FeedbackID,fq.QuestionID });      
            modelBuilder.Entity<Trainee_Assignment>().HasKey(ta => new { ta.RegistrationCode,ta.TraineeID });      
        }
        public DbSet<Admin> Admin { get; set; }
        public DbSet<Feedback_Question> Feedback_Question { get; set; }
        public DbSet<Feedback> Feedback { get; set; }
        public DbSet<Module> Module { get; set; }
        public DbSet<Topic> Topic { get; set; }
        public DbSet<Trainee_Assignment> Trainee_Assignment { get; set; }
        public DbSet<Trainee_Comment> Trainee_Comment { get; set; }
        public DbSet<TypeFeedback> TypeFeedback { get; set; }
    }
}
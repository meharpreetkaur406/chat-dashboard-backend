using Microsoft.EntityFrameworkCore;
using ChatDashboard.Api.DTOs;

namespace ChatDashboard.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageTarget> MessageTargets { get; set; }
        public DbSet<UserRef> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Message>(entity =>
            {
                entity.ToTable("messages");

                entity.HasKey(e => e.MessageId);

                entity.Property(e => e.MessageId).HasColumnName("message_id");
                entity.Property(e => e.SenderId).HasColumnName("sender_id");
                entity.Property(e => e.MessageBody).HasColumnName("message_body");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            });

            modelBuilder.Entity<MessageTarget>(entity =>
            {
                entity.ToTable("message_targets");

                entity.HasKey(e => new { e.MessageId, e.TargetId });

                entity.Property(e => e.MessageId).HasColumnName("message_id");
                entity.Property(e => e.TargetId).HasColumnName("target_id");
            });

            modelBuilder.Entity<UserRef>(entity =>
            {
                entity.ToTable("user_ref");

                entity.HasKey(e => e.UserId);

                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.Role).HasColumnName("role");
            });
        }
    }
}

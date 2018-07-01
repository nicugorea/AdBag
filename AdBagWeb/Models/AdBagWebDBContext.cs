using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AdBagWeb.Models
{
    public partial class AdBagWebDBContext : DbContext
    {
        public AdBagWebDBContext()
        {
        }

        public AdBagWebDBContext(DbContextOptions<AdBagWebDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Announcement> Announcement { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Comment> Comment { get; set; }
        public virtual DbSet<User> User { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=NICUPC\\LOCALHOST;Database=AdBagWebDB;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Announcement>(entity =>
            {
                entity.HasKey(e => e.IdAnnouncement);

                entity.Property(e => e.IdAnnouncement).HasColumnName("ID_Announcement");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.ExpirationDate)
                    .HasColumnName("Expiration_Date")
                    .HasColumnType("datetime");

                entity.Property(e => e.IdCategory).HasColumnName("ID_Category");

                entity.Property(e => e.IdUser).HasColumnName("ID_User");

                entity.Property(e => e.Image).HasColumnType("image");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UploadDate)
                    .HasColumnName("Upload_Date")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.IdCategoryNavigation)
                    .WithMany(p => p.Announcement)
                    .HasForeignKey(d => d.IdCategory)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Announcements_Categories");

                entity.HasOne(d => d.IdUserNavigation)
                    .WithMany(p => p.Announcement)
                    .HasForeignKey(d => d.IdUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Announcements_Users");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.IdCategory);

                entity.Property(e => e.IdCategory).HasColumnName("ID_Category");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasKey(e => e.IdComment);

                entity.Property(e => e.IdComment).HasColumnName("ID_Comment");

                entity.Property(e => e.IdAnnouncement).HasColumnName("ID_Announcement");

                entity.Property(e => e.IdUser).HasColumnName("ID_User");

                entity.Property(e => e.PostTime).HasColumnType("datetime");

                entity.Property(e => e.Text)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.IdAnnouncementNavigation)
                    .WithMany(p => p.Comment)
                    .HasForeignKey(d => d.IdAnnouncement)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Comment_Announcement");

                entity.HasOne(d => d.IdUserNavigation)
                    .WithMany(p => p.Comment)
                    .HasForeignKey(d => d.IdUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Comment_User");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.IdUser);

                entity.Property(e => e.IdUser)
                    .HasColumnName("ID_User")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.IdUserNavigation)
                    .WithOne(p => p.InverseIdUserNavigation)
                    .HasForeignKey<User>(d => d.IdUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Users_Users");
            });
        }
    }
}

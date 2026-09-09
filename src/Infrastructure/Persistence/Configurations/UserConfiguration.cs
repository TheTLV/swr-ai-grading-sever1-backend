using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

/// <summary>
/// Cấu hình EF Core cho bảng public.users trên Supabase.
/// Schema DB là chuẩn — không đổi.
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).HasColumnName("id");

        builder.Property(u => u.FullName).HasColumnName("full_name").IsRequired().HasMaxLength(255);
        builder.Property(u => u.Birthday).HasColumnName("birthday").IsRequired();
        builder.Property(u => u.PasswordHash).HasColumnName("password_hash").IsRequired();
        builder.Property(u => u.Role).HasColumnName("role").IsRequired().HasConversion<int>();
        builder.Property(u => u.CCCD).HasColumnName("cccd").IsRequired().HasMaxLength(20);
        builder.Property(u => u.IsDeleted).HasColumnName("is_deleted").IsRequired().HasDefaultValue(false);
        builder.Property(u => u.IsActive).HasColumnName("is_active").IsRequired().HasDefaultValue(true);
        builder.Property(u => u.Discriminator).HasColumnName("Discriminator").IsRequired().HasMaxLength(50);
        builder.Property(u => u.LecturerCode).HasColumnName("lecturer_code").HasMaxLength(50);
        builder.Property(u => u.Subject).HasColumnName("subject").HasMaxLength(255);
        builder.Property(u => u.StudentCode).HasColumnName("student_code").HasMaxLength(50);
        builder.Property(u => u.Major).HasColumnName("major").HasMaxLength(255);

        // Index cho student_code và lecturer_code
        builder.HasIndex(u => u.StudentCode);
        builder.HasIndex(u => u.LecturerCode);
        builder.HasIndex(u => u.CCCD);
    }
}

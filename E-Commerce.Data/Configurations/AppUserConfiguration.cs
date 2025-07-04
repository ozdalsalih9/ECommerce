using E_Commerse.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace E_Commerce.Data.Configurations
{
    public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.Property(x => x.Name).IsRequired()
                                         .HasColumnType("varchar(50)")
                                         .HasMaxLength(50);
            builder.Property(x => x.Surname).IsRequired()
                                            .HasColumnType("varchar(50)")
                                            .HasMaxLength(50);
            builder.Property(x => x.Email).IsRequired()
                                          .HasColumnType("varchar(50)")
                                          .HasMaxLength(50);
            builder.Property(x => x.Phone).HasColumnType("varchar(20)")
                                          .HasMaxLength(20);
            builder.Property(x => x.Email).IsRequired()
                                          .HasColumnType("varchar(50)")
                                          .HasMaxLength(50);
            builder.Property(x => x.Password).IsRequired()
                                             .HasColumnType("nvarchar(256)")
                                             .HasMaxLength(256);
            builder.Property(x => x.Phone).HasColumnType("varchar(15)")
                                          .HasMaxLength(15);

            builder.HasData(
                new AppUser
                {
                    Id = 1,
                    Email = "ozdalsalih9@gmail.com",
                    IsActive = true,
                    IsAdmin = true,
                    Name = "Admin",
                    Surname ="1",
                    Password = "002255"

                }
                );

        }
    }
}

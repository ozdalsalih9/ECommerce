using E_Commerse.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Data.Configurations
{
    public class ContactConfiguration : IEntityTypeConfiguration<Contact>
    {
        public void Configure(EntityTypeBuilder<Contact> builder)
        {
            builder.Property(x => x.Name).IsRequired()
                                         .HasMaxLength(50);
            
            builder.Property(x => x.Surname).IsRequired()
                                            .HasMaxLength(50);

            builder.Property(x => x.Phone).HasMaxLength(50);


            builder.Property(x => x.Email).HasMaxLength(50);

            builder.Property(x => x.Message).IsRequired()
                                            .HasMaxLength(500);

        }
    }
}

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace SampleMvcApp.Data.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string UserIdentifier { get; set; }
        public List<Natjecanje> Natjecanja { get; set; }
    }

    public class UserConfigurationBuilder : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable(nameof(User));
            builder.HasKey(x => x.Id);
            builder.Property(x => x.UserIdentifier)
                .IsRequired();
            builder.HasIndex(x => x.UserIdentifier)
            .IsUnique();
        }
    }
}

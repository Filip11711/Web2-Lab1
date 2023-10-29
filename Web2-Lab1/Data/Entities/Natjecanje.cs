using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace SampleMvcApp.Data.Entities
{
    public class Natjecanje
    {
        public int Id { get; set; }
        public int BodoviPobjeda { get; set; }
        public string Naziv { get; set; }
        public bool IsPublic { get; set; }
        public string? PublicUrl { get; set; }
        public int BodoviRemi { get; set; }
        public int BodoviPoraz { get; set; }
        public int? UserId { get; set; }
        public User User { get; set; }
        public List<Natjecatelj> Natjecatelji { get; set; }
    }

    public class NatjecanjeConfigurationBuilder : IEntityTypeConfiguration<Natjecanje>
    {
        public void Configure(EntityTypeBuilder<Natjecanje> builder)
        {
            builder.ToTable(nameof(Natjecanje));
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Naziv)
                .IsRequired();
            builder.Property(x => x.BodoviPobjeda)
                .IsRequired();
            builder.Property(x => x.BodoviRemi)
                .IsRequired();
            builder.Property(x => x.BodoviPoraz)
                .IsRequired();
            builder.Property(x => x.IsPublic)
                .IsRequired();
            builder.Property(x => x.PublicUrl);

            builder.HasOne(x => x.User)
                .WithMany(u => u.Natjecanja)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

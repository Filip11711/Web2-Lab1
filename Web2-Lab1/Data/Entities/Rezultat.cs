using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace SampleMvcApp.Data.Entities
{
    public class Rezultat
    {
        public int Ishod { get; set; }
        public string? Vrijednost { get; set; }
        public int Kolo { get; set; }
        public int IdDomacin { get; set; }
        public int IdGost { get; set; }
        public Natjecatelj Domacin { get; set; }
        public Natjecatelj Gost { get; set; }
    }

    public class RezultatConfigurationBuilder : IEntityTypeConfiguration<Rezultat>
    {
        public void Configure(EntityTypeBuilder<Rezultat> builder)
        {
            builder.ToTable(nameof(Rezultat));
            builder.HasKey(x => new { x.IdDomacin, x.IdGost });
            builder.Property(x => x.Ishod)
                .IsRequired();
            builder.Property(x => x.Vrijednost);
            builder.Property(x => x.Kolo)
                .IsRequired();

            builder.HasOne(x => x.Domacin)
                .WithMany(d => d.DomaćiRezultati)
                .HasForeignKey(x => x.IdDomacin)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Gost)
                .WithMany(g => g.GostujućiRezultati)
                .HasForeignKey(x => x.IdGost)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

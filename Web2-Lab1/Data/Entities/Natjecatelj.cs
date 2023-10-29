using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace SampleMvcApp.Data.Entities
{
    public class Natjecatelj
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public int BrojBodova { get; set; }
        public int Razlika { get; set; }
        public int BrojOdigranihKola {  get; set; }
        public int NatjecanjeId { get; set; }
        public Natjecanje Natjecanje { get; set; }
        public List<Rezultat> DomaćiRezultati { get; set; }
        public List<Rezultat> GostujućiRezultati { get; set; }
    }

    public class NatjecateljConfigurationBuilder : IEntityTypeConfiguration<Natjecatelj>
    {
        public void Configure(EntityTypeBuilder<Natjecatelj> builder)
        {
            builder.ToTable(nameof(Natjecatelj));
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Naziv)
                .IsRequired();
            builder.Property(x => x.BrojBodova)
                .IsRequired();
            builder.Property(x => x.Razlika)
                .IsRequired();
            builder.Property(x => x.BrojOdigranihKola)
                .IsRequired();

            builder.HasOne(x => x.Natjecanje)
                .WithMany(n => n.Natjecatelji)
                .HasForeignKey(x => x.NatjecanjeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DesafioNegociaOnline.Models
{
    public partial class db_selecao_imdbContext : DbContext
    {
        public db_selecao_imdbContext()
        {
        }

        public db_selecao_imdbContext(DbContextOptions<db_selecao_imdbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Address> Address { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseNpgsql("Server=no-db-dev-101.negocieonline.com.br;Database=db_selecao_imdb;Username=test;Password=Sacapp@2020");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>(entity =>
            {
                entity.ToTable("address");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Bairro)
                    .IsRequired()
                    .HasColumnName("bairro")
                    .HasMaxLength(100);

                entity.Property(e => e.Cep)
                    .IsRequired()
                    .HasColumnName("cep")
                    .HasMaxLength(9);

                entity.Property(e => e.Complemento)
                    .HasColumnName("complemento")
                    .HasMaxLength(200);

                entity.Property(e => e.Ddd).HasColumnName("ddd");

                entity.Property(e => e.Gia).HasColumnName("gia");

                entity.Property(e => e.Ibge).HasColumnName("ibge");

                entity.Property(e => e.Localidade)
                    .IsRequired()
                    .HasColumnName("localidade")
                    .HasMaxLength(100);

                entity.Property(e => e.Logradouro)
                    .IsRequired()
                    .HasColumnName("logradouro")
                    .HasMaxLength(150);

                entity.Property(e => e.Siafi).HasColumnName("siafi");

                entity.Property(e => e.Uf)
                    .IsRequired()
                    .HasColumnName("uf")
                    .HasMaxLength(2);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

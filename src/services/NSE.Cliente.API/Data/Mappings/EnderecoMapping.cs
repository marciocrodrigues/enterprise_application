﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSE.Clientes.API.Models;

namespace NSE.Clientes.API.Data.Mappings
{
    public class EnderecoMapping : IEntityTypeConfiguration<Endereco>
    {
        public void Configure(EntityTypeBuilder<Endereco> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Logradouro)
                .IsRequired()
                .HasColumnType("varchar(200)");

            builder.Property(e => e.Numero)
                .IsRequired()
                .HasColumnType("varchar(50)");

            builder.Property(e => e.Cep)
                .IsRequired()
                .HasColumnType("varchar(20)");

            builder.Property(e => e.Complemento)
                .HasColumnType("varchar(250)");

            builder.Property(e => e.Bairro)
                .HasColumnType("varchar(100)");

            builder.Property(e => e.Cidade)
                .HasColumnType("varchar(100)");

            builder.Property(e => e.Estado)
                .HasColumnType("varchar(50)");

            builder.ToTable("Enderecos");
        }
    }
}

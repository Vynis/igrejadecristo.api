using CursoIgreja.Domain.Models.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CursoIgreja.Repository.Mapping
{
    public class VwRelatorioInscricoesMap : IEntityTypeConfiguration<VwRelatorioInscricoes>
    {
        public void Configure(EntityTypeBuilder<VwRelatorioInscricoes> builder)
        {
            builder.ToTable("vw_relatorio_inscricoes");
            builder.HasKey(c => c.IdUsuario);
            builder.HasKey(c => c.IdInscricao);
        }
    }
}

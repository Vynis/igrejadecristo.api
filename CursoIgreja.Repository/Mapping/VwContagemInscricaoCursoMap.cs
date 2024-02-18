using CursoIgreja.Domain.Models.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CursoIgreja.Repository.Mapping
{
    public class VwContagemInscricaoCursoMap : IEntityTypeConfiguration<VwContagemInscricoesCurso>
    {
        public void Configure(EntityTypeBuilder<VwContagemInscricoesCurso> builder)
        {
            builder.ToTable("vw_contagem_inscricao_curso");
            builder.HasKey(c => c.Id);
        }
    }
}

using CursoIgreja.Domain.Models.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CursoIgreja.Repository.Mapping
{
    public class VwContagemInscricoesCongregacaoMap : IEntityTypeConfiguration<VwContagemInscricoes>
    {
        public void Configure(EntityTypeBuilder<VwContagemInscricoes> builder)
        {
            builder.ToTable("vw_contagem_inscricao");
            builder.HasKey(c => c.Id);
        }
    }
}

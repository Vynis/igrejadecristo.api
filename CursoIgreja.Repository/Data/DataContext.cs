using CursoIgreja.Domain.Models;
using CursoIgreja.Domain.Models.Views;
using CursoIgreja.Repository.Mapping;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CursoIgreja.Repository.Data
{
    public class DataContext : DbContext
    {

        public DataContext(DbContextOptions<DataContext> options) : base(options)  
        {  
        }

        public DbSet<Usuarios> Usuarios { get; set; }
        public DbSet<Congregacao> Congregacao { get; set; }
        public DbSet<Curso> Cursos { get; set; }
        public DbSet<ProcessoInscricao> ProcessoInscricoes { get; set; }
        public DbSet<InscricaoUsuario> InscricaoUsuario { get; set; }
        public DbSet<MeioPagamento> MeiosPagamentos { get; set; }
        public DbSet<ParametroSistema> ParametroSistema { get; set; }
        public DbSet<TransacaoInscricao> TransacaoInscricoes { get; set; }
        public DbSet<LogNotificacao> LogNotificacoes { get; set; }
        public DbSet<LogUsuario> LogUsuarios { get; set; }
        public DbSet<Modulo> Modulos { get; set; }
        public DbSet<Conteudo> Conteudos { get; set; }
        public DbSet<Anexo> Anexos { get; set; }
        public DbSet<ConteudoUsuario> ConteudoUsuarios { get; set; }
        public DbSet<Prova> Provas { get; set; }
        public DbSet<ItemProva> ItemProvas { get; set; }
        public DbSet<ProvaUsuario> ProvaUsuarios { get; set; }
        public DbSet<ItemProvaUsuario> ItemProvaUsuarios { get; set; }
        public DbSet<LiberacaoModulo> LiberacaoModulos { get; set; }
        public DbSet<UsuarioSistema> UsuarioSistemas { get; set; }
        public DbSet<Perfil> Perfis { get; set; }
        public DbSet<Permissoes> Permissoes { get; set; }
        public DbSet<UsuarioPerfis> UsuarioPerfis { get; set; }
        public DbSet<PerfilPermissoes> PerfilPermissoes { get; set; }
        public DbSet<Professor> Professores { get; set; }
        public DbSet<CursoProfessor> CursoProfessores { get; set; }
        public DbSet<Membro> Membros { get; set; }
        public DbSet<VwContagemInscricoes> VwContagemInscricoes { get; set; }
        public DbSet<VwContagemInscricoesCurso> VwContagemInscricoesCursos { get; set; }
        public DbSet<InscricaoLiberarCurso> InscricaoLiberarCursos { get; set; }
        public DbSet<GeolocalizacaoUsuario> GeolocalizacaoUsuarios { get; set; }
        public DbSet<PresencaUsuario> PresencaUsuarios { get; set; }
        public DbSet<VwRelatorioInscricoes> ListaVwRelatorioInscricoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Retira o delete on cascade
            var cascadeFKs = modelBuilder.Model.GetEntityTypes()
            .SelectMany(t => t.GetForeignKeys())
            .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
                fk.DeleteBehavior = DeleteBehavior.Restrict;  

            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new VwContagemInscricoesCongregacaoMap());
            modelBuilder.ApplyConfiguration(new VwContagemInscricaoCursoMap());
            modelBuilder.ApplyConfiguration(new VwRelatorioInscricoesMap());

        }

    }
}

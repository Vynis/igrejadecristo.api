using CursoIgreja.Domain.Models;
using CursoIgreja.Repository.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace CursoIgreja.Api.Services
{
    public static class AdminSecuritySeedService
    {
        private static readonly string[] Perfis =
        {
            "Administrador",
            "Secretaria",
            "Professor",
            "Financeiro",
            "Consulta"
        };

        private static readonly (string Titulo, string Chave)[] Permissoes =
        {
            ("Dashboard - Visualizar", "dashboard.visualizar"),
            ("Alunos - Visualizar", "alunos.visualizar"),
            ("Alunos - Criar", "alunos.criar"),
            ("Alunos - Editar", "alunos.editar"),
            ("Alunos - Resetar senha", "alunos.resetar_senha"),
            ("Alunos - Visualizar inscrições", "alunos.visualizar_inscricoes"),
            ("Usuários do Sistema - Visualizar", "usuariosistema.visualizar"),
            ("Usuários do Sistema - Criar", "usuariosistema.criar"),
            ("Usuários do Sistema - Editar", "usuariosistema.editar"),
            ("Usuários do Sistema - Resetar senha", "usuariosistema.resetar_senha"),
            ("Cursos - Visualizar", "cursos.visualizar"),
            ("Cursos - Criar", "cursos.criar"),
            ("Cursos - Editar", "cursos.editar"),
            ("Processo de Inscrição - Visualizar", "processoinscricao.visualizar"),
            ("Processo de Inscrição - Criar", "processoinscricao.criar"),
            ("Processo de Inscrição - Editar", "processoinscricao.editar"),
            ("Processo de Inscrição - Alunos inscritos", "processoinscricao.alunos_inscritos"),
            ("Processo de Inscrição - Lançar resultado", "processoinscricao.lancar_resultado"),
            ("Processo de Inscrição - Liberar cursos", "processoinscricao.liberar_cursos"),
            ("Presença - Visualizar", "presenca.visualizar"),
            ("Presença - Lançar", "presenca.lancar"),
            ("Presença - Remover", "presenca.remover"),
            ("Relatórios - Visualizar", "relatorios.visualizar"),
            ("Relatórios - Inscrições", "relatorios.inscricoes"),
            ("Relatórios - Presença", "relatorios.presenca"),
            ("Relatórios - Evolução dos Alunos", "relatorios.evolucao_alunos"),
            ("Permissões - Visualizar", "permissoes.visualizar"),
            ("Permissões - Editar", "permissoes.editar")
        };

        private static readonly Dictionary<string, string[]> PermissoesPorPerfil = new Dictionary<string, string[]>
        {
            { "Secretaria", new[]
                {
                    "dashboard.visualizar",
                    "alunos.visualizar",
                    "alunos.criar",
                    "alunos.editar",
                    "alunos.resetar_senha",
                    "alunos.visualizar_inscricoes",
                    "processoinscricao.visualizar",
                    "processoinscricao.alunos_inscritos",
                    "relatorios.visualizar",
                    "relatorios.inscricoes",
                    "relatorios.evolucao_alunos"
                }
            },
            { "Professor", new[]
                {
                    "dashboard.visualizar",
                    "alunos.visualizar",
                    "processoinscricao.visualizar",
                    "processoinscricao.alunos_inscritos",
                    "processoinscricao.lancar_resultado",
                    "presenca.visualizar",
                    "presenca.lancar",
                    "relatorios.visualizar",
                    "relatorios.presenca"
                }
            },
            { "Financeiro", new[]
                {
                    "dashboard.visualizar",
                    "alunos.visualizar",
                    "processoinscricao.visualizar",
                    "relatorios.visualizar",
                    "relatorios.inscricoes"
                }
            },
            { "Consulta", new[]
                {
                    "dashboard.visualizar",
                    "alunos.visualizar",
                    "processoinscricao.visualizar",
                    "relatorios.visualizar",
                    "relatorios.inscricoes",
                    "relatorios.presenca",
                    "relatorios.evolucao_alunos"
                }
            }
        };

        public static void Seed(DataContext dataContext)
        {
            CriarTabelasSeNaoExistirem(dataContext);
            CriarPerfis(dataContext);
            CriarPermissoes(dataContext);
            VincularPermissoes(dataContext);
        }

        private static void CriarTabelasSeNaoExistirem(DataContext dataContext)
        {
            dataContext.Database.ExecuteSqlRaw(@"
                CREATE TABLE IF NOT EXISTS perfil (
                  Id INT NOT NULL AUTO_INCREMENT,
                  Titulo VARCHAR(150) NOT NULL,
                  PRIMARY KEY (Id)
                );");

            dataContext.Database.ExecuteSqlRaw(@"
                CREATE TABLE IF NOT EXISTS permissoes (
                  Id INT NOT NULL AUTO_INCREMENT,
                  Titulo VARCHAR(150) NOT NULL,
                  Chave VARCHAR(150) NOT NULL,
                  Nivel INT NOT NULL DEFAULT 1,
                  IdPai INT NOT NULL DEFAULT 0,
                  PRIMARY KEY (Id)
                );");

            dataContext.Database.ExecuteSqlRaw(@"
                CREATE TABLE IF NOT EXISTS usuarioperfis (
                  Id INT NOT NULL AUTO_INCREMENT,
                  PerfilId INT NOT NULL,
                  UsuarioSistemaId INT NOT NULL,
                  PRIMARY KEY (Id),
                  INDEX IX_usuarioperfis_PerfilId (PerfilId),
                  INDEX IX_usuarioperfis_UsuarioSistemaId (UsuarioSistemaId)
                );");

            dataContext.Database.ExecuteSqlRaw(@"
                CREATE TABLE IF NOT EXISTS perfilpermissoes (
                  Id INT NOT NULL AUTO_INCREMENT,
                  PefilId INT NOT NULL,
                  PermissoesId INT NOT NULL,
                  PRIMARY KEY (Id),
                  INDEX IX_perfilpermissoes_PefilId (PefilId),
                  INDEX IX_perfilpermissoes_PermissoesId (PermissoesId)
                );");
        }

        private static void CriarPerfis(DataContext dataContext)
        {
            foreach (var perfil in Perfis)
            {
                if (!dataContext.Perfis.Any(x => x.Titulo == perfil))
                    dataContext.Perfis.Add(new Perfil { Titulo = perfil });
            }

            dataContext.SaveChanges();
        }

        private static void CriarPermissoes(DataContext dataContext)
        {
            foreach (var permissao in Permissoes)
            {
                if (!dataContext.Permissoes.Any(x => x.Chave == permissao.Chave))
                {
                    dataContext.Permissoes.Add(new Permissoes
                    {
                        Titulo = permissao.Titulo,
                        Chave = permissao.Chave,
                        Nivel = 1,
                        IdPai = 0
                    });
                }
            }

            dataContext.SaveChanges();
        }

        private static void VincularPermissoes(DataContext dataContext)
        {
            var todasPermissoes = dataContext.Permissoes.ToList();
            VincularPerfil(dataContext, "Administrador", todasPermissoes.Select(x => x.Chave).ToArray());

            foreach (var permissaoPerfil in PermissoesPorPerfil)
                VincularPerfil(dataContext, permissaoPerfil.Key, permissaoPerfil.Value);

            dataContext.SaveChanges();
        }

        private static void VincularPerfil(DataContext dataContext, string perfilTitulo, string[] chaves)
        {
            var perfil = dataContext.Perfis.FirstOrDefault(x => x.Titulo == perfilTitulo);

            if (perfil == null)
                return;

            var permissoes = dataContext.Permissoes.Where(x => chaves.Contains(x.Chave)).ToList();

            foreach (var permissao in permissoes)
            {
                if (!dataContext.PerfilPermissoes.Any(x => x.PefilId == perfil.Id && x.PermissoesId == permissao.Id))
                {
                    dataContext.PerfilPermissoes.Add(new PerfilPermissoes
                    {
                        PefilId = perfil.Id,
                        PermissoesId = permissao.Id
                    });
                }
            }
        }
    }
}

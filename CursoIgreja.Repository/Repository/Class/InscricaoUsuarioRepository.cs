    using CursoIgreja.Domain.Models;
using CursoIgreja.PagSeguro.TransferObjects;
using CursoIgreja.Repository.Data;
using CursoIgreja.Repository.Repository.Interfaces;
using FiltrDinamico.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CursoIgreja.Repository.Repository.Class
{
    public class InscricaoUsuarioRepository : RepositoryBase<InscricaoUsuario>, IInscricaoUsuarioRepository
    {
        private readonly DataContext _dataContext;

        public InscricaoUsuarioRepository(DataContext dataContext, IFiltroDinamico filtroDinamico) : base(dataContext, filtroDinamico)
        {
            _dataContext = dataContext;
        }

        public async override Task<InscricaoUsuario[]> Buscar(Expression<Func<InscricaoUsuario, bool>> predicado)
        {
            IQueryable<InscricaoUsuario> query = _dataContext.InscricaoUsuario
                                                                .Where(predicado)
                                                                .Include(c => c.Usuario)
                                                                .Include(c => c.ProcessoInscricao)
                                                                .Include(c => c.ProcessoInscricao.Curso)
                                                                .Include(c => c.TransacaoInscricoes);

            return await query.AsNoTracking().ToArrayAsync();
        }

        public async override Task<InscricaoUsuario[]> ObterTodos()
        {
            IQueryable<InscricaoUsuario> query = _dataContext.InscricaoUsuario
                                                                .Include(c => c.Usuario)
                                                                .Include(c => c.ProcessoInscricao)
                                                                .Include(c => c.TransacaoInscricoes);

            return await query.AsNoTracking().ToArrayAsync();
        }

        public async override Task<InscricaoUsuario> ObterPorId(int id)
        {
            IQueryable<InscricaoUsuario> query = _dataContext.InscricaoUsuario
                                                                .Include(c => c.Usuario)
                                                                .Include(c => c.ProcessoInscricao)
                                                                .Include(c => c.ProcessoInscricao.Curso)
                                                                .Include(c => c.ProcessoInscricao.Curso.Modulo)
                                                                .Include(c => c.Usuario.ProvaUsuarios)
                                                                .Include("ProcessoInscricao.Curso.Modulo.Conteudos")
                                                                .Include("ProcessoInscricao.Curso.Modulo.LiberacaoModulos")
                                                                .Include("ProcessoInscricao.Curso.Modulo.Conteudos.Anexos")
                                                                .Include("ProcessoInscricao.Curso.Modulo.Conteudos.ConteudoUsuarios")
                                                                .Include("ProcessoInscricao.Curso.Modulo.Conteudos.Provas")
                                                                .Include("ProcessoInscricao.Curso.Modulo.Conteudos.Provas.ItensProvas")
                                                                .Include(c => c.TransacaoInscricoes);

            return await query.Where(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> AtualizaGeral(List<ConsultaTransacaoPagSeguroTransactionDTO> listaNotificacoes, int idProcessoInscricao)
        {
            var dadosInscricao = await _dataContext.InscricaoUsuario.Where(x => x.ProcessoInscricaoId == idProcessoInscricao).AsNoTracking().ToListAsync();

            var listaDadosInscritos = new List<InscricaoUsuario>();

            foreach (var dados in dadosInscricao)
            {
                var dadosPagSeguro = listaNotificacoes.Where(x => x.Reference.Equals(dados.Id.ToString())).FirstOrDefault();

                if (dadosPagSeguro != null)
                {
                    dados.MeioPagamento = dadosPagSeguro.PaymentMethodType;
                    dados.MeioPagamentoDesc = DescricaoTipoPagamento(dadosPagSeguro.PaymentMethodType);
                    dados.ValorBruto = Convert.ToDecimal(dadosPagSeguro.GrossAmount);
                    dados.ValorLiquido = Convert.ToDecimal(dadosPagSeguro.NetAmount);
                    dados.QtdParcelas = dadosPagSeguro.installmentCount;

                    listaDadosInscritos.Add(dados);
                }
            }

            return await base.AtualizarRange(listaDadosInscritos.ToArray());
        }

        public static string DescricaoTipoPagamento(int Tipo)
        {
            switch (Tipo)
            {
                case 1:
                    return "Cartão de crédito";
                case 2:
                    return "Boleto";
                case 3:
                    return "Débito online (TEF)";
                case 4:
                    return "Saldo PagSeguro";
                case 5:
                    return "Oi Paggo";
                case 7:
                    return "Depósito em conta";
                case 8:
                    return "Cartão Emergencial Caixa (Débito)";
                case 11:
                    return "PIX";
                default:
                    return "";
            }
        }
    }
}

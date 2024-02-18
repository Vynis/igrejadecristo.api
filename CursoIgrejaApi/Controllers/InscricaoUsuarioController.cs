using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CursoIgreja.Api.Dtos;
using CursoIgreja.Domain.Models;
using CursoIgreja.PagSeguro;
using CursoIgreja.PagSeguro.Enum;
using CursoIgreja.PagSeguro.TransferObjects;
using CursoIgreja.PagSeguroApi;
using CursoIgreja.PagSeguroApi.Model;
using CursoIgreja.Repository.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CursoIgreja.Api.Controllers
{
    [ApiController]
    [Route("inscricao-usuario")]
    public class InscricaoUsuarioController : ControllerBase
    {
        private readonly IInscricaoUsuarioRepository _inscricaoUsuarioRepository;
        private readonly IMeioPagamentoRepository _meioPagamentoRepository;
        private readonly ICursoRepository _cursoRepository;
        private readonly IParametroSistemaRepository _parametroSistemaRepository;
        private readonly ITransacaoInscricaoRepository _transacaoInscricaoRepository;
        private readonly ILogNotificacoesRepository _logNotificacoesRepository;
        private readonly IProcessoInscricaoRepository _processoInscricaoRepository;
        private readonly IProvaUsuarioRepository _provaUsuarioRepository;
        private readonly IInscricaoLiberarCursoRepository _inscricaoLiberarCursoRepository;
        private string urlWsPagueSeguro = "";
        private string urlSitePagueSeguro = "";
        private string urlApiPagueSeguro = "";
        private string urlRedirectUrl = "";
        private string urlNotificationUrl = "";

        public InscricaoUsuarioController(IInscricaoUsuarioRepository inscricaoUsuarioRepository,
                                          IMeioPagamentoRepository meioPagamentoRepository,
                                          ICursoRepository cursoRepository,
                                          IParametroSistemaRepository parametroSistemaRepository,
                                          ITransacaoInscricaoRepository transacaoInscricaoRepository,
                                          ILogNotificacoesRepository logNotificacoesRepository,
                                          IProcessoInscricaoRepository processoInscricaoRepository,
                                          IProvaUsuarioRepository provaUsuarioRepository,
                                          IInscricaoLiberarCursoRepository inscricaoLiberarCursoRepository
                                          )
        {
            _inscricaoUsuarioRepository = inscricaoUsuarioRepository;
            _meioPagamentoRepository = meioPagamentoRepository;
            _cursoRepository = cursoRepository;
            _parametroSistemaRepository = parametroSistemaRepository;
            _transacaoInscricaoRepository = transacaoInscricaoRepository;
            _logNotificacoesRepository = logNotificacoesRepository;
            _processoInscricaoRepository = processoInscricaoRepository;
            _provaUsuarioRepository = provaUsuarioRepository;
            _inscricaoLiberarCursoRepository = inscricaoLiberarCursoRepository;
            urlSitePagueSeguro = _parametroSistemaRepository.Buscar(x => x.Status.Equals("A") && x.Titulo.Equals("SitePagueSeguro")).Result.FirstOrDefault().Valor;
            urlWsPagueSeguro = _parametroSistemaRepository.Buscar(x => x.Status.Equals("A") && x.Titulo.Equals("WsPagueSeguro")).Result.FirstOrDefault().Valor;
            urlApiPagueSeguro = _parametroSistemaRepository.Buscar(x => x.Status.Equals("A") && x.Titulo.Equals("ApiPagueSeguro")).Result.FirstOrDefault().Valor;
            urlRedirectUrl = _parametroSistemaRepository.Buscar(x => x.Status.Equals("A") && x.Titulo.Equals("RedirectUrl")).Result.FirstOrDefault().Valor;
            urlNotificationUrl = _parametroSistemaRepository.Buscar(x => x.Status.Equals("A") && x.Titulo.Equals("NotificationUrl")).Result.FirstOrDefault().Valor;
        }


        [HttpGet("processar-curso-inscrito/{id}")]
        [Obsolete("Nao esta sendo utilizado neste modulo. Olhar CursoController")]
        public async Task<IActionResult> ProcessarCursoInscrito(int id)
        {
            try
            {
                var response = await _inscricaoUsuarioRepository.ObterPorId(id);

                if (response.UsuarioId != Convert.ToInt32(User.Identity.Name))
                    return Response("Busca invalida", false);

                var listaProvaUsuario = await _provaUsuarioRepository.Buscar(x => x.UsuarioId.Equals(Convert.ToInt32(User.Identity.Name)));

                foreach (var modulo in response.ProcessoInscricao.Curso.Modulo)
                   foreach (var conteudo in modulo.Conteudos)
                    {
                        if (conteudo.Tipo.Equals("PR") || conteudo.Tipo.Equals("PA"))
                        {
                            var provaUsuario = listaProvaUsuario.Where(x => x.Prova.ConteudoId.Equals(conteudo.Id)).ToList();

                            if (provaUsuario.Count > 0)
                                conteudo.ConteudoConcluido = true;
                            else
                                conteudo.ConteudoConcluido = false;
                        }
                        else
                            conteudo.ConteudoConcluido = conteudo.ConteudoUsuarios.Exists(x => x.ConteudoId == conteudo.Id && x.UsuariosId == Convert.ToInt32(User.Identity.Name) && x.Concluido.Equals("S"));
                    }
                       

               return Response(response);
                
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpPost("cadastrar")]
        public async Task<IActionResult> Cadastrar(InscricaoUsuario inscricaoUsuario)
        {
            try
            {
                var processoInscricao = await _processoInscricaoRepository.ObterPorId(inscricaoUsuario.ProcessoInscricaoId);

                var usuarioIncritos = (await _inscricaoUsuarioRepository.Buscar(x => x.ProcessoInscricaoId.Equals(inscricaoUsuario.ProcessoInscricaoId) && x.Status != "CA")).Count();

                var validaInscricao = await _inscricaoUsuarioRepository.Buscar(x => x.ProcessoInscricaoId.Equals(inscricaoUsuario.ProcessoInscricaoId)
                && x.UsuarioId.Equals(Convert.ToInt32(User.Identity.Name)) && x.Status != "CA");
          
                if (validaInscricao.Any())
                    return Response("Já se encontra inscrito neste curso", false);

                if (processoInscricao.LimiteVagas != 0)
                 if (usuarioIncritos > processoInscricao.LimiteVagas)
                    return Response("Limite de vagas excedido.", false);

                var listaLiberacaoCurso = await _inscricaoLiberarCursoRepository.Buscar(x => x.ProcessoInscricoeId == inscricaoUsuario.ProcessoInscricaoId);

                if (listaLiberacaoCurso.Any())
                {
                    foreach(var item in listaLiberacaoCurso)
                    {
                        var processoInscricaoLiberar = await _processoInscricaoRepository.Buscar(x => x.CursoId == item.CursoId);

                        if (!processoInscricaoLiberar.Any())
                            return Response("Não é possível se inscrever para este curso.", false);

                        bool validaInscricao2 = false;

                        foreach (var processo in processoInscricaoLiberar)
                        {
                            var valida = await _inscricaoUsuarioRepository.Buscar(x => x.ProcessoInscricaoId.Equals(processo.Id)
                                                         && x.UsuarioId.Equals(Convert.ToInt32(User.Identity.Name)) && x.Status != "CA" && x.StatusEstudo.Equals("AP"));

                            if (valida.Count() > 0)
                            {
                                validaInscricao2 = true;
                                break;
                            }
                                
                        }

                        if (!validaInscricao2)
                            return Response("Não é possível se inscrever para este curso.", false);

                    }

                }

                inscricaoUsuario.DataInscricao = DateTime.Now;
                inscricaoUsuario.Usuario = null;
                inscricaoUsuario.ProcessoInscricao = null;
                inscricaoUsuario.UsuarioId = Convert.ToInt32(User.Identity.Name);

                var response = await _inscricaoUsuarioRepository.Adicionar(inscricaoUsuario);

                if (response)
                {
                    inscricaoUsuario.ProcessoInscricao = await _processoInscricaoRepository.ObterPorId(inscricaoUsuario.ProcessoInscricaoId);
                    return Response(inscricaoUsuario);
                }
                    

                return Response("Cadastro não realizado", false);
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpPut("cancelar-incricao/{id}")]
        public async Task<IActionResult> CancelarInscricao(int id)
        {
            try
            {
                var buscarInscricao = await _inscricaoUsuarioRepository.ObterPorId(id);

                if (buscarInscricao == null)
                    return Response("Inscrição nao existe", false);

                buscarInscricao.Status = "CA";

                var response = await _inscricaoUsuarioRepository.Atualizar(buscarInscricao);

                if (!response)
                    return Response("Erro ao atualizar", false);

                return Response(buscarInscricao);

            }
            catch (Exception ex)
            {

                return ResponseErro(ex);
            }
        }

        [HttpGet("buscar-transacao/{idTransacao}")]
        public async Task<IActionResult> BuscarTransacao(string idTransacao)
        {

            try
            {
                var paymentUrl = string.Concat($"{urlSitePagueSeguro}/v2/checkout/payment.html?code=", idTransacao);

                return Response(paymentUrl);
            }
            catch (Exception ex)
            {

                return ResponseErro(ex);
            }

        }

        [HttpGet("gerar-sessao")]
        public async Task<IActionResult> GerarSessao()
        {
            try
            {
                var dadosConfigPagamento = await _meioPagamentoRepository.Buscar(x => x.Status.Equals("A"));
                string urlCheckout = $"{urlWsPagueSeguro}/v2/sessions";
                var emailPagSeguro = dadosConfigPagamento.FirstOrDefault().Email;
                var tokenPagSeguro = dadosConfigPagamento.FirstOrDefault().Token;

                var apiPagSeguro = new PagSeguroAPI();

                var retorno = apiPagSeguro.GetSession(emailPagSeguro, tokenPagSeguro, urlCheckout);

                return Response(new { idSession = retorno });
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }


        [HttpPost("gerar/{id}/{metodoPagto}")]
        public async Task<IActionResult> Gerar(int id, string metodoPagto)
        {
            try
            {
                var inscricao = await _inscricaoUsuarioRepository.ObterPorId(id);

                if (inscricao == null)
                    return Response("Inscrição não localizada!", false);

                if (inscricao.TransacaoInscricoes.Any())
                    await _transacaoInscricaoRepository.RemoverRange(inscricao.TransacaoInscricoes.ToArray());

                inscricao.TransacaoInscricoes = new List<TransacaoInscricao>();

                var dadosConfigPagamento = await _meioPagamentoRepository.Buscar(x => x.Status.Equals("A"));

                var dadosCurso = await _cursoRepository.ObterPorId(inscricao.ProcessoInscricao.CursoId);

                var emailPagSeguro = dadosConfigPagamento.FirstOrDefault().Email;
                var tokenPagSeguro = dadosConfigPagamento.FirstOrDefault().Token;
                string urlCheckout = $"{urlApiPagueSeguro}/checkouts";

                var dadosPagamento = new PagSeguroModel.PagSeguro();

                dadosPagamento.reference_id = id.ToString();

                dadosPagamento.items = new List<PagSeguroModel.Item>
                {
                    new PagSeguroModel.Item
                    {
                        reference_id = dadosCurso.Id.ToString(),
                        name = $"Inscrição para o curso: {dadosCurso.Titulo}",
                        quantity = 1,
                        unit_amount = metodoPagto.Equals("CREDIT_CARD") ? Convert.ToInt32(inscricao.ProcessoInscricao.Valor.ToString("F").Replace(".","").Replace(",","")) : Convert.ToInt32(inscricao.ProcessoInscricao.ValorPixBoleto?.ToString("F").Replace(".","").Replace(",",""))                     
                    }
                };

                dadosPagamento.payment_methods = new List<PagSeguroModel.Payment_Methods>
                {
                    new PagSeguroModel.Payment_Methods
                    {
                        type = metodoPagto
                    }
                };

                if (metodoPagto.Equals("CREDIT_CARD"))
                {
                    dadosPagamento.payment_methods_configs = new List<PagSeguroModel.Payment_Methods_Configs>
                    {
                        new PagSeguroModel.Payment_Methods_Configs
                        {
                            type = "CREDIT_CARD",
                            config_options = new List<PagSeguroModel.Config_Options>
                            {
                                new PagSeguroModel.Config_Options
                                {
                                    option = "INSTALLMENTS_LIMIT",
                                    value = "3"
                                }
                            }
                        }
                    };
                }

                if (string.IsNullOrEmpty(inscricao.Usuario.Email))
                    inscricao.Usuario.Email = "sememail@cursoigrejacristobrasil.kinghost.net";

                dadosPagamento.customer = new PagSeguroModel.Customer
                {
                    name = inscricao.Usuario.Nome.Length > 50 ? inscricao.Usuario.Nome.Substring(0, 50).Trim() : inscricao.Usuario.Nome.Trim(),
                    email = inscricao.Usuario.Email.Length > 60 ? inscricao.Usuario.Nome.Substring(0, 60).ToLower().Trim() : inscricao.Usuario.Email.ToLower().Trim()
                };

                dadosPagamento.redirect_url = urlRedirectUrl;
                //dadosPagamento.notification_urls = new string[] { urlNotificationUrl };
                //dadosPagamento.payment_notification_urls = new string[] { urlNotificationUrl };

                var apiPagSeguro = new PagSeguroCheckoutApi();

                var retorno = apiPagSeguro.checkout(urlCheckout, dadosPagamento, tokenPagSeguro);

                var conversaoJson = JsonConvert.SerializeObject(retorno);

                var logNotificacoes = new LogNotificacao()
                {
                    Xml = conversaoJson,
                    Data = DateTime.Now
                };

                var response = await _logNotificacoesRepository.Adicionar(logNotificacoes);

                return Response(retorno.links.Where(x => x.rel.Equals("PAY")).FirstOrDefault().href);

            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }
        //public async Task<IActionResult> Gerar(int id,string metodoPagto)
        //{
        //    try
        //    {
        //        var inscricao = await _inscricaoUsuarioRepository.ObterPorId(id);

        //        if (inscricao == null)
        //            return Response("Inscrição não localizada!", false);

        //        if (inscricao.TransacaoInscricoes.Any())
        //            await _transacaoInscricaoRepository.RemoverRange(inscricao.TransacaoInscricoes.ToArray());

        //        inscricao.TransacaoInscricoes = new List<TransacaoInscricao>();

        //        var dadosConfigPagamento = await _meioPagamentoRepository.Buscar(x => x.Status.Equals("A"));

        //        var dadosCurso = await _cursoRepository.ObterPorId(inscricao.ProcessoInscricao.CursoId);

        //        var emailPagSeguro = dadosConfigPagamento.FirstOrDefault().Email;
        //        var tokenPagSeguro = dadosConfigPagamento.FirstOrDefault().Token;
        //        string urlCheckout = $"{urlWsPagueSeguro}/v2/checkout";

        //        var listaItensPedido = new List<PagSeguroItemDTO> {
        //            new PagSeguroItemDTO {
        //                itemId = id.ToString(),
        //                itemQuantity = "1",
        //                itemDescription = $"Inscrição para o curso: {dadosCurso.Titulo}",
        //                itemAmount = metodoPagto.Equals("CREDIT_CARD") ? inscricao.ProcessoInscricao.Valor.ToString("F").Replace(",",".") : inscricao.ProcessoInscricao.ValorPixBoleto?.ToString("F").Replace(",",".") ,
        //                itemWeight = "200"
        //            }
        //        };

        //        if (string.IsNullOrEmpty(inscricao.Usuario.Email))
        //            inscricao.Usuario.Email = "sememail@cursoigrejacristobrasil.kinghost.net";

        //        var dadosComprador = new PagSeguroCompradorDTO
        //        {
        //            SenderName = inscricao.Usuario.Nome.Length > 50 ? inscricao.Usuario.Nome.Substring(0, 50).Trim() : inscricao.Usuario.Nome.Trim(),
        //            senderEmail = inscricao.Usuario.Email.Length > 60 ? inscricao.Usuario.Nome.Substring(0, 60).ToLower().Trim() : inscricao.Usuario.Email.ToLower().Trim(),
        //            senderPhone = inscricao.Usuario.TelefoneCelular.Length > 9 ? inscricao.Usuario.TelefoneCelular.Substring(0, 9).Trim() : inscricao.Usuario.TelefoneCelular.Trim(),
        //            SenderAreaCode = "62"
        //        };

        //        var referencia = id.ToString();

        //        var apiPagSeguro = new PagSeguroAPI();

        //        //Retira os espaçõs entre o nome
        //        Regex regex = new Regex(@"\s{2,}");
        //        dadosComprador.SenderName = regex.Replace(dadosComprador.SenderName, " ");
        //        dadosComprador.SenderName = dadosComprador.SenderName.Replace("\0", "");

        //        var retornoApiPagSeguro = apiPagSeguro.Checkout(emailPagSeguro, tokenPagSeguro, urlCheckout, listaItensPedido, dadosComprador, referencia, metodoPagto);

        //        if (!string.IsNullOrEmpty(retornoApiPagSeguro))
        //        {
        //            var transacao = new TransacaoInscricao()
        //            {
        //                Codigo = retornoApiPagSeguro,
        //                InscricaoUsuarioId = id
        //            };

        //            var registraTransacao = await _transacaoInscricaoRepository.Adicionar(transacao);

        //            if (!registraTransacao)
        //                return Response("Erro na transação de pagamento", false);
        //        } else
        //        {
        //            return Response("Erro ao fazer a comunicação de pagamento", false);
        //        }

        //        var paymentUrl = string.Concat($"{urlSitePagueSeguro}/v2/checkout/payment.html?code=", retornoApiPagSeguro);

        //        return Response(paymentUrl);

        //    }
        //    catch (Exception ex)
        //    {
        //        return ResponseErro(ex);
        //    }
        //}

        [HttpGet("busca-curso-inscrito")]
        public async Task<IActionResult> BuscarCursosInscrito()
        {
            try
            {
                var resultado = await _inscricaoUsuarioRepository.Buscar(x => x.UsuarioId == Convert.ToInt32(User.Identity.Name) && !x.Status.Equals("CA"));

                return Response(resultado.OrderByDescending(c => c.DataInscricao));
            }
            catch (Exception ex)
            {

                return ResponseErro(ex);
            }
        }

        [HttpPost("notificacoes")]
        [AllowAnonymous]
        public async Task<IActionResult> Notificacoes(PagSeguroNotificacoesModel.PagSeguroNotificacoes notificacaoDto)
        {
            try
            {
                var conversaoJson = JsonConvert.SerializeObject(notificacaoDto);

                var logNotificacoes = new LogNotificacao()
                {
                    Xml = conversaoJson,
                    Data = DateTime.Now
                };

                var response = await _logNotificacoesRepository.Adicionar(logNotificacoes);

                if (notificacaoDto.charges.Count() <= 0 )
                    return Response("Erro ao atualizar inscrição");

                if (notificacaoDto.charges[0].status != "PAID")
                    return Response("Status diferente de PAID");

                if (notificacaoDto.charges[0].payment_response.message != "SUCESSO")
                    return Response("Status difenrente de SUCESSO");

                var dadosInscricao = await _inscricaoUsuarioRepository.Buscar(x => x.Id.Equals(Convert.ToInt32(notificacaoDto.reference_id)));
                var buscaDadosInscricao = dadosInscricao.FirstOrDefault();

                //Atualiza o status da isncricao
                if (!buscaDadosInscricao.Status.Equals("AG"))
                    return Response("Não foi possível fazer operacao", false);

                //Atualiza o status da isncricao
                if (!buscaDadosInscricao.Status.Equals("AG"))
                    return Response("Não foi possível fazer operacao", false);

                buscaDadosInscricao.Status = "CO";
                buscaDadosInscricao.DataConfirmacao = DateTime.Now;
                buscaDadosInscricao.ProcessoInscricao = null;
                buscaDadosInscricao.TransacaoInscricoes = null;
                buscaDadosInscricao.Usuario = null;

                buscaDadosInscricao.MeioPagamento = 0; //retornoApiPagSeguro.PaymentMethodType;
                buscaDadosInscricao.MeioPagamentoDesc = notificacaoDto.charges[0].payment_method.type;
                buscaDadosInscricao.ValorBruto = 0;//Convert.ToDecimal(retornoApiPagSeguro.GrossAmount);
                buscaDadosInscricao.ValorLiquido = 0;//Convert.ToDecimal(retornoApiPagSeguro.NetAmount);
                buscaDadosInscricao.QtdParcelas = notificacaoDto.charges[0].payment_method.installments; 

                var atualizaInscricao = await _inscricaoUsuarioRepository.Atualizar(buscaDadosInscricao);

                if (!atualizaInscricao)
                    return Response("Erro ao atualizar inscrição", false);

                return Response(buscaDadosInscricao);
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpPost("notificacoes_2")]
        [AllowAnonymous]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> Notificacoes2([FromForm] NotificacaoDto notificacaoDto)
        {
            try
            {

                //Busca os dados dados da transacao
                var dadosConfigPagamento = await _meioPagamentoRepository.Buscar(x => x.Status.Equals("A"));
                var emailPagSeguro = dadosConfigPagamento.FirstOrDefault().Email;
                var tokenPagSeguro = dadosConfigPagamento.FirstOrDefault().Token;
                string urlPagseguro = $"{urlWsPagueSeguro}/v3/transactions/notifications/";

                var apiPagSeguro = new PagSeguroAPI();

                var retornoApiPagSeguro = apiPagSeguro.ConsultaPorCodigoNotificacao(emailPagSeguro, tokenPagSeguro, urlPagseguro, notificacaoDto.NotificationCode);

                //Inicia com log
                var logNotificacoes = new LogNotificacao()
                {
                    Data = DateTime.Now,
                    NotificationCode = notificacaoDto.NotificationCode,
                    NotificationType = notificacaoDto.NotificationType,
                    Xml = retornoApiPagSeguro != null ? JsonConvert.SerializeObject(retornoApiPagSeguro) : ""
                };

                var response = await _logNotificacoesRepository.Adicionar(logNotificacoes);

                var dadosInscricao = await _inscricaoUsuarioRepository.Buscar(x => x.Id.Equals(Convert.ToInt32(retornoApiPagSeguro.Reference)));
                var buscaDadosInscricao = dadosInscricao.FirstOrDefault();

                if ( retornoApiPagSeguro.Status.Equals(Convert.ToInt32(StatusTransacaoEnum.Devolvida)) || retornoApiPagSeguro.Status.Equals(Convert.ToInt32(StatusTransacaoEnum.Cancelada))) {

                    if (buscaDadosInscricao.Status.Equals("CO") && retornoApiPagSeguro.Date > buscaDadosInscricao.DataConfirmacao)
                    {

                        buscaDadosInscricao.Status = "AG";
                        buscaDadosInscricao.DataConfirmacao = DateTime.MinValue;
                        buscaDadosInscricao.ProcessoInscricao = null;
                        buscaDadosInscricao.TransacaoInscricoes = null;
                        buscaDadosInscricao.Usuario = null;

                        buscaDadosInscricao.MeioPagamento = null;
                        buscaDadosInscricao.MeioPagamentoDesc = "";
                        buscaDadosInscricao.ValorBruto = null;
                        buscaDadosInscricao.ValorLiquido = null;
                        buscaDadosInscricao.QtdParcelas = null;

                        await _inscricaoUsuarioRepository.Atualizar(buscaDadosInscricao);

                        return Response(buscaDadosInscricao);
                    }

                }

                if (!retornoApiPagSeguro.Status.Equals(Convert.ToInt32(StatusTransacaoEnum.Paga)) && !retornoApiPagSeguro.Status.Equals(Convert.ToInt32(StatusTransacaoEnum.Disponivel)))
                    return Response("Não foi possível fazer operacao", false);

                //Atualiza o status da isncricao
                if (!buscaDadosInscricao.Status.Equals("AG"))
                    return Response("Não foi possível fazer operacao", false);

                buscaDadosInscricao.Status = "CO";
                buscaDadosInscricao.DataConfirmacao = DateTime.Now;
                buscaDadosInscricao.ProcessoInscricao = null;
                buscaDadosInscricao.TransacaoInscricoes = null;
                buscaDadosInscricao.Usuario = null;

                buscaDadosInscricao.MeioPagamento = retornoApiPagSeguro.PaymentMethodType;
                buscaDadosInscricao.MeioPagamentoDesc = DescricaoTipoPagamento(retornoApiPagSeguro.PaymentMethodType);
                buscaDadosInscricao.ValorBruto = Convert.ToDecimal(retornoApiPagSeguro.GrossAmount);
                buscaDadosInscricao.ValorLiquido = Convert.ToDecimal(retornoApiPagSeguro.NetAmount);
                buscaDadosInscricao.QtdParcelas = retornoApiPagSeguro.installmentCount;

                var atualizaInscricao = await _inscricaoUsuarioRepository.Atualizar(buscaDadosInscricao);

                if (!atualizaInscricao)
                    return Response("Erro ao atualizar inscrição", false);

                return Response(buscaDadosInscricao);
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }

        }

        [HttpPost("notificacao-especifica")]
        [AllowAnonymous]
        public async Task<IActionResult> NotificacaoEspecificao(NotificacaoDto notificacaoDto)
        {
            try
            {
                //Busca os dados dados da transacao
                var dadosConfigPagamento = await _meioPagamentoRepository.Buscar(x => x.Status.Equals("A"));
                var emailPagSeguro = dadosConfigPagamento.FirstOrDefault().Email;
                var tokenPagSeguro = dadosConfigPagamento.FirstOrDefault().Token;
                string urlPagseguro = $"{urlWsPagueSeguro}/v3/transactions/notifications/";

                var apiPagSeguro = new PagSeguroAPI();

                var retornoApiPagSeguro = apiPagSeguro.ConsultaPorCodigoNotificacao(emailPagSeguro, tokenPagSeguro, urlPagseguro, notificacaoDto.NotificationCode);

                return Response(retornoApiPagSeguro);
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }


        [HttpGet("notificacao-especifica")]
        [AllowAnonymous]
        public async Task<IActionResult> NotificacoesPorPeriodo(DateTime dateIni, DateTime dateFim,int processoInscricaoId)
        {
            try
            {

                return Response("Não autorizado!");

                var response = await _logNotificacoesRepository.Buscar(x => x.Data >= dateIni && x.Data <= dateFim);

                if (response.ToList().Count == 0)
                    return Response("Nenhum registro encontrado!", false);

                //Busca os dados dados da transacao
                var dadosConfigPagamento = await _meioPagamentoRepository.Buscar(x => x.Status.Equals("A"));
                var emailPagSeguro = dadosConfigPagamento.FirstOrDefault().Email;
                var tokenPagSeguro = dadosConfigPagamento.FirstOrDefault().Token;
                string urlPagseguro = $"{urlWsPagueSeguro}/v3/transactions/notifications/";

                var listaNotificacoes = new List<ConsultaTransacaoPagSeguroTransactionDTO>();

                foreach (var notificacao in response.ToList())
                {
                    var apiPagSeguro = new PagSeguroAPI();
                    listaNotificacoes.Add(apiPagSeguro.ConsultaPorCodigoNotificacao(emailPagSeguro, tokenPagSeguro, urlPagseguro, notificacao.NotificationCode));
                }

                //var dadosInscricao = await _inscricaoUsuarioRepository.Buscar(x => x.ProcessoInscricaoId == processoInscricaoId);

                //var listaDadosInscritos = new List<InscricaoUsuario>();

                //foreach(var dados in dadosInscricao.ToList())
                //{
                //    var dadosPagSeguro = listaNotificacoes.ToList().Where(x => x.Reference.Equals(dados.Id.ToString())).FirstOrDefault();

                //    if (dadosPagSeguro != null)
                //    {
                //        dados.MeioPagamento = dadosPagSeguro.PaymentMethodType;
                //        dados.MeioPagamentoDesc = DescricaoTipoPagamento(dadosPagSeguro.PaymentMethodType);
                //        dados.ValorBruto = Convert.ToDecimal(dadosPagSeguro.GrossAmount);
                //        dados.ValorLiquido = Convert.ToDecimal(dadosPagSeguro.NetAmount);
                //        dados.QtdParcelas = dadosPagSeguro.installmentCount;

                //        listaDadosInscritos.Add(dados);
                //    }
                //}

                var atualizaInscricao = await _inscricaoUsuarioRepository.AtualizaGeral(listaNotificacoes, processoInscricaoId);
                return Response();
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
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

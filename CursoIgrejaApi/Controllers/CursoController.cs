using AutoMapper;
using CursoIgreja.Api.Dtos;
using CursoIgreja.Api.Services;
using CursoIgreja.Domain.Models;
using CursoIgreja.Repository.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CursoIgreja.Api.Controllers
{
    [ApiController]
    [Route("curso")]
    public class CursoController : ControllerBase
    {
        private readonly IInscricaoUsuarioRepository _inscricaoUsuarioRepository;
        private readonly IProvaUsuarioRepository _provaUsuarioRepository;
        private readonly IModuloRepository _moduloRepository;
        private readonly IConteudoUsuarioRepository _conteudoUsuarioRepository;
        private readonly IConteudoRepository _conteudoRepository;
        private readonly ICursoRepository _cursoRepository;
        private readonly IMapper _mapper;

        public CursoController(IInscricaoUsuarioRepository inscricaoUsuarioRepository,
                               IProvaUsuarioRepository provaUsuarioRepository, 
                               IModuloRepository moduloRepository, 
                               IConteudoUsuarioRepository conteudoUsuarioRepository, 
                               IConteudoRepository conteudoRepository,
                               ICursoRepository cursoRepository,
                               IMapper mapper)
        {
            _inscricaoUsuarioRepository = inscricaoUsuarioRepository;
            _provaUsuarioRepository = provaUsuarioRepository;
            _moduloRepository = moduloRepository;
            _conteudoUsuarioRepository = conteudoUsuarioRepository;
            _conteudoRepository = conteudoRepository;
            _cursoRepository = cursoRepository;
            _mapper = mapper;
        }

        [HttpPost("busca-com-filtro")]
        public async Task<IActionResult> BuscarComFiltro([FromBody] PaginationFilter filtro)
        {
            try
            {
                if (filtro.Filtro.Count() == 0)
                    return Response(await _cursoRepository.ObterTodos());

                return Response(await _cursoRepository.BuscaFiltroDinamico(filtro));
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpGet("buscar-por-id/{id}")]
        public async Task<IActionResult> BuscaPorId(int id)
        {
            try
            {
                return Response(await _cursoRepository.ObterPorId(id));
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpPost("adcionar")]
        public async Task<IActionResult> Add(Curso curso)
        {
            try
            {
                var response = await _cursoRepository.Adicionar(curso);

                if (!response)
                    return Response("Erro ao cadastrar.", false);

                return Response("Cadastro realizado com sucesso!");
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpPut("alterar")]
        public async Task<IActionResult> Alt(Curso curso)
        {
            try
            {
                var valida = await _cursoRepository.ObterPorId(curso.Id);

                if (valida == null)
                    return Response("Id não enconrado", false);

                var response = await _cursoRepository.Atualizar(curso);

                if (!response)
                    return Response("Erro ao atualizar.", false);

                return Response("Atualização realizada com sucesso!");
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpGet("carrega-curso/{id}")]
        public async Task<IActionResult> CarregaCurso(int id)
        {
            try
            {
                //Retorna os dados da inscricao
                var retornoDadosInscricao = await _inscricaoUsuarioRepository.ObterPorId(id);

                if (retornoDadosInscricao.UsuarioId != Convert.ToInt32(User.Identity.Name))
                    return Response("Busca invalida", false);

                //Obter lista prova realizado
                var listaProvaUsuario = await _provaUsuarioRepository.Buscar(x => x.UsuarioId.Equals(Convert.ToInt32(User.Identity.Name)));

                //Busca o ultimo conteudo do usuario
                var retornoConteudoUsuario = await _conteudoUsuarioRepository.Buscar(x => x.UsuariosId == Convert.ToInt32(User.Identity.Name) && x.Conteudo.Modulo.CursoId == retornoDadosInscricao.ProcessoInscricao.CursoId);

                var _cursoService = new CursoService(_moduloRepository, _conteudoUsuarioRepository, Convert.ToInt32(User.Identity.Name));

                if (retornoConteudoUsuario.Length > 0)
                {
                    var ultimoConteudo = retornoConteudoUsuario.OrderByDescending(c => c.DataConclusao).Select(c => c.Conteudo).FirstOrDefault();

                    await _cursoService.SalvarVisualizacaoUsuario(ultimoConteudo.Id);

                    _cursoService.PreenchConteudoConcluido(ultimoConteudo, listaProvaUsuario);

                    TrataRetornoItensStatus(ultimoConteudo);

                    return Response(_mapper.Map<ConteudoDto>(ultimoConteudo));
                }

                var ultimoConteudoVisualizado = retornoDadosInscricao.ProcessoInscricao.Curso.Modulo.FirstOrDefault().Conteudos[0];

                await _cursoService.SalvarVisualizacaoUsuario(ultimoConteudoVisualizado.Id);

                _cursoService.PreenchConteudoConcluido(ultimoConteudoVisualizado, listaProvaUsuario);

                TrataRetornoItensStatus(ultimoConteudoVisualizado);

                return Response(_mapper.Map<ConteudoDto>(ultimoConteudoVisualizado));
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        private static void TrataRetornoItensStatus(Conteudo retorno)
        {
            retorno.Provas = retorno.Provas.Where(x => x.Status.Equals("A")).ToList();

            retorno.Provas.ForEach(x =>
            {
                if (x.ItensProvas.Count() > 0)
                    x.ItensProvas = x.ItensProvas.ToList().Where(a => a.Status.Equals("A")).ToList();

            });
        }

        [HttpGet("carrega-conteudo-curso/{id}/{idConteudo}")]
        public async Task<IActionResult> CarregaConteudoCurso(int id, int idConteudo)
        {
            try
            {    
                //Retorna os dados da inscricao
                var retornoDadosInscricao = await _inscricaoUsuarioRepository.ObterPorId(id);

                if (retornoDadosInscricao.UsuarioId != Convert.ToInt32(User.Identity.Name))
                    return Response("Busca invalida", false);

                var response = await _conteudoRepository.ObterPorId(idConteudo);

                //Obter lista prova realizado
                var listaProvaUsuario = await _provaUsuarioRepository.Buscar(x => x.UsuarioId.Equals(Convert.ToInt32(User.Identity.Name)));

                var _cursoService = new CursoService(_moduloRepository, _conteudoUsuarioRepository, Convert.ToInt32(User.Identity.Name));

                _cursoService.PreenchConteudoConcluido(response, listaProvaUsuario);

                if (!await _cursoService.ValidaConteudoExibicao(response, listaProvaUsuario))
                    return Response(false);

                await _cursoService.SalvarVisualizacaoUsuario(response.Id);

                TrataRetornoItensStatus(response);

                return Response(_mapper.Map<ConteudoDto>(response));
            }
            catch (Exception ex)
            {

                return ResponseErro(ex);
            }
        }

        [HttpGet("carrega-conteudo-acao/{id}/{idConteudo}/{acao}")]
        public async Task<IActionResult> CarregaConteudoCursoAcao(int id, int idConteudo, string acao)
        {
            try
            {
                //Valida se esta acesso do curso
                var retornoDadosInscricao = await _inscricaoUsuarioRepository.ObterPorId(id);

                if (retornoDadosInscricao.UsuarioId != Convert.ToInt32(User.Identity.Name))
                    return Response("Busca invalida", false);

                //Obter conteudo atual
                var conteudoAtual = await _conteudoRepository.ObterPorId(idConteudo);

                if (conteudoAtual == null)
                    return Response("Conteudo nao encontrado", false);

                //Obter todos os modulos do curso
                var obterModulosCurso = await _moduloRepository.Buscar(x => x.CursoId == conteudoAtual.Modulo.CursoId);

                if (obterModulosCurso.Length <= 0)
                    return Response("Modulo nao encontrado", false);

                //Obter lista prova realizado
                var listaProvaUsuario = await _provaUsuarioRepository.Buscar(x => x.UsuarioId.Equals(Convert.ToInt32(User.Identity.Name)));

                var _cursoService = new CursoService(_moduloRepository, _conteudoUsuarioRepository, Convert.ToInt32(User.Identity.Name));

                //Preenche o conteudo concluido
                _cursoService.PreencheModuloConteudoConcluido(obterModulosCurso, listaProvaUsuario);

                if (string.IsNullOrEmpty(acao) || (acao != "P" && acao != "A"))
                    return Response("Ação não definida", false);

                var moduloAtual = obterModulosCurso.ToList().Where(x => x.Id.Equals(conteudoAtual.ModuloId)).FirstOrDefault();

                if (acao.Equals("P"))
                {
                    var retornaProximo = moduloAtual.Conteudos.Where(x => x.Ordem > conteudoAtual.Ordem).FirstOrDefault();

                    if (retornaProximo != null)
                    {
                        
                        if (!await _cursoService.ValidaConteudoExibicao(retornaProximo, listaProvaUsuario))
                            return Response(false);

                        TrataRetornoItensStatus(retornaProximo);

                        await _cursoService.SalvarVisualizacaoUsuario(retornaProximo.Id);
                        return Response(_mapper.Map<ConteudoDto>(retornaProximo));
                    }
                       
                    var proximoModulo = obterModulosCurso.Where(c => c.Ordem > moduloAtual.Ordem).FirstOrDefault();

                    if (proximoModulo == null)
                        return Response("Ação Proxima não definida", false);

                    if (!await _cursoService.ValidaConteudoExibicao(proximoModulo.Conteudos.FirstOrDefault(), listaProvaUsuario))
                        return Response(false);

                    await _cursoService.SalvarVisualizacaoUsuario(proximoModulo.Conteudos.FirstOrDefault().Id);

                    TrataRetornoItensStatus(proximoModulo.Conteudos.FirstOrDefault());

                    return Response(_mapper.Map<ConteudoDto>(proximoModulo.Conteudos.FirstOrDefault()));
                }
                else
                {
                    var retonraAnterior = moduloAtual.Conteudos.Where(x => x.Ordem < conteudoAtual.Ordem).LastOrDefault();

                    if (retonraAnterior != null)
                    {
                        await _cursoService.SalvarVisualizacaoUsuario(retonraAnterior.Id);

                        TrataRetornoItensStatus(retonraAnterior);
                        return Response(_mapper.Map<ConteudoDto>(retonraAnterior));
                    }
                        
                    var anteriorModulo = obterModulosCurso.Where(c => c.Ordem < moduloAtual.Ordem).LastOrDefault();

                    if (anteriorModulo == null)
                        return Response("Ação Anterior não definida", false);

                    await _cursoService.SalvarVisualizacaoUsuario(anteriorModulo.Conteudos.LastOrDefault().Id);

                    TrataRetornoItensStatus(anteriorModulo.Conteudos.LastOrDefault());

                    return Response(_mapper.Map<ConteudoDto>(anteriorModulo.Conteudos.LastOrDefault()));
                }
            }
            catch (Exception ex)
            {

                return ResponseErro(ex);
            }
        }

        [HttpGet("carrega-modulo-curso/{id}")]
        public async Task<IActionResult> CarregaModuloCurso(int id)
        {
            try
            {
                //Valida se esta acesso do curso
                var retornoDadosInscricao = await _inscricaoUsuarioRepository.ObterPorId(id);

                if (retornoDadosInscricao.UsuarioId != Convert.ToInt32(User.Identity.Name))
                    return Response("Busca invalida", false);

                var retorno = await _moduloRepository.Buscar(x => x.CursoId.Equals(retornoDadosInscricao.ProcessoInscricao.CursoId));

                var listaProvaUsuario = await _provaUsuarioRepository.Buscar(x => x.UsuarioId.Equals(Convert.ToInt32(User.Identity.Name)));

                var _cursoService = new CursoService(_moduloRepository, _conteudoUsuarioRepository, Convert.ToInt32(User.Identity.Name));

                _cursoService.PreencheModuloConteudoConcluido(retorno, listaProvaUsuario);

                var listaLiberacaoModulo = new List<ModuloDto>();

                foreach(var modulo in retorno)
                {
                    if (modulo.LiberacaoModulos.Count > 0)
                    {
                        if (DateTime.Now > modulo.LiberacaoModulos.LastOrDefault().DataInicio)
                            listaLiberacaoModulo.Add(_mapper.Map<ModuloDto>(modulo));
                    }
                    else
                        listaLiberacaoModulo.Add(_mapper.Map<ModuloDto>(modulo));
                }


                return Response(listaLiberacaoModulo.OrderBy(c => c.Ordem));
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

    }
}

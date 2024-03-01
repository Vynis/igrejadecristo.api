using CursoIgreja.Api.Dtos;
using CursoIgreja.Domain.Models;
using CursoIgreja.Repository.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace CursoIgreja.Api.Controllers
{
    [ApiController]
    [Route("check-usuario")]
    public class CheckUsuarioController : ControllerBase
    {
        private readonly IGeolocalizacaoUsuarioRepository _geolocalizacaoUsuarioRepository;
        private readonly IProcessoInscricaoRepository _processoInscricaoRepository;
        private readonly IInscricaoUsuarioRepository _inscricaoUsuarioRepository;
        private readonly IPresencaUsuarioRepository _presencaUsuarioRepository;
        private readonly IParametroSistemaRepository _parametroSistemaRepository;

        private bool validarLocalUsuario = false;

        public CheckUsuarioController(IGeolocalizacaoUsuarioRepository geolocalizacaoUsuarioRepository, IProcessoInscricaoRepository processoInscricaoRepository, IInscricaoUsuarioRepository inscricaoUsuarioRepository, IPresencaUsuarioRepository presencaUsuarioRepository,IParametroSistemaRepository parametroSistemaRepository)
        {
            _geolocalizacaoUsuarioRepository = geolocalizacaoUsuarioRepository;
            _processoInscricaoRepository = processoInscricaoRepository;
            _inscricaoUsuarioRepository = inscricaoUsuarioRepository;
            _presencaUsuarioRepository = presencaUsuarioRepository;
            _parametroSistemaRepository = parametroSistemaRepository;
            validarLocalUsuario = _parametroSistemaRepository.Buscar(x => x.Status.Equals("A") && x.Titulo.Equals("ValidarLocalUsuario")).Result.FirstOrDefault().Valor == "S" ? true : false;
        }

        [HttpPost("cadastrar-localizacao-usuario")]
        public async Task<IActionResult> CadastrarLocalizacaoUsuario(ParamGeolocalizacaoDto paramGeolocalizacaoDto)
        {
            try
            {
                if (string.IsNullOrEmpty(paramGeolocalizacaoDto.Latitude) || string.IsNullOrEmpty(paramGeolocalizacaoDto.Longitude))
                    return Response("Latitude/Longitude não informado", false);

                var geolocalizacaoUsuario = new GeolocalizacaoUsuario();
                CultureInfo usCulture = new CultureInfo("en-US");

                geolocalizacaoUsuario.Latitude = Convert.ToDecimal(paramGeolocalizacaoDto.Latitude, usCulture);
                geolocalizacaoUsuario.Longitude = Convert.ToDecimal(paramGeolocalizacaoDto.Longitude, usCulture);
                geolocalizacaoUsuario.UsuarioId = Convert.ToInt32(User.Identity.Name);
                geolocalizacaoUsuario.DataRegistro = DateTime.Now;

                var response = await _geolocalizacaoUsuarioRepository.Adicionar(geolocalizacaoUsuario);

                if (!response)
                    return Response("Erro ao salvar Latitude/Longitude", false);

                return Response("Cadastro realizado com sucesso!");
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpGet("buscar-processos-inscricao-liberados-check")]
        public async Task<IActionResult> BuscarProcessosInscricaoLiberadosCheck()
        {
            try
            {
                CultureInfo idioma = new CultureInfo("pt-BR");
                var diaSemana = idioma.DateTimeFormat.GetDayName(DateTime.Now.DayOfWeek).ToLower();
                var processos = await _processoInscricaoRepository.Buscar(x => (DateTime.Now >= x.DataInicioPresencial 
                                                                            && DateTime.Now <= x.DataFinalPresencial) 
                                                                            && x.DiaSemanaCurso.Equals(diaSemana) 
                                                                            && (DateTime.Now.TimeOfDay >= x.HorarioListaPresencaInicial
                                                                            && DateTime.Now.TimeOfDay <=  x.HorarioListaPresencaFinal)
                                                                            );

                var listaProcessosLiberadosCheck = new List<ProcessoInscricao>();
                var dataInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                var dataFinal = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);

                foreach (var processo in processos)
                {

                    var inscricoesusuario = await _inscricaoUsuarioRepository.Buscar(x => x.UsuarioId == Convert.ToInt32(User.Identity.Name) && x.Status != "CA" && x.ProcessoInscricaoId == processo.Id);

                    if (inscricoesusuario.Any())
                    {
                        var presencaUsuario = await _presencaUsuarioRepository.Buscar(x => x.ProcessoInscricaoId == processo.Id && x.UsuarioId == Convert.ToInt32(User.Identity.Name) && x.DataRegistro >= dataInicio & x.DataRegistro <= dataFinal);

                        if (!presencaUsuario.Any())
                            listaProcessosLiberadosCheck.Add(processo);
                    }


                }

                return Response(listaProcessosLiberadosCheck);


            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpPost("checkin/{ProcessoInscricaoId}")]
        public async Task<IActionResult> ChekUsuario(int ProcessoInscricaoId)
        {
            try
            {
                if (validarLocalUsuario)
                {
                    var estaLocalCheck = await _geolocalizacaoUsuarioRepository.VerificaUsuarioEstaNoRaio(Convert.ToInt32(User.Identity.Name));

                    if (!estaLocalCheck)
                        return Response("Aluno não está proximo ao local da aula", false);

                }

                var presencaoUsuario = new PresencaUsuario
                {
                    ProcessoInscricaoId = ProcessoInscricaoId,
                    UsuarioId = Convert.ToInt32(User.Identity.Name),
                    DataRegistro = DateTime.Now
                };

                await _presencaUsuarioRepository.Adicionar(presencaoUsuario); 

                return Response("Check in realizado com sucesso!");
            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

        [HttpGet("historico-usuario")]
        public async Task<IActionResult> HistoricoUsuario()
        {
            try
            {

                var retorno = await _presencaUsuarioRepository.Buscar(x => x.UsuarioId == Convert.ToInt32(User.Identity.Name));

                return Response(retorno.OrderByDescending(x => x.DataRegistro));

            }
            catch (Exception ex)
            {
                return ResponseErro(ex);
            }
        }

    }
}

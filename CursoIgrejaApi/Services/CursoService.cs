using CursoIgreja.Domain.Models;
using CursoIgreja.Repository.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CursoIgreja.Api.Services
{
    public class CursoService
    {
        private readonly IModuloRepository _moduloRepository;
        private readonly IConteudoUsuarioRepository _conteudoUsuarioRepository;
        private readonly int _idUsuarioLogado;

        public CursoService(IModuloRepository moduloRepository, IConteudoUsuarioRepository conteudoUsuarioRepository, int idUsuarioLogado)
        {
            _moduloRepository = moduloRepository;
            _conteudoUsuarioRepository = conteudoUsuarioRepository;
            _idUsuarioLogado = idUsuarioLogado;
        }

        public async Task<bool> SalvarVisualizacaoUsuario(int idConteudo)
        {
            try
            {

                var valida = await _conteudoUsuarioRepository.Buscar(x => x.ConteudoId == idConteudo && x.UsuariosId == _idUsuarioLogado);

                if (valida.Any())
                    return false;

                var conteudoUsuario = new ConteudoUsuario();

                conteudoUsuario.DataConclusao = DateTime.Now;
                conteudoUsuario.UsuariosId = _idUsuarioLogado;
                conteudoUsuario.Concluido = "S";
                conteudoUsuario.ConteudoId = idConteudo;

                var retorno = await _conteudoUsuarioRepository.Adicionar(conteudoUsuario);

                if (!retorno)
                    return false;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> ValidaConteudoExibicao(Conteudo conteudoSelecionado, Domain.Models.ProvaUsuario[] listaProvaUsuario)
        {
            try
            {
                //Obter todos os modulos do curso
                var obterModulosCurso = await _moduloRepository.Buscar(x => x.CursoId == conteudoSelecionado.Modulo.CursoId);

                PreencheModuloConteudoConcluido(obterModulosCurso, listaProvaUsuario);

                foreach (var mod in obterModulosCurso)
                {
                    var validarProva = true;

                    validarProva = mod.Conteudos.Where(c => (c.Tipo.Equals("PR") || c.Tipo.Equals("PA")) && !c.ConteudoConcluido).Count() > 0 ? false : true;

                    if (!validarProva)
                    {
                        if (conteudoSelecionado.Modulo.Ordem > mod.Ordem)
                            return false;
                    }
                }

                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }


        public void PreencheModuloConteudoConcluido(Domain.Models.Modulo[] retorno, Domain.Models.ProvaUsuario[] listaProvaUsuario)
        {
            foreach (var modulo in retorno)
                foreach (var conteudo in modulo.Conteudos)
                    PreenchConteudoConcluido(conteudo, listaProvaUsuario);
        }

        public void PreenchConteudoConcluido(Conteudo conteudo, Domain.Models.ProvaUsuario[] listaProvaUsuario)
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
                if (conteudo.ConteudoUsuarios != null)
                conteudo.ConteudoConcluido = conteudo.ConteudoUsuarios.Exists(x => x.ConteudoId == conteudo.Id && x.UsuariosId == _idUsuarioLogado && x.Concluido.Equals("S"));
        }

    }
}

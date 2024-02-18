using AutoMapper;
using CursoIgreja.Api.Dtos;
using CursoIgreja.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CursoIgreja.Api.AutoMapper
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Conteudo, ConteudoDto>().ReverseMap();
            CreateMap<Prova, ProvaDto>().ReverseMap();
            CreateMap<ItemProva, ItemProvaDto>().ReverseMap();
            CreateMap<Modulo, ModuloDto>().ReverseMap();
            CreateMap<LiberacaoModulo, LiberacaoModuloDto>().ReverseMap();
            CreateMap<Curso, CursoDto>().ReverseMap();
            CreateMap<UsuarioSistema, UsuarioAutDto>().ReverseMap();
            CreateMap<Usuarios, UsuarioAutDto>().ReverseMap();
        }
    }
}

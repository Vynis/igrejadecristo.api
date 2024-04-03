using CursoIgreja.Domain.Models;
using CursoIgreja.Repository.Data;
using CursoIgreja.Repository.Repository.Interfaces;
using FiltrDinamico.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CursoIgreja.Repository.Repository.Class
{
    public class GeolocalizacaoUsuarioRepository : RepositoryBase<GeolocalizacaoUsuario>, IGeolocalizacaoUsuarioRepository
    {
        private readonly DataContext _dataContext;

        public GeolocalizacaoUsuarioRepository(DataContext dataContext, IFiltroDinamico filtroDinamico) : base(dataContext, filtroDinamico)
        {
            _dataContext = dataContext;
        }

        public async override Task<GeolocalizacaoUsuario[]> Buscar(Expression<Func<GeolocalizacaoUsuario, bool>> predicado)
        {
            IQueryable<GeolocalizacaoUsuario> query = _dataContext.GeolocalizacaoUsuarios
                                                    .Where(predicado);

            return await query.AsNoTracking().ToArrayAsync();

        }

        public async Task<bool> VerificaUsuarioEstaNoRaio(int UsuarioId)
        {
            CultureInfo usCulture = new CultureInfo("en-US");
            var listaGeolocalizacaoUsuario = await _dataContext.GeolocalizacaoUsuarios.Where(x => x.UsuarioId == UsuarioId && x.DataRegistro.Date == DateTime.Now.Date  ).AsNoTracking().ToListAsync();

            var latitudePadrao = (await _dataContext.ParametroSistema.Where(x => x.Titulo.Equals("LatitudeIgreja")).AsNoTracking().ToListAsync()).FirstOrDefault();
            var longitudePadrao = (await _dataContext.ParametroSistema.Where(x => x.Titulo.Equals("LongitudeIgreja")).AsNoTracking().ToListAsync()).FirstOrDefault();

            return CalculaDistancia(listaGeolocalizacaoUsuario, Convert.ToDouble(latitudePadrao.Valor, usCulture), Convert.ToDouble(longitudePadrao.Valor, usCulture));

        }

        private bool CalculaDistancia(List<GeolocalizacaoUsuario> geolocalizacaoUsuarios, double latitudePadrao, double longitudePadrao)
        {

            if (!geolocalizacaoUsuarios.Any())
                return false;

            var ultimaLocalizacaoUsuario = geolocalizacaoUsuarios.LastOrDefault();

            var calculoConsendoRadianLatitudeLocal = Math.Cos(ConvertToRadians(latitudePadrao));
            var calculoConsendoRadianLongitudeLocal = Math.Cos(ConvertToRadians(longitudePadrao));

            var calculoConsendoRadianLatitudeLista = Math.Cos(ConvertToRadians((double)ultimaLocalizacaoUsuario.Latitude));
            var calculoConsendoRadianLongitudeLista = Math.Cos(ConvertToRadians((double)ultimaLocalizacaoUsuario.Longitude));

            var distancia = 6371 * Math.Acos(
                                                calculoConsendoRadianLatitudeLocal * 
                                                calculoConsendoRadianLatitudeLista * 
                                                Math.Cos(calculoConsendoRadianLongitudeLocal - calculoConsendoRadianLongitudeLista) +
                                                Math.Sin(ConvertToRadians(latitudePadrao)) *
                                                Math.Sin(ConvertToRadians((double)ultimaLocalizacaoUsuario.Latitude)) );

            return distancia <= 1 ? true : false;

        }

        private double ConvertToRadians(double angle)
        {
            return (Math.PI / 180) * angle;
        }


    }
}

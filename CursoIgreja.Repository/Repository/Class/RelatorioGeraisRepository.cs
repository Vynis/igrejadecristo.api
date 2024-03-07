using CursoIgreja.Repository.Data;
using CursoIgreja.Repository.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Dynamic;
using System.Data;

namespace CursoIgreja.Repository.Repository.Class
{
    public class RelatorioGeraisRepository : IRelatorioGeraisRepository
    {
        private readonly DataContext _dataContext;

        public RelatorioGeraisRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<dynamic>> ObterTodos(int ciclo, int ano, int processoInscricaoId)
        {

            var conn = _dataContext.Database.GetDbConnection();

            await conn.OpenAsync();

            var listRelatorio = new List<dynamic>();


            using (var comand = conn.CreateCommand())
            {
                string query = "sp_relatorio_presenca_aula";
                comand.CommandType = System.Data.CommandType.StoredProcedure;
                comand.CommandText = query;


                var p1 = comand.CreateParameter();
                p1.ParameterName = "@ciclo";
                p1.Value = ciclo;

                comand.Parameters.Add(p1);


                var p2 = comand.CreateParameter();
                p2.ParameterName = "@ano";
                p2.Value = ano;

                comand.Parameters.Add(p2);

                var p3 = comand.CreateParameter();
                p3.ParameterName = "@processoInscricaoId";
                p3.Value = processoInscricaoId;

                comand.Parameters.Add(p3);

                DataTable result = new DataTable();
                result.Load(await comand.ExecuteReaderAsync());

                foreach(DataRow row in result.Rows)
                {
                    dynamic relatorio = new ExpandoObject();

                    foreach (DataColumn column in row.Table.Columns)
                        AddProperty(relatorio, column.ColumnName , row[column.ColumnName].ToString());
                    
                    listRelatorio.Add(relatorio);
                }
            }


            return listRelatorio;
        }

        public static void AddProperty(ExpandoObject objeto, string namePropertyname, object valueProperty)
        {
            var objetoDict = objeto as IDictionary<string, object>;

            if (objetoDict.ContainsKey(namePropertyname))
                objetoDict[namePropertyname] = valueProperty;
            else
                objetoDict.Add(namePropertyname, valueProperty);

        }

    }
}

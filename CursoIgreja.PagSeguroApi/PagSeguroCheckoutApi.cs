using CursoIgreja.PagSeguroApi.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace CursoIgreja.PagSeguroApi
{
    public class PagSeguroCheckoutApi
    {

        public PagSeguroRetornoModel.PagSeguro checkout(string urlApiPagueSeguro, PagSeguroModel.PagSeguro dadosPagamento, string token)
        {
            try
            {
                var conversaoJson = JsonConvert.SerializeObject(dadosPagamento);
                var request = new HttpRequestMessage(HttpMethod.Post, urlApiPagueSeguro);

                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                    var content = new StringContent(conversaoJson, null, "application/json");
                    request.Content = content;

                    var response = httpClient.SendAsync(request).Result;
                    response.EnsureSuccessStatusCode();
                    var jsonResult = response.Content.ReadAsStringAsync().Result;
                    var retorno = JsonConvert.DeserializeObject<PagSeguroRetornoModel.PagSeguro>(jsonResult);
                    return retorno;
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }


        }

        public PagSeguroRetornoCheckoutModel.Checkout vericaCheckou(string urlApiPagueSeguro, string token)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, urlApiPagueSeguro);
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    var response = httpClient.SendAsync(request).Result;
                    response.EnsureSuccessStatusCode();
                    var jsonResult = response.Content.ReadAsStringAsync().Result;
                    var retorno = JsonConvert.DeserializeObject<PagSeguroRetornoCheckoutModel.Checkout>(jsonResult);
                    return retorno;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        public PagSeguroRetornoOrderModel.Order vericaOrder(string urlApiPagueSeguro, string token)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, urlApiPagueSeguro);
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    var response = httpClient.SendAsync(request).Result;
                    response.EnsureSuccessStatusCode();
                    var jsonResult = response.Content.ReadAsStringAsync().Result;
                    var retorno = JsonConvert.DeserializeObject<PagSeguroRetornoOrderModel.Order>(jsonResult);
                    return retorno;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

    }
}

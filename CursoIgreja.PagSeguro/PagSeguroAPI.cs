using CursoIgreja.PagSeguro.Enum;
using CursoIgreja.PagSeguro.TransferObjects;
using CursoIgreja.PagSeguro.TransferObjects.NotificacaoTransacao;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;


namespace CursoIgreja.PagSeguro
{
    public class PagSeguroAPI
    {
        /// <summary>
        /// Construtor.
        /// Define SecurityProtocolType.
        /// </summary>
        public PagSeguroAPI() {
            //Define protocolos de comunicação.
            //Importante para funcionar SSL e TLS.
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13 ;
        }		
		

        public string GetSession(string emailUsuario, string token, string urlCheckout, int cont = 0)
        {
            System.Collections.Specialized.NameValueCollection postData = new System.Collections.Specialized.NameValueCollection();
            postData.Add("email", emailUsuario);
            postData.Add("token", token);

            //String que receberá o XML de retorno.
            string xmlString = null;

            //Webclient faz o post para o servidor de pagseguro.
            using (WebClient wc = new WebClient())
            {
                try
                {
                    //Informa header sobre URL.
                    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";

                    //Faz o POST e retorna o XML contendo resposta do servidor do pagseguro.
                    var result = wc.UploadValues(urlCheckout, postData);

                    //Obtém string do XML.
                    xmlString = Encoding.ASCII.GetString(result);
                }
                catch (Exception ex)
                {
                    cont++;
                    if (cont < 6)
                        GetSession(emailUsuario, token, urlCheckout,cont);

                    Console.WriteLine(ex.Message);
                }

            }

            //Cria documento XML.
            XmlDocument xmlDoc = new XmlDocument();

            //Carrega documento XML por string.
            xmlDoc.LoadXml(xmlString);

            //Retorna código do checkout.
            return xmlDoc.GetElementsByTagName("id")[0].InnerText;
        }


        /// <summary>
        /// Realiza checkout com a conta parametrizada na configuração do sistema.
        /// </summary>
        /// <param name="emailUsuario">E-mail usuário pagseguro.</param>
        /// <param name="token">Token.</param>
        /// <param name="urlCheckout">URL Checkout.</param>
        /// <param name="itens">Itens de venda.</param>
        /// <param name="comprador">Dados do comprador.</param>
        /// <param name="reference">Referência da transação.</param>
        /// <returns></returns>
        public string Checkout(string emailUsuario, string token, string urlCheckout, List<PagSeguroItemDTO> itens, PagSeguroCompradorDTO comprador, string reference, string metodoPagamento = "CREDIT_CARD,BOLETO,PIX")
        {
            //Conjunto de parâmetros/formData.
            System.Collections.Specialized.NameValueCollection postData = new System.Collections.Specialized.NameValueCollection();
            postData.Add("email", emailUsuario);
            postData.Add("token", token);
            postData.Add("currency", "BRL");

            //postData.Add("email", "vynis2005@gmail.com");
            ////postData.Add("token", "25625C54F05444CA9CB9030A8CA12119");
            //postData.Add("token", "AFCB097BE79B42F9902EAB5886E5477D");
            //postData.Add("currency", "BRL");
            //postData.Add("itemId1", "0001");
            //postData.Add("itemDescription1", "ProdutoPagSeguroI");
            //postData.Add("itemAmount1", "3.00");
            //postData.Add("itemQuantity1", "1");
            //postData.Add("itemWeight1", "200");
            //postData.Add("reference", "REF1234");
            //postData.Add("senderName", "Jose Comprador");
            //postData.Add("senderAreaCode", "44");
            //postData.Add("senderPhone", "999999999");
            //postData.Add("senderEmail", "comprador@gmail.com");
            //postData.Add("shippingAddressRequired", "false");


            for (int i = 0; i < itens.Count; i++)
            {
                postData.Add(string.Concat("itemId", i + 1), itens[i].itemId);
                postData.Add(string.Concat("itemDescription", i + 1), itens[i].itemDescription);
                postData.Add(string.Concat("itemAmount", i + 1), itens[i].itemAmount);
                postData.Add(string.Concat("itemQuantity", i + 1), itens[i].itemQuantity);
                postData.Add(string.Concat("itemWeight", i + 1), itens[i].itemWeight);
            }

            //Reference.
            postData.Add("reference", reference);

            //Comprador.
            if (comprador != null)
            {
                postData.Add("senderName", comprador.SenderName);
                postData.Add("senderAreaCode", comprador.SenderAreaCode);
                postData.Add("senderPhone", comprador.senderPhone);
                postData.Add("senderEmail", comprador.senderEmail);
            }

            //Shipping.
            postData.Add("shippingAddressRequired", "false");

            //Formas de pagamento.
            //Cartão de crédito e boleto.
            postData.Add("acceptPaymentMethodGroup", metodoPagamento);

            //String que receberá o XML de retorno.
            string xmlString = null;

            //Webclient faz o post para o servidor de pagseguro.
            using (WebClient wc = new WebClient())
            {
                try
                {
                    //Informa header sobre URL.
                    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";

                    //Faz o POST e retorna o XML contendo resposta do servidor do pagseguro.
                    var result = wc.UploadValues(urlCheckout, postData);

                    //Obtém string do XML.
                    xmlString = Encoding.ASCII.GetString(result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }

            //Cria documento XML.
            XmlDocument xmlDoc = new XmlDocument();

            //Carrega documento XML por string.
            xmlDoc.LoadXml(xmlString);

            //Obtém código de transação (Checkout).
            var code = xmlDoc.GetElementsByTagName("code")[0];

            //Obtém data de transação (Checkout).
            var date = xmlDoc.GetElementsByTagName("date")[0];

            //Retorna código do checkout.
            return code.InnerText;
        }

        /// <summary>
        /// Consulta por código referência.
        /// </summary>
        /// <param name="emailUsuario">E-mail usuário pagseguro.</param>
        /// <param name="token">Token.</param>
        /// <param name="urlConsultaTransacao">URL consulta transação.</param>
        /// <param name="codigoReferencia">Código de referência.</param>
        /// <returns>DTO com resultados do XML.</returns>
        public ConsultaTransacaoPagSeguroDTO ConsultaPorCodigoReferencia(string emailUsuario, string token, string urlConsultaTransacao, string codigoReferencia)
        {
            //Variável de retorno.
            ConsultaTransacaoPagSeguroDTO retorno = new ConsultaTransacaoPagSeguroDTO();
            retorno.listTransaction = new List<ConsultaTransacaoPagSeguroTransactionDTO>();

            try
            {
                //uri de consulta da transação.
                string uri = string.Concat(urlConsultaTransacao, "?email=", emailUsuario, "&token=", token, "&reference=", codigoReferencia);

                //Classe que irá fazer a requisição GET.
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);

                //Método do webrequest.
                request.Method = "GET";

                //String que vai armazenar o xml de retorno.
                string xmlString = null;

                //Obtém resposta do servidor.
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    //Cria stream para obter retorno.
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        //Lê stream.
                        using (StreamReader reader = new StreamReader(dataStream))
                        {
                            //Xml convertido para string.
                            xmlString = reader.ReadToEnd();

                            //Cria xml document para facilitar acesso ao xml.
                            XmlDocument xmlDoc = new XmlDocument();

                            //Carrega xml document através da string com XML.
                            xmlDoc.LoadXml(xmlString);

                            //Resultados na página.
                            //Por padrão é retornado a página 1 com 50 transações.
                            var resultsInThisPage = Convert.ToInt32(xmlDoc.GetElementsByTagName("resultsInThisPage")[0].InnerText);

                            //Total de páginas.
                            var totalPages = Convert.ToInt32(xmlDoc.GetElementsByTagName("totalPages")[0].InnerText);

                            //CurrentPage.
                            var currentPage = Convert.ToInt32(xmlDoc.GetElementsByTagName("currentPage")[0].InnerText);

                            //Obtém lista de Transações.
                            var listTransactions = xmlDoc.GetElementsByTagName("transactions")[0];

                            //Popula retorno.
                            retorno.CurrentPage = currentPage;
                            retorno.TotalPages = totalPages;
                            retorno.ResultsInThisPage = resultsInThisPage;

                            //Usado para conversão de data W3C.
                            string formatStringW3CDate = "yyyy-MM-ddTHH:mm:ss.fffzzz";
                            System.Globalization.CultureInfo cInfoW3CDate = new System.Globalization.CultureInfo("en-US", true);

                            //Popula transações.
                            if (listTransactions != null)
                            {
                                foreach (XmlNode childNode in listTransactions)
                                {
                                    //Cria novo item de transação.
                                    var itemTransacao = new ConsultaTransacaoPagSeguroTransactionDTO();

                                    foreach (XmlNode childNode2 in childNode.ChildNodes)
                                    {
                                        if (childNode2.Name == "date")
                                        {
                                            var date = System.DateTime.ParseExact(childNode2.InnerText, formatStringW3CDate, cInfoW3CDate);
                                            itemTransacao.Date = date;
                                        }
                                        else if (childNode2.Name == "reference")
                                        {
                                            itemTransacao.Reference = childNode2.InnerText;
                                        }
                                        else if (childNode2.Name == "code")
                                        {
                                            itemTransacao.Code = childNode2.InnerText;
                                        }
                                        else if (childNode2.Name == "type")
                                        {
                                            itemTransacao.type = Convert.ToInt32(childNode2.InnerText);
                                        }
                                        else if (childNode2.Name == "status")
                                        {
                                            itemTransacao.Status = Convert.ToInt32(childNode2.InnerText);
                                        }
                                        else if (childNode2.Name == "paymentMethod")
                                        {
                                            foreach (XmlNode nodePaymentMethod in childNode2.ChildNodes)
                                            {
                                                if (nodePaymentMethod.Name == "type")
                                                {
                                                    itemTransacao.PaymentMethodType = Convert.ToInt32(childNode2.InnerText);
                                                }
                                            }
                                        }
                                        else if (childNode2.Name == "grossAmount")
                                        {
                                            itemTransacao.GrossAmount = Convert.ToDouble(childNode2.InnerText, CultureInfo.InvariantCulture);
                                        }
                                        else if (childNode2.Name == "discountAmount")
                                        {
                                            itemTransacao.DiscountAmount = Convert.ToDouble(childNode2.InnerText, CultureInfo.InvariantCulture);
                                        }
                                        else if (childNode2.Name == "feeAmount")
                                        {
                                            itemTransacao.FeeAmount = Convert.ToDouble(childNode2.InnerText, CultureInfo.InvariantCulture);
                                        }
                                        else if (childNode2.Name == "netAmount")
                                        {
                                            itemTransacao.NetAmount = Convert.ToDouble(childNode2.InnerText, CultureInfo.InvariantCulture);
                                        }
                                        else if (childNode2.Name == "extraAmount")
                                        {
                                            itemTransacao.ExtraAmount = Convert.ToDouble(childNode2.InnerText, CultureInfo.InvariantCulture);
                                        }
                                        else if (childNode2.Name == "lastEventDate")
                                        {
                                            var lastEventDate = System.DateTime.ParseExact(childNode2.InnerText, formatStringW3CDate, cInfoW3CDate);
                                            itemTransacao.LastEventDate = lastEventDate;
                                        }
                                    }

                                    //Adiciona item de transação.
                                    retorno.listTransaction.Add(itemTransacao);
                                }
                            }

                            //Fecha reader.
                            reader.Close();

                            //Fecha stream.
                            dataStream.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //Retorno.
            return retorno;
        }

        public ConsultaTransacaoPagSeguroTransactionDTO ConsultaPorCodigoNotificacao(string emailUsuario, string token, string urlConsultaTransacao, string codigoNotificacao)
        {

            //Variável de retorno.
            ConsultaTransacaoPagSeguroTransactionDTO retorno = new ConsultaTransacaoPagSeguroTransactionDTO();

            try
            {

                //uri de consulta da transação.
                string uri = string.Concat(urlConsultaTransacao, codigoNotificacao, "?email=", emailUsuario, "&token=", token);

                //Classe que irá fazer a requisição GET.
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);

                //Método do webrequest.
                request.Method = "GET";

                //String que vai armazenar o xml de retorno.
                string xmlString = null;

                //Obtém resposta do servidor.
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    //Cria stream para obter retorno.
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        //Lê stream.
                        using (StreamReader reader = new StreamReader(dataStream))
                        {
                            //Xml convertido para string.
                            xmlString = reader.ReadToEnd();

                            //Cria xml document para facilitar acesso ao xml.
                            XmlDocument xmlDoc = new XmlDocument();

                            //Carrega xml document através da string com XML.
                            xmlDoc.LoadXml(xmlString);

                            // Obtém lista de Transações.
                            var listTransactions = xmlDoc.GetElementsByTagName("transaction")[0];

                            //Usado para conversão de data W3C.
                            string formatStringW3CDate = "yyyy-MM-ddTHH:mm:ss.fffzzz";
                            System.Globalization.CultureInfo cInfoW3CDate = new System.Globalization.CultureInfo("en-US", true);

                            if (listTransactions != null)
                            {
                                //Cria novo item de transação.
                                var itemTransacao = new ConsultaTransacaoPagSeguroTransactionDTO();

                                foreach (XmlNode childNode in listTransactions)
                                {

                                    if (childNode.Name == "date")
                                    {
                                        var date = System.DateTime.ParseExact(childNode.InnerText, formatStringW3CDate, cInfoW3CDate);
                                        itemTransacao.Date = date;
                                    }
                                    else if (childNode.Name == "reference")
                                    {
                                        itemTransacao.Reference = childNode.InnerText;
                                    }
                                    else if (childNode.Name == "code")
                                    {
                                        itemTransacao.Code = childNode.InnerText;
                                    }
                                    else if (childNode.Name == "type")
                                    {
                                        itemTransacao.type = Convert.ToInt32(childNode.InnerText);
                                    }
                                    else if (childNode.Name == "status")
                                    {
                                        itemTransacao.Status = Convert.ToInt32(childNode.InnerText);
                                    }
                                    else if (childNode.Name == "paymentMethod")
                                    {
                                        foreach (XmlNode nodePaymentMethod in childNode.ChildNodes)
                                        {
                                            if (nodePaymentMethod.Name == "type")
                                            {
                                                itemTransacao.PaymentMethodType = Convert.ToInt32(nodePaymentMethod.InnerText);
                                            }
                                        }
                                    }
                                    else if (childNode.Name == "grossAmount")
                                    {
                                        itemTransacao.GrossAmount = Convert.ToDouble(childNode.InnerText, CultureInfo.InvariantCulture);
                                    }
                                    else if (childNode.Name == "discountAmount")
                                    {
                                        itemTransacao.DiscountAmount = Convert.ToDouble(childNode.InnerText, CultureInfo.InvariantCulture);
                                    }
                                    else if (childNode.Name == "feeAmount")
                                    {
                                        itemTransacao.FeeAmount = Convert.ToDouble(childNode.InnerText, CultureInfo.InvariantCulture);
                                    }
                                    else if (childNode.Name == "netAmount")
                                    {
                                        itemTransacao.NetAmount = Convert.ToDouble(childNode.InnerText, CultureInfo.InvariantCulture);
                                    }
                                    else if (childNode.Name == "extraAmount")
                                    {
                                        itemTransacao.ExtraAmount = Convert.ToDouble(childNode.InnerText, CultureInfo.InvariantCulture);
                                    }
                                    else if (childNode.Name == "lastEventDate")
                                    {
                                        var lastEventDate = System.DateTime.ParseExact(childNode.InnerText, formatStringW3CDate, cInfoW3CDate);
                                        itemTransacao.LastEventDate = lastEventDate;
                                    }
                                    else if (childNode.Name == "installmentCount")
                                    {
                                        itemTransacao.installmentCount = Convert.ToInt32(childNode.InnerText);
                                    }

                                }


                                //Adiciona item de transação.
                                retorno = itemTransacao;
                            }

                            //Fecha reader.
                            reader.Close();

                            //Fecha stream.
                            dataStream.Close();
                        }
                     }
                 }


                }
            catch (Exception ex)
            {

                throw ex;
            }


            //Retorno.
            return retorno;

        }

        public TransactionPagSeguro ConsultaPorCodigoNotificacao2(string emailUsuario, string token, string urlConsultaTransacao, string codigoNotificacao)
        {
            //uri de consulta da transação.
            string uri = string.Concat(urlConsultaTransacao, codigoNotificacao, "?email=", emailUsuario, "&token=", token);

            //Classe que irá fazer a requisição GET.
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);

            //Método do webrequest.
            request.Method = "GET";

            //String que vai armazenar o xml de retorno.
            string xmlString = null;

            XmlSerializer serializer = new XmlSerializer(typeof(TransactionPagSeguro));

            var retornoTransacao = new TransactionPagSeguro();

            //Obtém resposta do servidor.
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {

                using (Stream dataStream = response.GetResponseStream())
                {
                    //Lê stream.
                    using (StreamReader reader = new StreamReader(dataStream))
                    {
                        xmlString = reader.ReadToEnd();

                        using (StringReader xmlReader = new StringReader(xmlString))
                        {
                            var transacao = (TransactionPagSeguro)serializer.Deserialize(xmlReader);

                            transacao.Xml = xmlString;

                            return transacao;
                        }


                    }
                }

            }

            return retornoTransacao;
        }

        /// <summary>
        /// Cancela transação com status de "Aguardando Pagamento" ou "Em Análise".
        /// </summary>
        /// <param name="emailUsuario">E-mail usuário pagseguro.</param>
        /// <param name="token">Token.</param>
        /// <param name="urlCancelamento">URL Cancelamento.</param>
        /// <param name="transactionCode">Código da transação.</param>
        /// <returns>Bool. Caso true, transação foi cancelada. Caso false, transação não foi cancelada.</returns>
        public bool CancelarTransacao(string emailUsuario, string token, string urlCancelamento, string transactionCode)
        {
            //Monta url completa para solicitação.
            string urlCompleta = string.Concat(urlCancelamento);

            //String que receberá o XML de retorno.
            string xmlString = null;

            //Webclient faz o post para o servidor de pagseguro.
            using (WebClient wc = new WebClient())
            {
                //Informa header sobre URL.
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";

                //PostData.
                System.Collections.Specialized.NameValueCollection postData = new System.Collections.Specialized.NameValueCollection();
                postData.Add("email", emailUsuario);
                postData.Add("token", token);
                postData.Add("transactionCode", transactionCode);

                //Faz o POST e retorna o XML contendo resposta do servidor do pagseguro.
                var result = wc.UploadValues(urlCancelamento, postData);

                //Obtém string do XML.
                xmlString = Encoding.ASCII.GetString(result);
            }

            //Cria documento XML.
            XmlDocument xmlDoc = new XmlDocument();

            //Carrega documento XML por string.
            xmlDoc.LoadXml(xmlString);

            //Obtém código de transação (Checkout).
            //Caso ocorra tudo ok, a API do PagSeguro retornará uma tag "result" com o conteúdo "OK"
            //Caso o cancelamento não ocorra, será retornado as tags errors -> error e dentro da error, as tags code e message.
            var xmlResult = xmlDoc.GetElementsByTagName("result");

            //Retorno.
            bool retorno;

            //Verifica se tem a tag resultado.
            if (xmlResult.Count > 0)
            {
                retorno = xmlResult[0].InnerText == "OK";
            }
            else
            {
                retorno = false;
            }

            //Retorno do método.
            return retorno;
        }

        /// <summary>
        /// Nome amigável para status do pagseguro.
        /// </summary>
        /// <param name="status">Status.</param>
        /// <returns>Nome amigável.</returns>
        public  string NomeAmigavelStatusPagSeguro(StatusTransacaoEnum status)
        {
            string retorno;

            if (status == StatusTransacaoEnum.NaoExisteTransacao)
            {
                retorno = "Nenhuma Transação Encontrada";
            }
            else if (status == StatusTransacaoEnum.AguardandoPagamento)
            {
                retorno = "Aguardando Pagamento";
            }
            else if (status == StatusTransacaoEnum.EmAnalise)
            {
                retorno = "Em Análise";
            }
            else if (status == StatusTransacaoEnum.Paga)
            {
                retorno = "Pago";
            }
            else if (status == StatusTransacaoEnum.Disponivel)
            {
                retorno = "Disponível";
            }
            else if (status == StatusTransacaoEnum.EmDisputa)
            {
                retorno = "Em Disputa";
            }
            else if (status == StatusTransacaoEnum.Devolvida)
            {
                retorno = "Devolvida";
            }
            else if (status == StatusTransacaoEnum.Cancelada)
            {
                retorno = "Cancelada";
            }
            else if (status == StatusTransacaoEnum.Debitado)
            {
                retorno = "Debitado (Devolvido)";
            }
            else if (status == StatusTransacaoEnum.RetencaoTemporaria)
            {
                retorno = "Retenção Temp.";
            }
            else
            {
                throw new Exception("Falha ao resolver status pagseguro.");
            }

            return retorno;
        }

        /// <summary>
        /// Nome amigável para tipo de pagamento do pagseguro.
        /// </summary>
        /// <param name="tipoPagamento">Tipo do pagamento.</param>
        /// <returns>Tipo pagamento.</returns>
        public string NomeAmigavelTipoPagamentoPagSeguro(TipoPagamentoEnum tipoPagamento)
        {
            string retorno;

            if (tipoPagamento == TipoPagamentoEnum.Boleto)
            {
                retorno = "Boleto";
            }
            else if (tipoPagamento == TipoPagamentoEnum.CartaoDeCredito)
            {
                retorno = "Cartão de Crédito";
            }
            else if (tipoPagamento == TipoPagamentoEnum.DebitoOnLineTEF)
            {
                retorno = "Débito Online (TEF)";
            }
            else if (tipoPagamento == TipoPagamentoEnum.OiPago)
            {
                retorno = "Oi Pago";
            }
            else if (tipoPagamento == TipoPagamentoEnum.SaldoPagSeguro)
            {
                retorno = "Saldo PagSeguro";
            }
            else if (tipoPagamento == TipoPagamentoEnum.DepositoEmConta)
            {
                retorno = "Depósito em Conta";
            }
            else
            {
                throw new Exception("Falha ao resolver tipo pagamento pagseguro.");
            }

            return retorno;
        }
    }
}
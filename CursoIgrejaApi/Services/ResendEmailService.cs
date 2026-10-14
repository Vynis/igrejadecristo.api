using CursoIgreja.Repository.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CursoIgreja.Api.Services
{
    public class ResendEmailService : IResendEmailService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IParametroSistemaRepository _parametroSistemaRepository;
        private readonly ILogger<ResendEmailService> _logger;

        public ResendEmailService(
            HttpClient httpClient,
            IConfiguration configuration,
            IParametroSistemaRepository parametroSistemaRepository,
            ILogger<ResendEmailService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _parametroSistemaRepository = parametroSistemaRepository;
            _logger = logger;
        }

        private async Task<(string ApiKey, string FromEmail)> ObterConfiguracaoAsync()
        {
            var apiKey = _configuration["Resend:ApiKey"];
            var fromEmail = _configuration["Resend:FromEmail"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                var paramsApiKey = await _parametroSistemaRepository.Buscar(x => x.Status == "A" && x.Titulo == "ResendApiKey");
                apiKey = paramsApiKey.FirstOrDefault()?.Valor;
            }

            if (string.IsNullOrWhiteSpace(fromEmail))
            {
                var paramsFrom = await _parametroSistemaRepository.Buscar(x => x.Status == "A" && x.Titulo == "ResendFromEmail");
                fromEmail = paramsFrom.FirstOrDefault()?.Valor;
            }

            if (string.IsNullOrWhiteSpace(fromEmail))
            {
                fromEmail = "Summit Academy <onboarding@resend.dev>";
            }

            return (apiKey?.Trim(), fromEmail?.Trim());
        }

        public async Task<(bool Sucesso, string MensagemErro)> EnviarEmailAsync(string destinatario, string assunto, string conteudoHtml)
        {
            try
            {
                var (apiKey, fromEmail) = await ObterConfiguracaoAsync();

                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    var msg = "Chave da API Resend não configurada. Defina em appsettings.json (Resend:ApiKey) ou na tabela parametrosistema.";
                    _logger.LogWarning(msg);
                    return (false, msg);
                }

                var payload = new
                {
                    from = fromEmail,
                    to = new[] { destinatario },
                    subject = assunto,
                    html = conteudoHtml
                };

                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.resend.com/emails");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                request.Content = content;

                var response = await _httpClient.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"E-mail enviado via Resend para {destinatario} com sucesso. Status: {response.StatusCode}");
                    return (true, null);
                }
                else
                {
                    var erro = $"Erro Resend ({response.StatusCode}): {responseBody}";
                    _logger.LogError(erro);
                    return (false, erro);
                }
            }
            catch (Exception ex)
            {
                var erro = $"Exceção ao enviar e-mail via Resend: {ex.Message}";
                _logger.LogError(ex, erro);
                return (false, erro);
            }
        }

        public async Task<(bool Sucesso, string MensagemErro)> EnviarCodigoRecuperacaoAsync(string destinatario, string nomeUsuario, string codigo)
        {
            var assunto = $"Seu código de recuperação: {codigo} - Summit Academy";
            var html = MontarTemplateEmailCodigo(nomeUsuario, codigo);

            return await EnviarEmailAsync(destinatario, assunto, html);
        }

        private string MontarTemplateEmailCodigo(string nome, string codigo)
        {
            var primeiroNome = !string.IsNullOrWhiteSpace(nome) ? nome.Split(' ')[0] : "Aluno(a)";

            return $@"
<!DOCTYPE html>
<html lang=""pt-BR"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Código de Recuperação</title>
</head>
<body style=""margin: 0; padding: 0; background-color: #f4f6f8; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif; color: #333333;"">
    <table role=""presentation"" width=""100%"" cellspacing=""0"" cellpadding=""0"" border=""0"" style=""background-color: #f4f6f8; padding: 30px 10px;"">
        <tr>
            <td align=""center"">
                <table role=""presentation"" width=""100%"" max-width=""560px"" style=""max-width: 560px; background-color: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 14px rgba(0,0,0,0.06);"" cellspacing=""0"" cellpadding=""0"" border=""0"">
                    <!-- Header -->
                    <tr>
                        <td style=""background-color: #1a2a3a; padding: 28px 30px; text-align: center;"">
                            <h1 style=""color: #ffffff; margin: 0; font-size: 22px; font-weight: 700; letter-spacing: 1px;"">SUMMIT ACADEMY</h1>
                            <p style=""color: #94a3b8; margin: 6px 0 0 0; font-size: 13px;"">Igreja de Cristo</p>
                        </td>
                    </tr>

                    <!-- Body Content -->
                    <tr>
                        <td style=""padding: 36px 32px;"">
                            <h2 style=""color: #1e293b; font-size: 19px; margin: 0 0 16px 0; font-weight: 600;"">Olá, {primeiroNome}!</h2>
                            <p style=""font-size: 15px; line-height: 1.6; color: #475569; margin: 0 0 24px 0;"">
                                Recebemos uma solicitação para redefinir sua senha no aplicativo <strong>Summit Academy</strong>. Use o código de 6 dígitos abaixo para continuar:
                            </p>

                            <!-- Verification Code Box -->
                            <div style=""background-color: #f1f5f9; border: 2px dashed #cbd5e1; border-radius: 10px; padding: 20px; text-align: center; margin: 26px 0;"">
                                <span style=""display: inline-block; font-size: 34px; font-weight: 800; letter-spacing: 8px; color: #1e3a8a; font-family: Consolas, 'Courier New', monospace;"">
                                    {codigo}
                                </span>
                            </div>

                            <div style=""background-color: #fef3c7; border-left: 4px solid #f59e0b; padding: 12px 16px; border-radius: 4px; margin-bottom: 24px;"">
                                <p style=""margin: 0; font-size: 13px; color: #92400e; line-height: 1.5;"">
                                    ⏰ <strong>Atenção:</strong> Este código é válido por <strong>15 minutos</strong>.
                                </p>
                            </div>

                            <p style=""font-size: 13px; line-height: 1.5; color: #64748b; margin: 0;"">
                                Se você não solicitou a recuperação da senha, nenhuma ação é necessária. Sua conta permanece segura.
                            </p>
                        </td>
                    </tr>

                    <!-- Footer -->
                    <tr>
                        <td style=""background-color: #f8fafc; padding: 20px 30px; text-align: center; border-top: 1px solid #e2e8f0;"">
                            <p style=""font-size: 12px; color: #94a3b8; margin: 0;"">
                                © Summit Academy • Igreja de Cristo Brasil
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
        }
    }
}

using CursoIgreja.Repository.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace CursoIgreja.Api.Services
{
    public class EnviaEmail
    {
        private readonly IParametroSistemaRepository _parametroSistemaRepository;
        private string urlEmailConfig = "";
        private string senhaEmailConfig = "";
        private string smtpEmailConfig = "";

        public EnviaEmail(IParametroSistemaRepository parametroSistemaRepository)
        {
            _parametroSistemaRepository = parametroSistemaRepository;
            urlEmailConfig = _parametroSistemaRepository.Buscar(x => x.Status.Equals("A") && x.Titulo.Equals("Email")).Result.FirstOrDefault().Valor;
            senhaEmailConfig = _parametroSistemaRepository.Buscar(x => x.Status.Equals("A") && x.Titulo.Equals("SenhaEmail")).Result.FirstOrDefault().Valor;
            smtpEmailConfig = _parametroSistemaRepository.Buscar(x => x.Status.Equals("A") && x.Titulo.Equals("SmtpEmail")).Result.FirstOrDefault().Valor;
        }

        public async Task<bool> Enviar(string destinatario, string cabecalho, string texto, string titulo)
        {

            try
            {
                MailMessage mail = new MailMessage()
                {
                    From = new MailAddress(urlEmailConfig, titulo)
                };

                mail.To.Add(new MailAddress(destinatario));
                mail.Subject = cabecalho;
                mail.Body = texto;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;

                using (SmtpClient smtp = new SmtpClient(smtpEmailConfig, 587))
                {
                    smtp.Credentials = new NetworkCredential(urlEmailConfig, senhaEmailConfig);
                    await smtp.SendMailAsync(mail);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}

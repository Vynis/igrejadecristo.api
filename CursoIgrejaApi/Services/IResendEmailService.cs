using System.Threading.Tasks;

namespace CursoIgreja.Api.Services
{
    public interface IResendEmailService
    {
        Task<(bool Sucesso, string MensagemErro)> EnviarEmailAsync(string destinatario, string assunto, string conteudoHtml);
        Task<(bool Sucesso, string MensagemErro)> EnviarCodigoRecuperacaoAsync(string destinatario, string nomeUsuario, string codigo);
    }
}

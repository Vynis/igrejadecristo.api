using System.ComponentModel.DataAnnotations;

namespace CursoIgreja.Api.Dtos
{
    public class SolicitarCodigoDto
    {
        [Required(ErrorMessage = "Informe o e-mail ou CPF")]
        public string EmailOuCpf { get; set; }
    }

    public class ValidarCodigoDto
    {
        [Required(ErrorMessage = "Informe o e-mail ou CPF")]
        public string EmailOuCpf { get; set; }

        [Required(ErrorMessage = "Informe o código de verificação")]
        [StringLength(10, MinimumLength = 4, ErrorMessage = "Código inválido")]
        public string Codigo { get; set; }
    }

    public class RedefinirSenhaCodigoDto
    {
        [Required(ErrorMessage = "Token temporário obrigatório")]
        public string TokenTemporario { get; set; }

        [Required(ErrorMessage = "A nova senha é obrigatória")]
        [MinLength(4, ErrorMessage = "A senha deve ter pelo menos 4 caracteres")]
        public string NovaSenha { get; set; }
    }
}

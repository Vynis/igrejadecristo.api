using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CursoIgreja.Domain.Models
{
    [Table("lognotificacoes")]
    public class LogNotificacao
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("notificationcode")]
        public string NotificationCode { get; set; }
        [Column("notificationtype")]
        public string NotificationType { get; set; }
        [Column("data")]
        public DateTime Data { get; set; }
        [Column("xml")]
        public string Xml { get; set; }
    }
}

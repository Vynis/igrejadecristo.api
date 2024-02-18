using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CursoIgreja.PagSeguro.TransferObjects.NotificacaoTransacao
{
	[XmlRoot(ElementName = "paymentMethod")]
	public class PaymentMethod
	{

		[XmlElement(ElementName = "type")]
		public int Type { get; set; }

		[XmlElement(ElementName = "code")]
		public int Code { get; set; }
	}
}

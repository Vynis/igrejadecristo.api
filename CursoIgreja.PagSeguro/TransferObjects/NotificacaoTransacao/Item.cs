using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CursoIgreja.PagSeguro.TransferObjects.NotificacaoTransacao
{
	[XmlRoot(ElementName = "item")]
	public class Item
	{

		[XmlElement(ElementName = "id")]
		public int Id { get; set; }

		[XmlElement(ElementName = "description")]
		public string Description { get; set; }

		[XmlElement(ElementName = "quantity")]
		public int Quantity { get; set; }

		[XmlElement(ElementName = "amount")]
		public double Amount { get; set; }
	}
}

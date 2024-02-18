using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CursoIgreja.PagSeguro.TransferObjects.NotificacaoTransacao
{
	[XmlRoot(ElementName = "items")]
	public class Items
	{

		[XmlElement(ElementName = "item")]
		public Item Item { get; set; }
	}
}

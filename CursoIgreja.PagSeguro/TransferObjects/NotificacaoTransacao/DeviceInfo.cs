using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CursoIgreja.PagSeguro.TransferObjects.NotificacaoTransacao
{
	[XmlRoot(ElementName = "deviceInfo")]
	public class DeviceInfo
	{

		[XmlElement(ElementName = "reference")]
		public string Reference { get; set; }

		[XmlElement(ElementName = "bin")]
		public string Bin { get; set; }

		[XmlElement(ElementName = "holder")]
		public string Holder { get; set; }

		[XmlElement(ElementName = "serialNumber")]
		public string SerialNumber { get; set; }
	}
}

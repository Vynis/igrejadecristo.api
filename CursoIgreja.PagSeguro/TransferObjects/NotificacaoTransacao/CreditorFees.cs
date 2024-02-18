using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CursoIgreja.PagSeguro.TransferObjects.NotificacaoTransacao
{
	[XmlRoot(ElementName = "creditorFees")]
	public class CreditorFees
	{

		[XmlElement(ElementName = "installmentFeeAmount")]
		public double InstallmentFeeAmount { get; set; }

		[XmlElement(ElementName = "intermediationRateAmount")]
		public double IntermediationRateAmount { get; set; }

		[XmlElement(ElementName = "intermediationFeeAmount")]
		public double IntermediationFeeAmount { get; set; }
	}

}

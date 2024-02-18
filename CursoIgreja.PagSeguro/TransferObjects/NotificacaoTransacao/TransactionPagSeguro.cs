using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CursoIgreja.PagSeguro.TransferObjects.NotificacaoTransacao
{
	[XmlRoot(ElementName = "transaction")]
	public class TransactionPagSeguro
	{

		[XmlElement(ElementName = "date")]
		public DateTime Date { get; set; }

		[XmlElement(ElementName = "code")]
		public string Code { get; set; }

		[XmlElement(ElementName = "type")]
		public int Type { get; set; }

		[XmlElement(ElementName = "status")]
		public int Status { get; set; }

		[XmlElement(ElementName = "lastEventDate")]
		public DateTime LastEventDate { get; set; }

		[XmlElement(ElementName = "paymentMethod")]
		public PaymentMethod PaymentMethod { get; set; }

		[XmlElement(ElementName = "grossAmount")]
		public double GrossAmount { get; set; }

		[XmlElement(ElementName = "discountAmount")]
		public double DiscountAmount { get; set; }

		[XmlElement(ElementName = "creditorFees")]
		public CreditorFees CreditorFees { get; set; }

		[XmlElement(ElementName = "netAmount")]
		public double NetAmount { get; set; }

		[XmlElement(ElementName = "extraAmount")]
		public double ExtraAmount { get; set; }

		[XmlElement(ElementName = "escrowEndDate")]
		public DateTime EscrowEndDate { get; set; }

		[XmlElement(ElementName = "installmentCount")]
		public int InstallmentCount { get; set; }

		[XmlElement(ElementName = "itemCount")]
		public int ItemCount { get; set; }

		[XmlElement(ElementName = "items")]
		public Items Items { get; set; }

		[XmlElement(ElementName = "primaryReceiver")]
		public PrimaryReceiver PrimaryReceiver { get; set; }

		[XmlElement(ElementName = "deviceInfo")]
		public DeviceInfo DeviceInfo { get; set; }

		[XmlIgnore]
        public string Xml { get; set; }
    }
}

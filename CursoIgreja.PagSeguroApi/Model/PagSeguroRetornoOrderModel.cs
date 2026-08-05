using System;
using System.Collections.Generic;
using System.Text;

namespace CursoIgreja.PagSeguroApi.Model
{
    public class PagSeguroRetornoOrderModel
    {
        public class Order
        {
            public string Id { get; set; }
            public string Reference_Id { get; set; }
            public DateTime CreatedAt { get; set; }
            public Customer Customer { get; set; }
            public List<Item> Items { get; set; }
            public List<Charge> Charges { get; set; }
            public List<string> NotificationUrls { get; set; }
            public List<Link> Links { get; set; }
        }

        public class Customer
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public string TaxId { get; set; }
            public List<Phone> Phones { get; set; }
        }

        public class Phone
        {
            public string Type { get; set; }
            public string Country { get; set; }
            public string Area { get; set; }
            public string Number { get; set; }
        }

        public class Item
        {
            public string ReferenceId { get; set; }
            public string Name { get; set; }
            public int Quantity { get; set; }
            public int UnitAmount { get; set; }
        }

        public class Charge
        {
            public string Id { get; set; }
            public string ReferenceId { get; set; }
            public string Status { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? PaidAt { get; set; }
            public Amount Amount { get; set; }
            public PaymentResponse Payment_Response { get; set; }
            public PaymentMethod PaymentMethod { get; set; }
            public List<Link> Links { get; set; }
            public Dictionary<string, object> Metadata { get; set; }
        }

        public class Amount
        {
            public int Value { get; set; }
            public string Currency { get; set; }
            public Summary Summary { get; set; }
        }

        public class Summary
        {
            public int Total { get; set; }
            public int Paid { get; set; }
            public int Refunded { get; set; }
            public int Incremented { get; set; }
        }

        public class PaymentResponse
        {
            public string Code { get; set; }
            public string Message { get; set; }
            public string Reference { get; set; }
            public RawData RawData { get; set; }
        }

        public class RawData
        {
            public string AuthorizationCode { get; set; }
            public string Nsu { get; set; }
            public string ReasonCode { get; set; }
        }

        public class PaymentMethod
        {
            public string Type { get; set; }
            public int Installments { get; set; }
            public bool Capture { get; set; }
            public Card Card { get; set; }
            public AuthenticationMethod AuthenticationMethod { get; set; }
            public string SoftDescriptor { get; set; }
        }

        public class Card
        {
            public string Brand { get; set; }
            public string FirstDigits { get; set; }
            public string LastDigits { get; set; }
            public string ExpMonth { get; set; }
            public string ExpYear { get; set; }
            public Holder Holder { get; set; }
            public TokenData TokenData { get; set; }
            public Issuer Issuer { get; set; }
            public string Country { get; set; }
        }

        public class Holder
        {
            public string Name { get; set; }
            public string TaxId { get; set; }
        }

        public class TokenData
        {
            public string RequestorId { get; set; }
            public int AssuranceLevel { get; set; }
        }

        public class Issuer
        {
            public string Name { get; set; }
            public string Product { get; set; }
        }

        public class AuthenticationMethod
        {
            public string Id { get; set; }
            public string Status { get; set; }
            public string Type { get; set; }
        }

        public class Link
        {
            public string Rel { get; set; }
            public string Href { get; set; }
            public string Media { get; set; }
            public string Type { get; set; }
        }
    }
}

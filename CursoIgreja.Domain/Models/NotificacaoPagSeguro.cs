using System;
using System.Collections.Generic;
using System.Text;

namespace CursoIgreja.Domain.Models
{
    public class Checkout
    {
        public string Id { get; set; }
        public string ReferenceId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public Customer Customer { get; set; }
        public bool CustomerModifiable { get; set; }
        public List<Item> Items { get; set; }
        public int AdditionalAmount { get; set; }
        public int DiscountAmount { get; set; }
        public List<PaymentMethod> PaymentMethods { get; set; }
        public List<PaymentMethodConfig> PaymentMethodsConfigs { get; set; }
        public string RedirectUrl { get; set; }
        public List<string> NotificationUrls { get; set; }
        public List<string> PaymentNotificationUrls { get; set; }
        public List<Link> Links { get; set; }
    }

    public class Customer
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class Item
    {
        public string ReferenceId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public int UnitAmount { get; set; }
    }

    public class PaymentMethod
    {
        public string Type { get; set; }
    }

    public class PaymentMethodConfig
    {
        public string Type { get; set; }
        public List<ConfigOption> ConfigOptions { get; set; }
    }

    public class ConfigOption
    {
        public string Option { get; set; }
        public string Value { get; set; }
    }

    public class Link
    {
        public string Rel { get; set; }
        public string Href { get; set; }
        public string Method { get; set; }
    }

}

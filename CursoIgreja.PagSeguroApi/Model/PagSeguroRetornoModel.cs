using System;
using System.Collections.Generic;
using System.Text;

namespace CursoIgreja.PagSeguroApi.Model
{
    public class PagSeguroRetornoModel
    {

        public class PagSeguro
        {
            public string id { get; set; }
            public string reference_id { get; set; }
            public DateTime created_at { get; set; }
            public string status { get; set; }
            public Customer customer { get; set; }
            public bool customer_modifiable { get; set; }
            public Item[] items { get; set; }
            public int additional_amount { get; set; }
            public int discount_amount { get; set; }
            public Payment_Methods[] payment_methods { get; set; }
            public Payment_Methods_Configs[] payment_methods_configs { get; set; }
            public string redirect_url { get; set; }
            public string[] notification_urls { get; set; }
            public string[] payment_notification_urls { get; set; }
            public Link[] links { get; set; }
        }

        public class Customer
        {
            public string name { get; set; }
            public string email { get; set; }
        }

        public class Item
        {
            public string reference_id { get; set; }
            public string name { get; set; }
            public int quantity { get; set; }
            public int unit_amount { get; set; }
        }

        public class Payment_Methods
        {
            public string type { get; set; }
        }

        public class Payment_Methods_Configs
        {
            public string type { get; set; }
            public Config_Options[] config_options { get; set; }
        }

        public class Config_Options
        {
            public string option { get; set; }
            public string value { get; set; }
        }

        public class Link
        {
            public string rel { get; set; }
            public string href { get; set; }
            public string method { get; set; }
        }

    }
}

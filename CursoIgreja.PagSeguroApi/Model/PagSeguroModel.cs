using System;
using System.Collections.Generic;
using System.Text;

namespace CursoIgreja.PagSeguroApi.Model
{
    public class PagSeguroModel
    {

        public class PagSeguro
        {
            public string reference_id { get; set; }
            public List<Item> items { get; set; }
            public List<Payment_Methods> payment_methods { get; set; }
            public List<Payment_Methods_Configs> payment_methods_configs { get; set; }
            public Customer customer { get; set; }
            public string redirect_url { get; set; }
            public string[] notification_urls { get; set; }
            public string[] payment_notification_urls { get; set; }
        }

        public class Customer
        {
            public string name { get; set; }
            public string email { get; set; }
            public string tax_id { get; set; }
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
            public string[] brands { get; set; }
            public List<Config_Options> config_options { get; set; }
        }

        public class Config_Options
        {
            public string option { get; set; }
            public string value { get; set; }
        }

    }
}

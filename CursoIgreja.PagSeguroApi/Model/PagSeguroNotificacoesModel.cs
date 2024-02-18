using System;
using System.Collections.Generic;
using System.Text;

namespace CursoIgreja.PagSeguroApi.Model
{
    public class PagSeguroNotificacoesModel
    {

        public class PagSeguroNotificacoes
        {
            public string id { get; set; }
            public string reference_id { get; set; }
            public DateTime created_at { get; set; }
            public Shipping shipping { get; set; }
            public Item[] items { get; set; }
            public Customer customer { get; set; }
            public Charge[] charges { get; set; }
            public Qr_Code[] qr_code { get; set; }
            public Link2[] links { get; set; }
            public string status { get; set; }
        }

        public class Shipping
        {
            public Address address { get; set; }
        }

        public class Address
        {
            public string street { get; set; }
            public string number { get; set; }
            public string complement { get; set; }
            public string locality { get; set; }
            public string city { get; set; }
            public string region_code { get; set; }
            public string country { get; set; }
            public string postal_code { get; set; }
        }

        public class Customer
        {
            public string name { get; set; }
            public string email { get; set; }
            public string tax_id { get; set; }
            public Phone[] phones { get; set; }
        }

        public class Phone
        {
            public string country { get; set; }
            public string area { get; set; }
            public string number { get; set; }
            public string type { get; set; }
        }

        public class Item
        {
            public string reference_id { get; set; }
            public string name { get; set; }
            public int quantity { get; set; }
            public int unit_amount { get; set; }
        }

        public class Charge
        {
            public string id { get; set; }
            public string reference_id { get; set; }
            public string status { get; set; }
            public DateTime created_at { get; set; }
            public DateTime paid_at { get; set; }
            public string description { get; set; }
            public Amount amount { get; set; }
            public Payment_Response payment_response { get; set; }
            public Payment_Method payment_method { get; set; }
            public Link[] links { get; set; }
        }

        public class Amount
        {
            public int value { get; set; }
            public string currency { get; set; }
            public Summary summary { get; set; }
        }

        public class Summary
        {
            public int total { get; set; }
            public int paid { get; set; }
            public int refunded { get; set; }
        }

        public class Payment_Response
        {
            public string code { get; set; }
            public string message { get; set; }
            public string reference { get; set; }
        }

        public class Payment_Method
        {
            public string type { get; set; }
            public int installments { get; set; }
            public bool capture { get; set; }
            public Card card { get; set; }
        }

        public class Card
        {
            public string brand { get; set; }
            public string first_digits { get; set; }
            public string last_digits { get; set; }
            public string exp_month { get; set; }
            public string exp_year { get; set; }
            public Holder holder { get; set; }
        }

        public class Holder
        {
            public string name { get; set; }
        }

        public class Link
        {
            public string rel { get; set; }
            public string href { get; set; }
            public string media { get; set; }
            public string type { get; set; }
        }

        public class Qr_Code
        {
            public string id { get; set; }
            public Amount1 amount { get; set; }
            public string text { get; set; }
            public Link1[] links { get; set; }
        }

        public class Amount1
        {
            public int value { get; set; }
        }

        public class Link1
        {
            public string rel { get; set; }
            public string href { get; set; }
            public string media { get; set; }
            public string type { get; set; }
        }

        public class Link2
        {
            public string rel { get; set; }
            public string href { get; set; }
            public string media { get; set; }
            public string type { get; set; }
        }

    }
}

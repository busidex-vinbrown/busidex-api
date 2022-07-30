using System;

namespace Busidex.DomainModels
{
    [Serializable]
    public class Resource
    {
        public string id { get; set; }
        public string create_time { get; set; }
        public string update_time { get; set; }
        public string state { get; set; }
        public string status { get; set; }
        public Amount amount { get; set; }
        public Seller_Protection seller_protection { get; set; }
        
        
        public bool final_capture { get; set; }
        public Seller_Receivable_Breakdown seller_receivable_breakdown { get; set; }
        public Link[] links { get; set; }
        
    }

    [Serializable]
    public class AmountDetails
    {
        public string subtotal { get; set; }
    }
    [Serializable]
    public class Amount
    {
        public string currency_code { get; set; }
        public string value { get; set; }
    }
    public class Seller_Protection
    {
        public string status { get; set; }
        public string dispute_categories { get; set; }
    }
    public class Seller_Receivable_Breakdown
    {
        public Amount gross_amount { get; set; }
        public Amount paypal_fee { get; set; }
        public Amount net_amount { get; set; }

    }

    [Serializable]
    public class Link
    {
        public string href { get; set; }
        public string rel { get; set; }
        public string method { get; set; } 
        public string encType { get; set; }
    }

    [Serializable]
    public class PayPalPayment
    {
        public string id { get; set; }
        public string create_time { get; set; } 
        public string resource_type { get; set; }
        public string event_type { get; set; }  
        public string summary { get; set; }
        public Resource resource { get; set; }
        public Link[] links { get; set; }
        public string event_version { get; set; }
        public string resource_version { get; set; }        
    }
}
/*
         * {
  "id": "WH-58D329510W468432D-8HN650336L201105X",
  "create_time": "2019-02-14T21:50:07.940Z",
  "resource_type": "capture",
  "event_type": "PAYMENT.CAPTURE.COMPLETED",
  "summary": "Payment completed for $ 2.51 USD",
  "resource": {
    "amount": {
      "currency_code": "USD",
      "value": "2.51"
    },
    "seller_protection": {
      "status": "ELIGIBLE",
      "dispute_categories": [
        "ITEM_NOT_RECEIVED",
        "UNAUTHORIZED_TRANSACTION"
      ]
    },
    "update_time": "2019-02-14T21:49:58Z",
    "create_time": "2019-02-14T21:49:58Z",
    "final_capture": true,
    "seller_receivable_breakdown": {
      "gross_amount": {
        "currency_code": "USD",
        "value": "2.51"
      },
      "paypal_fee": {
        "currency_code": "USD",
        "value": "0.37"
      },
      "net_amount": {
        "currency_code": "USD",
        "value": "2.14"
      }
    },
    "links": [
      {
        "href": "https://api.paypal.com/v2/payments/captures/27M47624FP291604U",
        "rel": "self",
        "method": "GET"
      },
      {
        "href": "https://api.paypal.com/v2/payments/captures/27M47624FP291604U/refund",
        "rel": "refund",
        "method": "POST"
      },
      {
        "href": "https://api.paypal.com/v2/payments/authorizations/7W5147081L658180V",
        "rel": "up",
        "method": "GET"
      }
    ],
    "id": "27M47624FP291604U",
    "status": "COMPLETED"
  },
  "links": [
    {
      "href": "https://api.paypal.com/v1/notifications/webhooks-events/WH-58D329510W468432D-8HN650336L201105X",
      "rel": "self",
      "method": "GET",
      "encType": "application/json"
    },
    {
      "href": "https://api.paypal.com/v1/notifications/webhooks-events/WH-58D329510W468432D-8HN650336L201105X/resend",
      "rel": "resend",
      "method": "POST",
      "encType": "application/json"
    }
  ],
  "event_version": "1.0",
  "resource_version": "2.0"
}
         */
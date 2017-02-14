using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BOE.Areas.ClientSurvey.Models
{
    public class ClientSurveyVM
    {
        public string DepositeNo { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
        public int ID { get; set; }
        public string Date { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string UserName { get; set; }
        public string Text { get; set; }
        public DateTime CommunicationDate { get; set; }
        public string DepositeDate { get; set; }
        public string ClientType { get; set; }
        public string DepositeDateEdit{get;set;}

    }
}
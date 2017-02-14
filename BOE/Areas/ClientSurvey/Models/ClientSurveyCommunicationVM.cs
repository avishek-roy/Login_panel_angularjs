using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BOE.Areas.ClientSurvey.Models
{
    public class ClientSurveyCommunicationVM
    {
        public int Id { get; set; }
        public DateTime Date { get;set;}
        public string DateShow { get; set; }
        public string Code { get; set; }
        public string Text { get; set; }
        public string ContactPerson { get; set; }
        public string ContactNo { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Reason { get; set; }
        public string FeedBack { get; set; }
        public string FollowUpDate { get; set; }
        public string CommunicationDate { get; set; }
        public string FollowUpDateShow { get; set; }
        public string CommunicationDateShow { get; set; }
        public string Type { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Username { get; set; }
        public string ComnucationType { get; set; }
    }
}
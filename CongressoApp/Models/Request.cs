using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CongressoApp.Models
{
    public class Request
    {
        public int Id { get; set; }
        public string Theme { get; set; }
        public string Location { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? Date { get; set; }
        public string DateTimeFormat
        {
            get { return Date.HasValue?$"{Date.Value.Day}/{Date.Value.Month}/{Date.Value.Year}":""; }
        }
        public double? Price { get; set; }
        public string PriceFormat
        {
            get { return Price.HasValue?$"{Price.Value} €":"n\\a"; }                
        }
        public RequestType RequestType { get; set; }
        public List<Document> Documents { get; set; }
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }
        public virtual List<Professional> Professionals { get; set; }
        public virtual ApplicationUser CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public ApproveStatus? StatusLevel1 { get; set; }
        public virtual ApplicationUser ApproverLevel1 { get; set; }
        public string Level1Notes { get; set; }
        public ApproveStatus? StatusLevel2 { get; set; }
        public virtual ApplicationUser ApproverLevel2 { get; set; }
        public string Level2Notes { get; set; }
        public ApproveStatus? StatusLevel3 { get; set; }
        public virtual ApplicationUser ApproverLevel3 { get; set; }
        public string Level3Notes { get; set; }
        public ApproveStatus? StatusLevel4 { get; set; }
        public virtual ApplicationUser ApproverLevel4 { get; set; }
        public string Level4Notes { get; set; }
        public int CurrentLevel { get; set; }
    }

    public class RequestViewModel
    {
        public Request Request { get; set; }
        public IEnumerable<SelectListItem> ProfessionalsList { get; set; }
        public string SelectedPro { get; set; }
        public string RequestDate { get; set; }
        public List<DocumentViewModel> Documents { get; set; }
    }

    public class RequestsIndexViewModel
    {
        //public List<Request> Level1 { get; set; }
        //public List<Request> Level2 { get; set; }
        //public List<Request> Level3 { get; set; }
        //public List<Request> Level4 { get; set; }
        public List<Request> Level1 = new List<Request>();
        public List<Request> Level2 = new List<Request>();
        public List<Request> Level3 = new List<Request>();
        public List<Request> Level4 = new List<Request>();
        public List<Request> Accepted = new List<Request>();
    }

}
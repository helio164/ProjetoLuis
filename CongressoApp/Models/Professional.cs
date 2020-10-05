using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CongressoApp.Models
{
    public class Professional
    {
        [Key]
        public int Id { get; set; }
        public string Ballot { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string DropdownFormatName { get { return $"{Ballot} - {Name} {Surname}"; } }
        public string VatNumber { get; set; }
        [Phone]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string Address { get; set; }
        public ProfessionalCategory ProfessionalCategory { get; set; }
        public string Specialty { get; set; }
        public string WorkPlace { get; set; }
        public virtual ICollection<Request> Requests { get; set; }
    }
}
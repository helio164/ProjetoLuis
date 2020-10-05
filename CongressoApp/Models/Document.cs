using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CongressoApp.Models
{
    public class Document
    {
        public int Id { get; set; }
        public virtual Request Request { get; set; }
        public string Name { get; set; }
        [DataType(DataType.Date)]
        public DateTime? UploadedAt { get; set; }
        [DataType(DataType.Upload)]
        public byte[] File { get; set; }
        public DocumentType DocumentType { get; set; }
        public string Notes { get; set; }
    }

    public class DocumentViewModel
    {
        public string Name { get; set; }
        public string Path { get; set; }

    }
}
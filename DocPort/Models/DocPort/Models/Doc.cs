using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace DocPort.Models
{
    [Table("Doc", Schema = "Document")]
    public class Doc
    {
        [Key]
        public int ID { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public required string Title { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime LastTimeUpdated { get; set; }
        [StringLength(500)]
        [Unicode(false)]
        public string? Description { get; set; }
        public bool IsTrashed { get; set; }
        public bool IsTrashedPrime { get; set; }
        public int? DirectoryID { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocPort.Models.DocPort.Models
{
    [Table("ContentHolder", Schema = "Document")]
    public class ContentHolder
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
        public int DirectoryID { get; set; }
        [ForeignKey("DirectoryID")]
        public Doc? DirectoryDoc { get; set; }
    }
}

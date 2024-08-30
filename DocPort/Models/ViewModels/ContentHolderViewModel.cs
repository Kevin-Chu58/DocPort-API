using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DocPort.Models.DocPort.Models;

namespace DocPort.Models.ViewModels
{
    public class ContentHolderViewModel
    {
        public int ID { get; set; }
        public required string Title { get; set; }
        public DateTime LastTimeUpdated { get; set; }
        public string? Description { get; set; }
        public bool IsTrashed { get; set; }
        public bool IsTrashedPrime { get; set; }
        public int DirectoryID { get; set; }

        public static implicit operator ContentHolderViewModel(ContentHolder ch)
        {
            ContentHolderViewModel vm = new()
            {
                ID = ch.ID,
                Title = ch.Title,
                LastTimeUpdated = ch.LastTimeUpdated,
                Description = ch.Description,
                IsTrashed = ch.IsTrashed,
                IsTrashedPrime = ch.IsTrashedPrime,
                DirectoryID = ch.DirectoryID,
            };

            return vm;
        }
    }
}

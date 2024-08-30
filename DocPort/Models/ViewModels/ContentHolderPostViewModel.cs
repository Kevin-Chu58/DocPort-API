using DocPort.Models.DocPort.Models;

namespace DocPort.Models.ViewModels
{
    public class ContentHolderPostViewModel
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public int DirectoryID { get; set; }
        
        public ContentHolder ToContentHolder()
        {
            ContentHolder ch = new()
            {
                Title = Title,
                LastTimeUpdated = DateTime.UtcNow,
                Description = Description,
                DirectoryID = DirectoryID,
                IsTrashed = false,
                IsTrashedPrime = false,
            };
            return ch;
        }
    }
}

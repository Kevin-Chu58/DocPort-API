namespace DocPort.Models.ViewModels
{
    public class DocPostViewModel
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public int? DirectoryID { get; set; }

        public Doc ToDoc()
        {
            Doc doc = new()
            {
                Title = Title,
                LastTimeUpdated = DateTime.UtcNow,
                Description = Description,
                IsTrashed = false,
                IsTrashedPrime = false,
                DirectoryID = DirectoryID ?? 0,
            };

            return doc;
        }
    }
}

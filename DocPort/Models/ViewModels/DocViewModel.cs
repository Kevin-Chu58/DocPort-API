using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DocPort.Models.ViewModels
{
    public class DocViewModel
    {
        public int ID { get; set; }
        public required string Title { get; set; }
        public DateTime LastTimeUpdated { get; set; }
        public string? Description { get; set; }
        public bool IsTrashed { get; set; }
        // true - Doc is trashed, and it is the primary doc trashed,
        // not subsequent ones
        public bool IsTrashedPrime { get; set; }
        // 0 = Under "primary directory"
        // others = refer to Under another Doc
        public int? DirectoryID { get; set; }


        public static implicit operator DocViewModel(Doc doc)
        {
            DocViewModel vm = new()
            {
                ID = doc.ID,
                Title = doc.Title,
                LastTimeUpdated = doc.LastTimeUpdated,
                Description = doc.Description,
                DirectoryID = doc.DirectoryID,
                IsTrashed = doc.IsTrashed,
                IsTrashedPrime = doc.IsTrashedPrime,
            };

            return vm;
        }

        public static implicit operator Doc(DocViewModel vm)
        {
            Doc doc = new()
            {
                ID = vm.ID,
                Title = vm.Title,
                LastTimeUpdated = vm.LastTimeUpdated,
                Description = vm.Description,
                DirectoryID = vm.DirectoryID,
                IsTrashed = vm.IsTrashed,
                IsTrashedPrime = vm.IsTrashedPrime,
            };
            return doc;
        }
    }
}

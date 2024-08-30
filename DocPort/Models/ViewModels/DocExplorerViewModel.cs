namespace DocPort.Models.ViewModels
{
    public class DocExplorerViewModel
    {
        public int ID { get; set; }
        public required string Title { get; set; }
        public int SubDocsInside { get; set; }
        public List<DocExplorerViewModel>? Docs { get; set; }
    }
}

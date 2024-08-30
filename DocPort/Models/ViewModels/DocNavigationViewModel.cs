namespace DocPort.Models.ViewModels
{
    public class DocNavigationViewModel
    {
        public List<int>? Ids { get; set; }
        public List<string>? Titles { get; set; }
        public bool IsPrimeDirectory { get; set; }
        public string? Title { get; set; }
    }
}

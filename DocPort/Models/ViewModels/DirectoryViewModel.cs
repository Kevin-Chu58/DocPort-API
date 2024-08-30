using DocPort.Static.Names;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DocPort.Models.ViewModels
{
    public class DirectoryViewModel(string type, object content)
    {
        public string? Type { get; set; } = type;
        public object? Content { get; set; } = content;

        public static implicit operator DirectoryViewModel(DocViewModel doc)
        {
            return new DirectoryViewModel(DirectoryTypes.DOC, doc);
        }

        public static implicit operator DirectoryViewModel(ContentHolderViewModel ch)
        {
            return new DirectoryViewModel(DirectoryTypes.CONTENT_HOLDER, ch);
        }
    }
}

using DocPort.Models;
using DocPort.Models.DocPort.Models;
using DocPort.Models.ViewModels;
using System.Collections.Generic;

namespace DocPort.Services
{
    public class ServiceInterfaces
    {
        public interface IDocsService
        {
            Doc GetDocByID(int docID, bool throwsException = true);
            DocViewModel GetDocRestored();
            IEnumerable<DocViewModel> GetDocsByDirectoryID(int directoryID);
            IEnumerable<DocViewModel> GetBin();
            DocNavigationViewModel GetNavigation(int docID);
            DocExplorerViewModel GetDocExplorer(int directoryID);
            DocViewModel AddNewDoc(DocPostViewModel newDoc);
            DocViewModel UpdateDoc(int docID, DocPostViewModel newDoc);
            IEnumerable<DocViewModel> UpdateDocIsTrashed(int docID);
            DocViewModel UpdateDirectoryID(int docID, int directoryID);
            IEnumerable<DocViewModel> DeleteDoc(int docID);
            IEnumerable<DocViewModel> DeleteAllTrash();
            bool IsDirectoryInBin(int directoryID);
        }

        public interface IContentHoldersService
        {
            ContentHolder GetContentHolderByID(int chID, bool throwsException = true);
            IEnumerable<ContentHolderViewModel> GetContentHoldersByDirectoryID(int directoryID);
            IEnumerable<ContentHolderViewModel> GetBin();
            ContentHolderViewModel AddNewContentHolder(ContentHolderPostViewModel ch);
            ContentHolderViewModel UpdateContentHolder(int chID, ContentHolderPostViewModel ch);
            ContentHolderViewModel UpdateContentHolderIsTrashed(int chID, bool isTrashedPrime = true);
            ContentHolderViewModel UpdateDirectoryID(int chID, int directoryID);
            ContentHolderViewModel DeleteContentHolder(int chID);
            IEnumerable<ContentHolderViewModel> DeleteAllTrash();
        }

        // more services
    }
}

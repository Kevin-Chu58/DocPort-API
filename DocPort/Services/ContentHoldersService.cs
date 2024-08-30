using DocPort.Models.DocPort.Context;
using DocPort.Models.DocPort.Models;
using DocPort.Models.ViewModels;
using static DocPort.Services.ServiceInterfaces;

namespace DocPort.Services
{
    public class ContentHoldersService(DocPortContext context, IDocsService _docsService) : IContentHoldersService
    {
        readonly DocsService docsService = (DocsService)_docsService;

        public ContentHolder GetContentHolderByID(int chID, bool throwsException = true)
        {
            var ch = context.ContentHolders.Find(chID);
            if (throwsException && ch == null) 
            {
                throw new Exception("Content Holder not found.");
            }
            return ch;
        }

        public IEnumerable<ContentHolderViewModel> GetContentHoldersByDirectoryID(int directoryID)
        {
            var chItems = context.ContentHolders
                .Where(ch => ch.IsTrashed == false 
                    && ch.DirectoryID == directoryID)
                .Select<ContentHolder, ContentHolderViewModel>(x => x);

            return chItems;
        }

        public IEnumerable<ContentHolderViewModel> GetBin()
        {
            var chItems = context.ContentHolders
                .Where(ch => ch.IsTrashed == true
                    && ch.IsTrashedPrime == true)
                .Select<ContentHolder, ContentHolderViewModel>(x => x);

            return chItems;
        }

        public ContentHolderViewModel AddNewContentHolder(ContentHolderPostViewModel ch)
        {
            var chToAdd = ch.ToContentHolder();

            // check if directory in Bin
            if (ch.DirectoryID > 0 && docsService.IsDirectoryInBin(ch.DirectoryID))
                throw new Exception("Invalid creation of Content Holder in Bin.");

            context.ContentHolders.Add(chToAdd);
            context.SaveChanges();

            return (ContentHolderViewModel)chToAdd;
        }

        public ContentHolderViewModel UpdateContentHolder(int chID, ContentHolderPostViewModel ch)
        {
            var chToUpdate = GetContentHolderByID(chID);
            chToUpdate.Title = ch.Title;
            chToUpdate.Description = ch.Description;
            chToUpdate.LastTimeUpdated = DateTime.UtcNow;

            context.ContentHolders.Update(chToUpdate);
            context.SaveChanges();

            UpdateDirectoryID(chID, ch.DirectoryID);
            chToUpdate.DirectoryID = ch.DirectoryID;

            return (ContentHolderViewModel)chToUpdate;
        }

        public ContentHolderViewModel UpdateContentHolderIsTrashed(int chID, bool isTrashedPrime = true)
        {
            var chToUpdate = GetContentHolderByID(chID);

            // when restore ContentHolder from Bin, check whether its directory is valid for restoration
            var chToUpdateDirectory = docsService.GetDocByID(chToUpdate.DirectoryID, false);
            if (chToUpdate.IsTrashed && (chToUpdateDirectory == null || chToUpdateDirectory.IsTrashed))
            {
                var docRestored = docsService.GetDocRestored();
                chToUpdate.DirectoryID = docRestored.ID;
            }

            chToUpdate.IsTrashed = !chToUpdate.IsTrashed;
            if (isTrashedPrime)
                chToUpdate.IsTrashedPrime = !chToUpdate.IsTrashedPrime;

            context.ContentHolders.Update(chToUpdate);
            context.SaveChanges();

            return (ContentHolderViewModel)chToUpdate;
        }

        public ContentHolderViewModel UpdateDirectoryID(int chID, int directoryID)
        {

            // check invalid directoryID
            if (directoryID <= 0)
                throw new Exception("Invalid directory.");

            // neither can update directoryID of a Doc in Bin, nor
            // can update to a directory that is trashed

            var chToUpdate = GetContentHolderByID(chID);
            if (chToUpdate.IsTrashed)
                throw new Exception("Invalid directory update to a Doc in Bin.");

            if (directoryID > 0)
            {
                var directoryForUpdate = _docsService.GetDocByID(directoryID);
                if (directoryForUpdate.IsTrashed)
                    throw new Exception("Invalid directory in Bin.");
            }

            // update directoryID
            chToUpdate.DirectoryID = directoryID;
            context.Update(chToUpdate);
            context.SaveChanges();

            return (ContentHolderViewModel)chToUpdate;
        }

        public ContentHolderViewModel DeleteContentHolder(int chID)
        {
            var chToDelete = GetContentHolderByID(chID);
            if (!chToDelete.IsTrashed)
                throw new Exception("Content Holder is not in Bin.");

            context.ContentHolders.Remove(chToDelete);
            context.SaveChanges();

            return (ContentHolderViewModel)chToDelete;
        }

        public IEnumerable<ContentHolderViewModel> DeleteAllTrash()
        {
            var chsToDelete = context.ContentHolders
                .Where(x => x.IsTrashed)
                .ToList();
            context.ContentHolders.RemoveRange(chsToDelete);
            context.SaveChanges();

            return chsToDelete.Select<ContentHolder, ContentHolderViewModel>(x => x);
        }
    }
}

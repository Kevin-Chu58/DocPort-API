using DocPort.Models.ViewModels;
using DocPort.Static.Names;
using Microsoft.AspNetCore.Mvc;
using static DocPort.Services.ServiceInterfaces;

namespace DocPort.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DirectoryController(IDocsService docsService, IContentHoldersService contentHolderService) : DocPortControllerBase
    {
        /// <summary>
        /// Get all items with directoryID
        /// </summary>
        /// <param name="directoryID">id of the directory</param>
        /// <returns>a list of different items</returns>
        [HttpGet]
        [Route("{directoryID}")]
        public ActionResult<IEnumerable<DirectoryViewModel>> GetByDirectoryID(int directoryID)
        {
            var result = new List<DirectoryViewModel>();
            var docs = docsService.GetDocsByDirectoryID(directoryID);
            var contentHolders = contentHolderService.GetContentHoldersByDirectoryID(directoryID);

            foreach (var doc in docs)
            {
                result.Add(new DirectoryViewModel(DirectoryTypes.DOC, doc));
            }

            foreach (var contentHolder in contentHolders)
            {
                result.Add(new DirectoryViewModel(DirectoryTypes.CONTENT_HOLDER, contentHolder));
            }

            return Ok(result);
        }

        /// <summary>
        /// Get all items from Bin
        /// </summary>
        /// <returns>a list of trashed items</returns>
        [HttpGet]
        [Route("bin")]
        public ActionResult<IEnumerable<DirectoryViewModel>> GetBin()
        {
            var result = new List<DirectoryViewModel>();
            var docs = docsService.GetBin();
            var contentHolders = contentHolderService.GetBin();

            foreach (var doc in docs)
            {
                result.Add(new DirectoryViewModel(DirectoryTypes.DOC, doc));
            }

            foreach (var contentHolder in contentHolders)
            {
                result.Add(new DirectoryViewModel(DirectoryTypes.CONTENT_HOLDER, contentHolder));
            }

            return Ok(result);
        }

        /// <summary>
        /// Get Navigation view model of a Doc or ContentHolder
        /// </summary>
        /// <param name="type"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpGet]
        [Route("nav/type={type}&id={ID}")]
        public ActionResult<DocNavigationViewModel> GetNavigation(string type, int ID)
        {
            DocNavigationViewModel result;
            switch(type)
            {
                case DirectoryTypes.DOC:
                    result = docsService.GetNavigation(ID);
                    break;
                case DirectoryTypes.CONTENT_HOLDER:
                    var ch = contentHolderService.GetContentHolderByID(ID);
                    var doc = docsService.GetDocByID(ch.DirectoryID);
                    result = docsService.GetNavigation(doc.ID);
                    result.Ids?.Add(doc.ID);
                    result.Titles?.Add(doc.Title);
                    result.Title = ch.Title;
                    break;
                default:
                    throw new NotImplementedException();
            }

            return Ok(result);
        }

        /// <summary>
        /// Update the directoryID of an item (Drag & Drop)
        /// </summary>
        /// <param name="directoryID">id of item's directory</param>
        /// <param name="directoryItem">type and id of the item</param>
        /// <returns>the updated item</returns>
        [HttpPatch]
        [Route("{directoryID}")]
        public ActionResult<DirectoryViewModel> UpdateDirectory(int directoryID, [FromBody] DirectoryPatchViewModel directoryItem)
        {
            object content = directoryItem.Type switch
            {
                DirectoryTypes.DOC => docsService.UpdateDirectoryID(directoryItem.ID, directoryID),
                DirectoryTypes.CONTENT_HOLDER => contentHolderService.UpdateDirectoryID(directoryItem.ID, directoryID),
                _ => throw new Exception("Invalid type"),
            };
            var result = new DirectoryViewModel(directoryItem.Type, content);

            return Ok(result);
        }

        /// <summary>
        /// Trash a list of different items
        /// </summary>
        /// <param name="directoryItems">a list of types and ids of items to be updated</param>
        /// <returns>a list of updated items</returns>
        [HttpPatch]
        [Route("trash")]
        public ActionResult<IEnumerable<DirectoryViewModel>> UpdateIsTrashed([FromBody] List<DirectoryPatchViewModel> directoryItems)
        {
            // check type validity first before processing
            CheckItemsValidType(directoryItems);

            var result = new List<DirectoryViewModel>();

            foreach (var directoryItem in directoryItems)
            {
                object content = directoryItem.Type switch
                {
                    DirectoryTypes.DOC => docsService.UpdateDocIsTrashed(directoryItem.ID),
                    DirectoryTypes.CONTENT_HOLDER => contentHolderService.UpdateContentHolderIsTrashed(directoryItem.ID),
                    _ => throw new NotImplementedException()
                };
                result.Add(new DirectoryViewModel(directoryItem.Type, content));
            }

            return Ok(result);
        }

        /// <summary>
        /// Delete a list of different items
        /// </summary>
        /// <param name="directoryItems"> a list of types and ids of items to be deleted</param>
        /// <returns>a list of deleted items</returns>
        [HttpDelete]
        [Route("")]
        public ActionResult<IEnumerable<DirectoryViewModel>> DeleteTrashes([FromBody] List<DirectoryPatchViewModel> directoryItems)
        {
            // check type validity first before processing
            CheckItemsValidType(directoryItems);

            var result = new List<DirectoryViewModel>();

            foreach (var directoryItem in directoryItems)
            {
                object content = directoryItem.Type switch
                {
                    DirectoryTypes.DOC => docsService.DeleteDoc(directoryItem.ID),
                    DirectoryTypes.CONTENT_HOLDER => contentHolderService.DeleteContentHolder(directoryItem.ID),
                    _ => throw new NotImplementedException()
                };
                result.Add(new DirectoryViewModel(directoryItem.Type, content));
            }

            return Ok(result);
        }

        // Delete all docs and content holders
        // (test route)
        [HttpDelete]
        [Route("all")]
        public ActionResult<IEnumerable<DirectoryViewModel>> DeleteAll()
        {
            IEnumerable<DirectoryViewModel> result = [];

            var docsDeleted = docsService.DeleteAllTrash();
            var chDeleted = contentHolderService.DeleteAllTrash();

            result = result.Concat(docsDeleted.Select<DocViewModel, DirectoryViewModel>(x => x));
            result = result.Concat(chDeleted.Select<ContentHolderViewModel, DirectoryViewModel>(x => x));

            return Ok(result);
        }

        private static bool CheckValidType(string type)
        {
            string[] typeCheck = [DirectoryTypes.DOC, DirectoryTypes.CONTENT_HOLDER];
            return typeCheck.Contains(type);
        }

        private static void CheckItemsValidType(List<DirectoryPatchViewModel> directoryItems)
        {
            var directoryTypes = directoryItems.Select(x => x.Type).ToList();
            if (directoryTypes.Any(type => !CheckValidType(type)))
                throw new Exception("Invalid type");
        }
    }
}

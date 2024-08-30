using DocPort.Migrations;
using DocPort.Models;
using DocPort.Models.DocPort.Context;
using DocPort.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.CodeDom;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using static DocPort.Services.ServiceInterfaces;

namespace DocPort.Services
{
    public class DocsService(DocPortContext context) : IDocsService
    {

        /// <summary>
        /// Get a Doc entity by id
        /// </summary>
        /// <param name="docID">id of Doc item</param>
        /// <returns>Doc item with matched id</returns>
        public Doc GetDocByID(int docID, bool throwsException = true)
        {
            var docItem = context.Docs.Find(docID);
            if (throwsException && docItem == null)
            {
                throw new Exception("Doc not found.");
            }

            return docItem;
        }

        public DocViewModel GetDocRestored()
        {
            var title = "Restored";
            var docRestored = context.Docs.FirstOrDefault(x => 
                x.Title == title 
                && x.DirectoryID == 0 
                && x.IsTrashed == false);
            
            if (docRestored == null)
            {
                DocPostViewModel newDoc = new()
                {
                    Title = title,
                    Description = "Restored Docs and PlaceHolder from Bin.",
                };
                docRestored = (Doc)AddNewDoc(newDoc);
            }
            return (DocViewModel)docRestored;
        }

        /// <summary>
        /// Get a list of Docs under a given directory that are not in Bin
        /// </summary>
        /// <param name="docID">ID of Doc</param>
        /// <returns></returns>
        public IEnumerable<DocViewModel> GetDocsByDirectoryID(int directoryID)
        {
            var docItems = context.Docs
                            .Where(doc => doc.IsTrashed == false
                                && doc.DirectoryID == directoryID)
                            .Select<Doc, DocViewModel>(x => x);
            return docItems;
        }

        /// <summary>
        /// Get all eneities of Doc in Bin
        /// </summary>
        /// <returns>a list of Doc view models</returns>
        public IEnumerable<DocViewModel> GetBin()
        {
            var docItems = context.Docs
                            .Where(doc => doc.IsTrashed == true
                                && doc.IsTrashedPrime == true)
                            .Select<Doc, DocViewModel>(x => x);
            return docItems;
        }

        public DocNavigationViewModel GetNavigation(int docID)
        {
            IEnumerable<int> ids = [];
            IEnumerable<string> titles = [];
            bool isPrimeDirectory = true;
            string title = "";
            
            if (docID > 0)
            {
                Doc currentDoc = GetDocByID(docID);
                isPrimeDirectory = false;
                title = currentDoc.Title;

                while (true)
                {

                    if (currentDoc.DirectoryID > 0)
                    {
                        currentDoc = GetDocByID((int)currentDoc.DirectoryID);
                        ids = ids.Prepend(currentDoc.ID);
                        titles = titles.Prepend(currentDoc.Title);
                    }
                    else
                        break;
                }
            }

            // prepend the primary directory
            ids = ids.Prepend(0);
            titles = titles.Prepend("Docs");

            DocNavigationViewModel navObjects = new()
            {
                Ids = ids.ToList(),
                Titles = titles.ToList(),
                IsPrimeDirectory = isPrimeDirectory,
                Title = title,
            };

            return navObjects;
        }

        public DocExplorerViewModel GetDocExplorer(int directoryID)
        {
            List<DocExplorerViewModel> subDocsExp = [.. context.Docs.Where(x => x.DirectoryID == directoryID && !x.IsTrashed)
                .Select(doc => new DocExplorerViewModel
                {
                    ID = doc.ID,
                    Title = doc.Title,
                    SubDocsInside = context.Docs
                        .Where(x => x.DirectoryID == doc.ID)
                        .Count(),
                })];
            
            DocExplorerViewModel docExplorer = new()
            {
                ID = directoryID,
                Title = directoryID == 0 ? 
                    "Docs" :
                    GetDocByID(directoryID).Title,
                Docs = subDocsExp,
                SubDocsInside = subDocsExp.Count,
            };

            /** old version - may pontentially taking too long and too much space
            // initialize DocExplorer
            DocExplorerViewModel docExplorer = new()
            {
                ID = 0,
                Title = "Docs",
                Docs = [],
            };

            // goes through all subsequent Docs
            Queue<IEnumerable<int>> docExpPaths = [];
            Queue<int> docIDs = [];

            // get all untrashed docs only with IDs, Titles, and DirectoryIDs
            // reduces the number of accessing the database
            var docItems = context.Docs.Where(x => !x.IsTrashed).Select(doc => new
            {
                doc.ID, 
                doc.Title,
                doc.DirectoryID,
            });

            // initialize docIDs and docExpPaths
            var curDocs = docItems.Where(x => x.DirectoryID == 0).ToList();
            for(int i = 0; i < curDocs.Count; i++)
            {
                docIDs.Enqueue(curDocs[i].ID);
                docExpPaths.Enqueue([i]);
                docExplorer.Docs.Add(new DocExplorerViewModel
                {
                    ID = curDocs[i].ID,
                    Title = curDocs[i].Title,
                    Docs = [],
                });
            }

            DocExplorerViewModel curDocExplorer;

            while (docIDs.Count > 0)
            {
                var curDocID = docIDs.Dequeue();
                var curPath = docExpPaths.Dequeue();
                curDocExplorer = docExplorer;
                // find the correct DocExplorer
                foreach (var pathIndex in curPath)
                {
                    curDocExplorer = curDocExplorer.Docs[pathIndex];
                }

                curDocs = [.. docItems.Where(x => x.DirectoryID == curDocID)];
                for (int i = 0; i < curDocs.Count; i++)
                {
                    docIDs.Enqueue(curDocs[i].ID);
                    docExpPaths.Enqueue(curPath.Append(i));

                    curDocExplorer.Docs.Add(new DocExplorerViewModel
                    {
                        ID = curDocs[i].ID,
                        Title = curDocs[i].Title,
                        Docs = [],
                    });
                }
            }
            */

            return docExplorer;
        }

        /// <summary>
        /// add a new Doc item to database
        /// </summary>
        /// <param name="newDoc">the Doc entity to be added</param>
        /// <returns>the view model of added Doc</returns>
        public DocViewModel AddNewDoc(DocPostViewModel newDoc)
        {
            var docToAdd = newDoc.ToDoc();

            // check if directory in Bin
            if (docToAdd.DirectoryID > 0 && IsDirectoryInBin((int)docToAdd.DirectoryID))
                throw new Exception("Invalid creation of Doc in Bin.");

            context.Docs.Add(docToAdd);
            context.SaveChanges();
            return (DocViewModel)docToAdd;
        }

        /// <summary>
        /// update the Doc information
        /// </summary>
        /// <param name="doc">Doc view model</param>
        /// <returns>Updated Doc view model</returns>
        /// <exception cref="Exception">Doc not found</exception>
        public DocViewModel UpdateDoc(int docID, DocPostViewModel doc)
        {
            var docToUpdate = GetDocByID(docID);

            docToUpdate.Title = doc.Title ?? docToUpdate.Title;
            docToUpdate.Description = doc.Description ?? docToUpdate.Description;
            docToUpdate.LastTimeUpdated = DateTime.UtcNow;

            context.Docs.Update(docToUpdate);
            context.SaveChanges();

            UpdateDirectoryID(docID, doc.DirectoryID ?? 0);
            docToUpdate.DirectoryID = doc.DirectoryID ?? 0;

            return (DocViewModel)docToUpdate;
        }

        /// <summary>
        /// update Doc isTrashed state
        /// </summary>
        /// <param name="docID">id of a Doc</param>
        /// <returns>a list of Docs updated</returns>
        public IEnumerable<DocViewModel> UpdateDocIsTrashed(int docID)
        {
            // update the doc with id and all of its subsequent docs
            IEnumerable<Doc> docsToUpdate = [GetDocByID(docID)];
            Queue<int> subDocs = new();

            var docToUpdate = docsToUpdate.Last();

            if (docToUpdate.IsTrashed && !docToUpdate.IsTrashedPrime)
                throw new Exception("Cannot restore subsequent Docs from Bin.");

            // if the parent Doc is also in Bin, restore the Doc to primary directory
            
            if (docToUpdate.DirectoryID > 0)
            {
                var docToUpdateDirectory = GetDocByID((int)docToUpdate.DirectoryID, false);

                docToUpdate.DirectoryID = docToUpdateDirectory?.ID ?? 0;
            }
            

            docToUpdate.IsTrashed = !docToUpdate.IsTrashed;
            docToUpdate.IsTrashedPrime = !docToUpdate.IsTrashedPrime;
            
            while(true)
            {
                foreach (var subDoc in context.Docs
                .Where(x => x.DirectoryID == docsToUpdate.Last().ID))
                    subDocs.Enqueue(subDoc.ID);

                if (subDocs.Count > 0)
                {
                    docsToUpdate = docsToUpdate.Append(GetDocByID(subDocs.Dequeue()));
                    docToUpdate = docsToUpdate.Last();
                    if (!docToUpdate.IsTrashedPrime)
                        docToUpdate.IsTrashed = !docToUpdate.IsTrashed;
                }
                else
                    break;
            }

            context.UpdateRange(docsToUpdate);
            context.SaveChanges();

            return docsToUpdate.Select<Doc, DocViewModel>(x => x);
        }

        /// <summary>
        /// update directoryID of a Doc
        /// </summary>
        /// <param name="docID">id of a Doc</param>
        /// <param name="directoryID">directoryID of a Doc</param>
        /// <returns>view model of the updated Doc</returns>
        /// <exception cref="Exception">Invalid directory, invalid directory in Bin, invalid Doc in Bin</exception>
        public DocViewModel UpdateDirectoryID(int docID, int directoryID)
        {

            // check invalid directoryID
            if (directoryID < 0)
                throw new Exception("Invalid directory.");

            // neither can update directoryID of a Doc in Bin, nor
            // can update to a directory that is trashed

            var docToUpdate = GetDocByID(docID);
            if (docToUpdate.IsTrashed)
                throw new Exception("Invalid directory update to a Doc in Bin.");

            if (directoryID > 0)
            {
                var directoryForUpdate = GetDocByID(directoryID);
                if (directoryForUpdate.IsTrashed)
                    throw new Exception("Invalid directory in Bin.");
            }

            // cannot create/update a Doc causing a directory loop
            if (DoesDirectoryLoop(docID, directoryID))
                throw new Exception("Directory loop detected.");
            
            // update directoryID
            docToUpdate.DirectoryID = directoryID;
            context.Update(docToUpdate);
            context.SaveChanges();

            return (DocViewModel)docToUpdate;
        }

        /// <summary>
        /// Delete a trashed Doc entity and its subsequent docs by id
        /// </summary>
        /// <param name="docID">id of Doc item</param>
        /// <returns>the Doc entity to be deleted</returns>
        /// <exception cref="Exception">Doc is not in Bin</exception>
        public IEnumerable<DocViewModel> DeleteDoc(int docID) 
        {
            IEnumerable<Doc> docsToDelete = [GetDocByID(docID)];
            Queue<int> directoryIDs = new();
            directoryIDs.Enqueue(docID);

            while (directoryIDs.Count > 0)
            {
                int directoryID = directoryIDs.Dequeue();
                Doc doc = GetDocByID(directoryID);

                if (!doc.IsTrashed)
                    throw new Exception("Doc is not in Bin.");

                if (!doc.IsTrashedPrime)
                    docsToDelete = docsToDelete.Append(doc);

                foreach (var subDoc in context.Docs.Where(x => x.DirectoryID == doc.ID))
                    directoryIDs.Enqueue(subDoc.ID);
            }

            context.Docs.RemoveRange(docsToDelete);
            context.SaveChanges();

            return docsToDelete.Select<Doc, DocViewModel>(x => x);
        }

        /// <summary>
        /// Delete all Docs in Bin
        /// </summary>
        /// <returns>a list of delete Docs</returns>
        public IEnumerable<DocViewModel> DeleteAllTrash()
        {
            var docsToDelete = context.Docs
                .Where(x => x.IsTrashed)
                .ToList();
            context.RemoveRange(docsToDelete);
            context.SaveChanges();

            return docsToDelete.Select<Doc, DocViewModel>(x => x);
        }

        public bool IsDirectoryInBin(int directoryID)
        {
            return GetDocByID(directoryID).IsTrashed;
        }

        private bool DoesDirectoryLoop(int docID, int directoryID)
        {
            List<int> path = [docID];
            while (directoryID > 0)
            {
                if (path.Contains(directoryID))
                {
                    return true;
                }
                else
                {
                    path.Add(directoryID);
                    directoryID = (int)GetDocByID(directoryID).DirectoryID;
                }
            }
            return false;
        }

        // preparation for the next huge update after basic user account construction
        // when a new user account is created,
        // auto-generate a Doc with directory = null as the root directory for that user
        // this method gets you the ID of the root directory
        private int GetRootDirectory()
        {
            var rootDirectory = context.Docs.First(x => x.DirectoryID == null);
            return rootDirectory.ID;
        }
    }
}

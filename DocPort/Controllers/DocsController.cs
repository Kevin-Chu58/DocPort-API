using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DocPort.Models;
using DocPort.Models.DocPort.Context;
using DocPort.Services;
using static DocPort.Services.ServiceInterfaces;
using DocPort.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;

namespace DocPort.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocsController(IDocsService docsService) : DocPortControllerBase
    { 

        /// <summary>
        /// Add a new Doc
        /// </summary>
        /// <param name="newDoc">the Doc viewmodel about to add</param>
        /// <returns>the added Doc item</returns>
        [HttpPost]
        [Route("")]
        public ActionResult<DocViewModel> Add([FromBody] DocPostViewModel newDoc)
        {
            var result = docsService.AddNewDoc(newDoc);
            return Ok(result);
        }

        /// <summary>
        /// Update a Doc information
        /// </summary>
        /// <param name="doc">view model of a Doc</param>
        /// <returns>the updated Doc item</returns>
        [HttpPatch]
        [Route("{docID}")]
        public ActionResult<DocViewModel> Update(int docID, [FromBody] DocPostViewModel doc)
        {
            var result = docsService.UpdateDoc(docID, doc);
            return Ok(result);
        }

        /// <summary>
        /// Get explorer of a doc
        /// </summary>
        /// <param name="docID">id of a Doc</param>
        /// <returns>the Doc Explorer item</returns>
        [HttpGet]
        [Route("explorer/{docID}")]
        public ActionResult<DocExplorerViewModel> GetExplorer(int docID)
        {
            var result = docsService.GetDocExplorer(docID);
            return Ok(result);
        }
    }
}

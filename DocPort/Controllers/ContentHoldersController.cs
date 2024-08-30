using DocPort.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using static DocPort.Services.ServiceInterfaces;

namespace DocPort.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentHoldersController(IContentHoldersService contentHoldersService) : DocPortControllerBase
    {
        [HttpPost]
        [Route("")]
        public ActionResult<ContentHolderViewModel> Add([FromBody] ContentHolderPostViewModel newCh)
        {
            var result = contentHoldersService.AddNewContentHolder(newCh);
            return Ok(result);
        }

        [HttpPatch]
        [Route("{chID}")]
        public ActionResult<ContentHolderViewModel> Update(int chID, [FromBody] ContentHolderPostViewModel ch)
        {
            var result = contentHoldersService.UpdateContentHolder(chID, ch);
            return Ok(result);
        }
    }
}

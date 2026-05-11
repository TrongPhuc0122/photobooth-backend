using Microsoft.AspNetCore.Mvc;
using Shared.Results;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController : ControllerBase
    {
        protected IActionResult ToActionResult(ServiceResult result)
        {
            return result.ResultType switch
            {
                ServiceResultType.Success => Ok(new { result.Message }),
                ServiceResultType.Created => StatusCode(StatusCodes.Status201Created, new { result.Message }),
                ServiceResultType.NoContent => NoContent(),
                ServiceResultType.NotFound => NotFound(new { result.Message }),
                ServiceResultType.BadRequest => BadRequest(new { result.Message }),
                ServiceResultType.ValidationError => BadRequest(new { result.Message }),
                ServiceResultType.Unauthorized => Unauthorized(new { result.Message }),
                ServiceResultType.Forbidden => Forbid(),
                ServiceResultType.Conflict => Conflict(new { result.Message }),
                ServiceResultType.InternalServerError => StatusCode(500, new { result.Message }),
                _ => StatusCode(500, new { result.Message })
            };
        }

        protected IActionResult ToActionResult<T>(ServiceResult<T> result)
        {
            return result.ResultType switch
            {
                ServiceResultType.Success => Ok(result.Data),
                ServiceResultType.Created => StatusCode(StatusCodes.Status201Created, result.Data),
                ServiceResultType.NoContent => NoContent(),
                ServiceResultType.NotFound => NotFound(new { result.Message }),
                ServiceResultType.BadRequest => BadRequest(new { result.Message }),
                ServiceResultType.ValidationError => BadRequest(new { result.Message }),
                ServiceResultType.Unauthorized => Unauthorized(new { result.Message }),
                ServiceResultType.Forbidden => Forbid(),
                ServiceResultType.Conflict => Conflict(new { result.Message }),
                ServiceResultType.InternalServerError => StatusCode(500, new { result.Message }),
                _ => StatusCode(500, new { result.Message })
            };
        }
    }
}
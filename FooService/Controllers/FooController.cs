using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FooService.Controllers;

[ApiController, Route("foo")]
public class FooController : Controller
{
    [HttpGet("bar")]
    public IActionResult Bar()
    {
        return new JsonResult(new {x = 1});
    }
}
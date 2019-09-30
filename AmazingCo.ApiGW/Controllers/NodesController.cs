using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace AmazingCo.ApiGW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NodesController : ControllerBase
    {
        
        [HttpGet("{id}/getChildren")]
        public string GetChildren(int id)
        {
            return string.Empty;
        }
        
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {

        }
    }
}

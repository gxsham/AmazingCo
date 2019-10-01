using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AmazingCo.Business;
using AmazingCo.Models;

namespace AmazingCo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NodesController : ControllerBase
    {
        private readonly INodeBusiness _business;

        public NodesController(INodeBusiness nodeBusiness)
        {
            _business = nodeBusiness;
        }

        [HttpGet("{parentId}/getChildren")]
        public async Task<ActionResult<IEnumerable<Node>>> GetChildren(string parentId)
        {
            try
            {
                return await _business.GetChildren(parentId).ToListAsync();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ChangeParent(string id, string parentId)
        {
            try
            {
                await _business.ChangeParentAsync(id, parentId);
            }
            catch(ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return NoContent();
        }
    }
}

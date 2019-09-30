using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AmazingCo.Models;
using AmazingCo.Business;

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
            catch (Exception ex)
            {
                if (!await _business.NodeExists(parentId))
                {
                    return NotFound($"Node with id: {parentId} not found.");
                }
                return BadRequest(ex);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ChangeParent(string id, string parentId)
        {
            try
            {
                await _business.ChangeParentAsync(id, parentId);
            }
            catch (Exception ex)
            {
                if (!await _business.NodeExists(id))
                {
                    return NotFound($"Node with id: {id} not found.");
                }
                if (!await _business.NodeExists(parentId))
                {
                    return NotFound($"Node with id: {parentId} not found.");
                }

                return BadRequest(ex);
            }

            return NoContent();
        }
    }
}

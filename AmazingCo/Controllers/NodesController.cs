using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AmazingCo.Models;
using AmazingCo.Business;
using AmazingCo.Middlewares;
using System.Net;

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
                    throw new StatusCodeException(HttpStatusCode.NotFound, Format(parentId, ex));
                }
                throw ex;
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
                    throw new StatusCodeException(HttpStatusCode.NotFound, Format(id, ex));
                }
                if (!await _business.NodeExists(parentId))
                {
                    throw new StatusCodeException(HttpStatusCode.NotFound, Format(id, ex));
                }
            }

            return NoContent();
        }

        private string Format(string id, Exception exception)
        {
            return $"NodeId: {id}, Message: {exception.Message}";
        }
    }
}

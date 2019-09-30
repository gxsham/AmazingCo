﻿using System.Threading.Tasks;
using AmazingCo.ApiGW.Clients;
using Microsoft.AspNetCore.Mvc;

namespace AmazingCo.ApiGW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NodesController : ControllerBase
    {
        private readonly INodeClient _nodeClient;
        public NodesController(INodeClient nodeClient)
        {
            _nodeClient = nodeClient ?? throw new System.ArgumentNullException(nameof(nodeClient));
        }
        
        [HttpGet("{id}/getChildren")]
        public async Task<IActionResult> GetChildren(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Parent id cannot be null.");
            }
            var response = await _nodeClient.GetChildrenAsync(id);
            if (response.IsSuccessStatusCode)
            {
                return Ok(await response.Content.ReadAsStringAsync());
            } 
            
            return BadRequest(response);
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, string parentId)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Node id cannot be null.");
            }
            var response = await _nodeClient.ChangeParentAsync(id, parentId);
            if (response.IsSuccessStatusCode)
            {
                return NoContent();
            }

            return BadRequest(response);
        }
    }
}

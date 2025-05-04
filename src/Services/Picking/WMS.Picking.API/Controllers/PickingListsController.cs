using Microsoft.AspNetCore.Mvc;
using WMS.Picking.Application.Commands;
using WMS.Picking.Application.DTOs;
using WMS.Picking.Application.Services;

namespace WMS.Picking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PickingListsController : ControllerBase
    {
        private readonly IPickingService _pickingService;
        private readonly ILogger<PickingListsController> _logger;

        public PickingListsController(IPickingService pickingService, ILogger<PickingListsController> logger)
        {
            _pickingService = pickingService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PickingListDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PickingListDto>>> GetPickingLists(
            [FromQuery] int pageSize = 10, 
            [FromQuery] int pageNumber = 1,
            [FromQuery] string status = null)
        {
            var result = await _pickingService.GetPickingListsAsync(pageSize, pageNumber, status);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PickingListDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PickingListDto>> GetPickingList(Guid id)
        {
            var result = await _pickingService.GetPickingListByIdAsync(id);
            
            if (result == null)
                return NotFound();
                
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(PickingListDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PickingListDto>> CreatePickingList(CreatePickingListCommand command)
        {
            var result = await _pickingService.CreatePickingListAsync(command);
            
            return CreatedAtAction(nameof(GetPickingList), new { id = result.Id }, result);
        }

        [HttpPost("{id}/items")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddPickingListItem(Guid id, AddPickingListItemCommand command)
        {
            if (id != command.PickingListId)
                return BadRequest();
                
            await _pickingService.AddPickingListItemAsync(command);
            
            return NoContent();
        }

        [HttpPut("{id}/items/{sku}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePickingListItem(Guid id, string sku, UpdatePickingListItemCommand command)
        {
            if (id != command.PickingListId || sku != command.Sku)
                return BadRequest();
                
            await _pickingService.UpdatePickingListItemAsync(command);
            
            return NoContent();
        }

        [HttpDelete("{id}/items/{sku}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemovePickingListItem(Guid id, string sku)
        {
            await _pickingService.RemovePickingListItemAsync(id, sku);
            
            return NoContent();
        }

        [HttpPost("{id}/release")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ReleasePickingList(Guid id)
        {
            await _pickingService.ReleasePickingListAsync(id);
            
            return NoContent();
        }

        [HttpPost("{id}/assign")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AssignPickingList(Guid id, [FromBody] string userId)
        {
            await _pickingService.AssignPickingListAsync(id, userId);
            
            return NoContent();
        }

        [HttpPost("{id}/unassign")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UnassignPickingList(Guid id)
        {
            await _pickingService.UnassignPickingListAsync(id);
            
            return NoContent();
        }

        [HttpPost("{id}/complete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CompletePickingList(Guid id)
        {
            await _pickingService.CompletePickingListAsync(id);
            
            return NoContent();
        }

        [HttpPost("{id}/cancel")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CancelPickingList(Guid id, [FromBody] string reason)
        {
            await _pickingService.CancelPickingListAsync(id, reason);
            
            return NoContent();
        }

        [HttpPost("{id}/priority")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePickingListPriority(Guid id, [FromBody] string priority)
        {
            await _pickingService.UpdatePickingListPriorityAsync(id, priority);
            
            return NoContent();
        }

        [HttpPost("{id}/items/{sku}/pick")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RecordPick(Guid id, string sku, RecordPickCommand command)
        {
            if (id != command.PickingListId || sku != command.Sku)
                return BadRequest();
                
            await _pickingService.RecordPickAsync(command);
            
            return NoContent();
        }

        [HttpPost("{id}/items/{sku}/short")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MarkItemAsShort(Guid id, string sku, [FromBody] string reason)
        {
            await _pickingService.MarkItemAsShortAsync(id, sku, reason);
            
            return NoContent();
        }
    }
}
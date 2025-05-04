using Microsoft.AspNetCore.Mvc;
using WMS.Inventory.Application.Commands;
using WMS.Inventory.Application.DTOs;
using WMS.Inventory.Application.Services;

namespace WMS.Inventory.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryItemsController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<InventoryItemsController> _logger;

        public InventoryItemsController(IInventoryService inventoryService, ILogger<InventoryItemsController> logger)
        {
            _inventoryService = inventoryService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<InventoryItemDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetInventoryItems(
            [FromQuery] int pageSize = 10, 
            [FromQuery] int pageNumber = 1,
            [FromQuery] string searchTerm = "")
        {
            var result = await _inventoryService.GetInventoryItemsAsync(pageSize, pageNumber, searchTerm);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(InventoryItemDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InventoryItemDto>> GetInventoryItem(Guid id)
        {
            var result = await _inventoryService.GetInventoryItemByIdAsync(id);
            
            if (result == null)
                return NotFound();
                
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(InventoryItemDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<InventoryItemDto>> CreateInventoryItem(CreateInventoryItemCommand command)
        {
            var result = await _inventoryService.CreateInventoryItemAsync(command);
            
            return CreatedAtAction(nameof(GetInventoryItem), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateInventoryItem(Guid id, UpdateInventoryItemCommand command)
        {
            if (id != command.Id)
                return BadRequest();
                
            await _inventoryService.UpdateInventoryItemAsync(command);
            
            return NoContent();
        }

        [HttpPost("{id}/add-stock")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddStock(Guid id, AddStockCommand command)
        {
            if (id != command.InventoryItemId)
                return BadRequest();
                
            await _inventoryService.AddStockAsync(command);
            
            return NoContent();
        }

        [HttpPost("{id}/remove-stock")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveStock(Guid id, RemoveStockCommand command)
        {
            if (id != command.InventoryItemId)
                return BadRequest();
                
            await _inventoryService.RemoveStockAsync(command);
            
            return NoContent();
        }

        [HttpPost("{id}/allocate-stock")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AllocateStock(Guid id, AllocateStockCommand command)
        {
            if (id != command.InventoryItemId)
                return BadRequest();
                
            await _inventoryService.AllocateStockAsync(command);
            
            return NoContent();
        }

        [HttpPost("{id}/deallocate-stock")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeallocateStock(Guid id, DeallocateStockCommand command)
        {
            if (id != command.InventoryItemId)
                return BadRequest();
                
            await _inventoryService.DeallocateStockAsync(command);
            
            return NoContent();
        }
    }
}
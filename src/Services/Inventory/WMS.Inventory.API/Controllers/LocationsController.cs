using Microsoft.AspNetCore.Mvc;
using WMS.Inventory.Application.Commands;
using WMS.Inventory.Application.DTOs;
using WMS.Inventory.Application.Services;

namespace WMS.Inventory.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationService _locationService;
        private readonly ILogger<LocationsController> _logger;

        public LocationsController(ILocationService locationService, ILogger<LocationsController> logger)
        {
            _locationService = locationService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<LocationDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<LocationDto>>> GetLocations(
            [FromQuery] int pageSize = 10, 
            [FromQuery] int pageNumber = 1,
            [FromQuery] string searchTerm = "")
        {
            var result = await _locationService.GetLocationsAsync(pageSize, pageNumber, searchTerm);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(LocationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LocationDto>> GetLocation(Guid id)
        {
            var result = await _locationService.GetLocationByIdAsync(id);
            
            if (result == null)
                return NotFound();
                
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(LocationDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<LocationDto>> CreateLocation(CreateLocationCommand command)
        {
            var result = await _locationService.CreateLocationAsync(command);
            
            return CreatedAtAction(nameof(GetLocation), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateLocation(Guid id, UpdateLocationCommand command)
        {
            if (id != command.Id)
                return BadRequest();
                
            await _locationService.UpdateLocationAsync(command);
            
            return NoContent();
        }

        [HttpPost("{id}/activate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ActivateLocation(Guid id)
        {
            await _locationService.ActivateLocationAsync(id);
            
            return NoContent();
        }

        [HttpPost("{id}/deactivate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeactivateLocation(Guid id)
        {
            await _locationService.DeactivateLocationAsync(id);
            
            return NoContent();
        }

        [HttpPost("{id}/add-stock")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddStockToLocation(Guid id, AddStockToLocationCommand command)
        {
            if (id != command.LocationId)
                return BadRequest();
                
            await _locationService.AddStockToLocationAsync(command);
            
            return NoContent();
        }

        [HttpPost("{id}/remove-stock")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveStockFromLocation(Guid id, RemoveStockFromLocationCommand command)
        {
            if (id != command.LocationId)
                return BadRequest();
                
            await _locationService.RemoveStockFromLocationAsync(command);
            
            return NoContent();
        }

        [HttpGet("{id}/stock")]
        [ProducesResponseType(typeof(IEnumerable<StockOnHandDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<StockOnHandDto>>> GetLocationStock(Guid id)
        {
            var result = await _locationService.GetLocationStockAsync(id);
            
            return Ok(result);
        }
    }
}
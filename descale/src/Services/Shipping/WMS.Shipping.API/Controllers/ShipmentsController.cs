using Microsoft.AspNetCore.Mvc;

namespace WMS.Shipping.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShipmentsController : ControllerBase
    {
        private readonly IShippingService _shippingService;
        private readonly ILogger<ShipmentsController> _logger;

        public ShipmentsController(IShippingService shippingService, ILogger<ShipmentsController> logger)
        {
            _shippingService = shippingService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ShipmentDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ShipmentDto>>> GetShipments(
            [FromQuery] int pageSize = 10, 
            [FromQuery] int pageNumber = 1,
            [FromQuery] string status = null)
        {
            var result = await _shippingService.GetShipmentsAsync(pageSize, pageNumber, status);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ShipmentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ShipmentDto>> GetShipment(Guid id)
        {
            var result = await _shippingService.GetShipmentByIdAsync(id);
            
            if (result == null)
                return NotFound();
                
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ShipmentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ShipmentDto>> CreateShipment(CreateShipmentCommand command)
        {
            var result = await _shippingService.CreateShipmentAsync(command);
            
            return CreatedAtAction(nameof(GetShipment), new { id = result.Id }, result);
        }

        [HttpPost("{id}/items")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddShipmentItem(Guid id, AddShipmentItemCommand command)
        {
            if (id != command.ShipmentId)
                return BadRequest();
                
            await _shippingService.AddShipmentItemAsync(command);
            
            return NoContent();
        }

        [HttpPut("{id}/items/{sku}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateShipmentItem(Guid id, string sku, UpdateShipmentItemCommand command)
        {
            if (id != command.ShipmentId || sku != command.Sku)
                return BadRequest();
                
            await _shippingService.UpdateShipmentItemAsync(command);
            
            return NoContent();
        }

        [HttpDelete("{id}/items/{sku}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveShipmentItem(Guid id, string sku)
        {
            await _shippingService.RemoveShipmentItemAsync(id, sku);
            
            return NoContent();
        }

        [HttpPost("{id}/ready")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ReadyForProcessing(Guid id)
        {
            await _shippingService.ReadyForProcessingAsync(id);
            
            return NoContent();
        }
        
        [HttpPost("{id}/process")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> StartProcessing(Guid id)
        {
            await _shippingService.StartProcessingAsync(id);
            
            return NoContent();
        }

        [HttpPost("{id}/packages")]
        [ProducesResponseType(typeof(ShipmentPackageDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ShipmentPackageDto>> AddPackage(Guid id, AddPackageCommand command)
        {
            if (id != command.ShipmentId)
                return BadRequest();
                
            var result = await _shippingService.AddPackageAsync(command);
            
            return CreatedAtAction(nameof(GetPackage), new { id = id, packageId = result.Id }, result);
        }

        [HttpGet("{id}/packages/{packageId}")]
        [ProducesResponseType(typeof(ShipmentPackageDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ShipmentPackageDto>> GetPackage(Guid id, Guid packageId)
        {
            var result = await _shippingService.GetPackageByIdAsync(id, packageId);
            
            if (result == null)
                return NotFound();
                
            return Ok(result);
        }

        [HttpDelete("{id}/packages/{packageId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemovePackage(Guid id, Guid packageId)
        {
            await _shippingService.RemovePackageAsync(id, packageId);
            
            return NoContent();
        }

        [HttpPost("{id}/packages/{packageId}/items")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddItemToPackage(Guid id, Guid packageId, AddItemToPackageCommand command)
        {
            if (id != command.ShipmentId || packageId != command.PackageId)
                return BadRequest();
                
            await _shippingService.AddItemToPackageAsync(command);
            
            return NoContent();
        }

        [HttpPost("{id}/packages/{packageId}/seal")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SealPackage(Guid id, Guid packageId)
        {
            await _shippingService.SealPackageAsync(id, packageId);
            
            return NoContent();
        }

        [HttpPost("{id}/packages/{packageId}/label")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> LabelPackage(Guid id, Guid packageId, [FromBody] string trackingNumber)
        {
            await _shippingService.LabelPackageAsync(id, packageId, trackingNumber);
            
            return NoContent();
        }

        [HttpPost("{id}/ship")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ShipShipment(Guid id, ShipShipmentCommand command)
        {
            if (id != command.ShipmentId)
                return BadRequest();
                
            await _shippingService.ShipShipmentAsync(command);
            
            return NoContent();
        }

        [HttpPost("{id}/cancel")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CancelShipment(Guid id, [FromBody] string reason)
        {
            await _shippingService.CancelShipmentAsync(id, reason);
            
            return NoContent();
        }

        [HttpPut("{id}/address")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateShippingAddress(Guid id, UpdateShippingAddressCommand command)
        {
            if (id != command.ShipmentId)
                return BadRequest();
                
            await _shippingService.UpdateShippingAddressAsync(command);
            
            return NoContent();
        }

        [HttpPut("{id}/method")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateShippingMethod(Guid id, [FromBody] string shippingMethod)
        {
            await _shippingService.UpdateShippingMethodAsync(id, shippingMethod);
            
            return NoContent();
        }
    }
}
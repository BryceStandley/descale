using Microsoft.AspNetCore.Mvc;

namespace descale.Services.Receiving.WMS.Receiving.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseOrdersController : ControllerBase
    {
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly ILogger<PurchaseOrdersController> _logger;

        public PurchaseOrdersController(IPurchaseOrderService purchaseOrderService, ILogger<PurchaseOrdersController> logger)
        {
            _purchaseOrderService = purchaseOrderService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PurchaseOrderDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PurchaseOrderDto>>> GetPurchaseOrders(
            [FromQuery] int pageSize = 10, 
            [FromQuery] int pageNumber = 1,
            [FromQuery] string status = null)
        {
            var result = await _purchaseOrderService.GetPurchaseOrdersAsync(pageSize, pageNumber, status);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PurchaseOrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PurchaseOrderDto>> GetPurchaseOrder(Guid id)
        {
            var result = await _purchaseOrderService.GetPurchaseOrderByIdAsync(id);
            
            if (result == null)
                return NotFound();
                
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(PurchaseOrderDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PurchaseOrderDto>> CreatePurchaseOrder(CreatePurchaseOrderCommand command)
        {
            var result = await _purchaseOrderService.CreatePurchaseOrderAsync(command);
            
            return CreatedAtAction(nameof(GetPurchaseOrder), new { id = result.Id }, result);
        }

        [HttpPost("{id}/lines")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddOrderLine(Guid id, AddPurchaseOrderLineCommand command)
        {
            if (id != command.PurchaseOrderId)
                return BadRequest();
                
            await _purchaseOrderService.AddOrderLineAsync(command);
            
            return NoContent();
        }

        [HttpPut("{id}/lines/{sku}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateOrderLine(Guid id, string sku, UpdatePurchaseOrderLineCommand command)
        {
            if (id != command.PurchaseOrderId || sku != command.Sku)
                return BadRequest();
                
            await _purchaseOrderService.UpdateOrderLineAsync(command);
            
            return NoContent();
        }

        [HttpDelete("{id}/lines/{sku}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveOrderLine(Guid id, string sku)
        {
            await _purchaseOrderService.RemoveOrderLineAsync(id, sku);
            
            return NoContent();
        }

        [HttpPost("{id}/confirm")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ConfirmPurchaseOrder(Guid id)
        {
            await _purchaseOrderService.ConfirmPurchaseOrderAsync(id);
            
            return NoContent();
        }

        [HttpPost("{id}/cancel")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CancelPurchaseOrder(Guid id, [FromBody] string reason)
        {
            await _purchaseOrderService.CancelPurchaseOrderAsync(id, reason);
            
            return NoContent();
        }

        [HttpPost("{id}/receipts")]
        [ProducesResponseType(typeof(ReceiptDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ReceiptDto>> CreateReceipt(Guid id, CreateReceiptCommand command)
        {
            if (id != command.PurchaseOrderId)
                return BadRequest();
                
            var result = await _purchaseOrderService.CreateReceiptAsync(command);
            
            return CreatedAtAction(nameof(GetReceipt), new { id = result.Id }, result);
        }

        [HttpGet("{id}/receipts")]
        [ProducesResponseType(typeof(IEnumerable<ReceiptDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ReceiptDto>>> GetPurchaseOrderReceipts(Guid id)
        {
            var result = await _purchaseOrderService.GetPurchaseOrderReceiptsAsync(id);
            
            return Ok(result);
        }

        [HttpGet("receipts/{id}")]
        [ProducesResponseType(typeof(ReceiptDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ReceiptDto>> GetReceipt(Guid id)
        {
            var result = await _purchaseOrderService.GetReceiptByIdAsync(id);
            
            if (result == null)
                return NotFound();
                
            return Ok(result);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WMS.BuildingBlocks.EventBus;
using WMS.Receiving.Application.Commands;
using WMS.Receiving.Application.DTOs;
using WMS.Receiving.Application.IntegrationEvents.Events;
using WMS.Receiving.Domain.Entities;
using WMS.Receiving.Domain.Repositories;

namespace WMS.Receiving.Application.Services
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;
        private readonly IReceiptRepository _receiptRepository;
        private readonly IEventBus _eventBus;
        private readonly ILogger<PurchaseOrderService> _logger;

        public PurchaseOrderService(
            IPurchaseOrderRepository purchaseOrderRepository,
            IReceiptRepository receiptRepository,
            IEventBus eventBus,
            ILogger<PurchaseOrderService> logger)
        {
            _purchaseOrderRepository = purchaseOrderRepository;
            _receiptRepository = receiptRepository;
            _eventBus = eventBus;
            _logger = logger;
        }

        public async Task<IEnumerable<PurchaseOrderDto>> GetPurchaseOrdersAsync(int pageSize, int pageNumber, string status)
        {
            var spec = new PurchaseOrdersWithPaginationSpecification(pageSize, pageNumber, status);
            var purchaseOrders = await _purchaseOrderRepository.ListAsync(spec);
            
            return purchaseOrders.Select(MapToDto);
        }

        public async Task<PurchaseOrderDto> GetPurchaseOrderByIdAsync(Guid id)
        {
            var spec = new PurchaseOrderWithOrderLinesSpecification(id);
            var purchaseOrder = await _purchaseOrderRepository.FirstOrDefaultAsync(spec);
            
            if (purchaseOrder == null)
                return null;
                
            return MapToDto(purchaseOrder);
        }

        public async Task<PurchaseOrderDto> CreatePurchaseOrderAsync(CreatePurchaseOrderCommand command)
        {
            var nextNumber = await _purchaseOrderRepository.GetNextPurchaseOrderNumberAsync();
            
            var purchaseOrder = new PurchaseOrder(
                nextNumber,
                command.VendorId,
                command.VendorName,
                command.OrderDate,
                command.ExpectedDeliveryDate,
                command.Notes);
                
            await _purchaseOrderRepository.AddAsync(purchaseOrder);
            
            _logger.LogInformation("Purchase order {Number} created with ID {Id}", nextNumber, purchaseOrder.Id);
            
            return MapToDto(purchaseOrder);
        }

        public async Task AddOrderLineAsync(AddPurchaseOrderLineCommand command)
        {
            var purchaseOrder = await _purchaseOrderRepository.GetByIdAsync(command.PurchaseOrderId);
            
            if (purchaseOrder == null)
                throw new Exception($"Purchase order with ID {command.PurchaseOrderId} not found.");
                
            purchaseOrder.AddOrderLine(
                command.Sku,
                command.ItemName,
                command.Quantity,
                command.UnitPrice);
                
            await _purchaseOrderRepository.UpdateAsync(purchaseOrder);
            
            _logger.LogInformation("Order line {Sku} added to purchase order {Id}", command.Sku, command.PurchaseOrderId);
        }

        public async Task UpdateOrderLineAsync(UpdatePurchaseOrderLineCommand command)
        {
            var purchaseOrder = await _purchaseOrderRepository.GetByIdAsync(command.PurchaseOrderId);
            
            if (purchaseOrder == null)
                throw new Exception($"Purchase order with ID {command.PurchaseOrderId} not found.");
                
            purchaseOrder.UpdateOrderLine(command.Sku, command.Quantity, command.UnitPrice);
            
            await _purchaseOrderRepository.UpdateAsync(purchaseOrder);
            
            _logger.LogInformation("Order line {Sku} updated in purchase order {Id}", command.Sku, command.PurchaseOrderId);
        }

        public async Task RemoveOrderLineAsync(Guid purchaseOrderId, string sku)
        {
            var purchaseOrder = await _purchaseOrderRepository.GetByIdAsync(purchaseOrderId);
            
            if (purchaseOrder == null)
                throw new Exception($"Purchase order with ID {purchaseOrderId} not found.");
                
            purchaseOrder.RemoveOrderLine(sku);
            
            await _purchaseOrderRepository.UpdateAsync(purchaseOrder);
            
            _logger.LogInformation("Order line {Sku} removed from purchase order {Id}", sku, purchaseOrderId);
        }

        public async Task ConfirmPurchaseOrderAsync(Guid purchaseOrderId)
        {
            var purchaseOrder = await _purchaseOrderRepository.GetByIdAsync(purchaseOrderId);
            
            if (purchaseOrder == null)
                throw new Exception($"Purchase order with ID {purchaseOrderId} not found.");
                
            purchaseOrder.Confirm();
            
            await _purchaseOrderRepository.UpdateAsync(purchaseOrder);
            
            await _eventBus.PublishAsync(new PurchaseOrderConfirmedIntegrationEvent(
                purchaseOrderId,
                purchaseOrder.Number,
                purchaseOrder.VendorId,
                purchaseOrder.VendorName));
            
            _logger.LogInformation("Purchase order {Id} confirmed", purchaseOrderId);
        }

        public async Task CancelPurchaseOrderAsync(Guid purchaseOrderId, string reason)
        {
            var purchaseOrder = await _purchaseOrderRepository.GetByIdAsync(purchaseOrderId);
            
            if (purchaseOrder == null)
                throw new Exception($"Purchase order with ID {purchaseOrderId} not found.");
                
            purchaseOrder.Cancel(reason);
            
            await _purchaseOrderRepository.UpdateAsync(purchaseOrder);
            
            await _eventBus.PublishAsync(new PurchaseOrderCancelledIntegrationEvent(
                purchaseOrderId,
                purchaseOrder.Number,
                reason));
            
            _logger.LogInformation("Purchase order {Id} cancelled. Reason: {Reason}", purchaseOrderId, reason);
        }

        public async Task<ReceiptDto> CreateReceiptAsync(CreateReceiptCommand command)
        {
            var purchaseOrder = await _purchaseOrderRepository.GetByIdAsync(command.PurchaseOrderId);
            
            if (purchaseOrder == null)
                throw new Exception($"Purchase order with ID {command.PurchaseOrderId} not found.");
                
            var nextReceiptNumber = await _receiptRepository.GetNextReceiptNumberAsync();
            
            var receipt = purchaseOrder.AddReceipt(
                nextReceiptNumber,
                command.ReceivedDate,
                command.ReceivedBy,
                command.Notes);
                
            // Add receipt lines from the command
            foreach (var line in command.Lines)
            {
                var orderLine = purchaseOrder.OrderLines.FirstOrDefault(ol => ol.Sku == line.Sku);
                if (orderLine == null)
                    throw new Exception($"Order line with SKU {line.Sku} not found in purchase order {command.PurchaseOrderId}");
                    
                receipt.AddReceiptLine(
                    line.Sku,
                    orderLine.ItemName, 
                    orderLine.Quantity,  // Expected quantity from order
                    line.QuantityReceived,
                    line.LocationCode);
            }
            
            await _purchaseOrderRepository.UpdateAsync(purchaseOrder);
            
            _logger.LogInformation("Receipt {Number} created for purchase order {Id}", nextReceiptNumber, command.PurchaseOrderId);
            
            return MapToDto(receipt);
        }

        public async Task<IEnumerable<ReceiptDto>> GetPurchaseOrderReceiptsAsync(Guid purchaseOrderId)
        {
            var spec = new PurchaseOrderWithReceiptsSpecification(purchaseOrderId);
            var purchaseOrder = await _purchaseOrderRepository.FirstOrDefaultAsync(spec);
            
            if (purchaseOrder == null)
                return Enumerable.Empty<ReceiptDto>();
                
            return purchaseOrder.Receipts.Select(MapToDto);
        }

        public async Task<ReceiptDto> GetReceiptByIdAsync(Guid receiptId)
        {
            var spec = new ReceiptWithLinesSpecification(receiptId);
            var receipt = await _receiptRepository.FirstOrDefaultAsync(spec);
            
            if (receipt == null)
                return null;
                
            return MapToDto(receipt);
        }

        public async Task CompleteReceiptAsync(Guid receiptId)
        {
            var receipt = await _receiptRepository.GetByIdAsync(receiptId);
            
            if (receipt == null)
                throw new Exception($"Receipt with ID {receiptId} not found.");
                
            receipt.Complete();
            
            await _receiptRepository.UpdateAsync(receipt);
            
            // Publish event that will update inventory
            await _eventBus.PublishAsync(new ReceiptCompletedIntegrationEvent(
                receipt.Id,
                receipt.ReceiptNumber,
                receipt.PurchaseOrderId,
                receipt.ReceiptLines.Select(rl => new ReceiptCompletedIntegrationEvent.ReceiptLineInfo(
                    rl.Sku,
                    rl.QuantityReceived,
                    rl.LocationCode
                )).ToList()));
            
            _logger.LogInformation("Receipt {Id} completed", receiptId);
        }

        public async Task CancelReceiptAsync(Guid receiptId, string reason)
        {
            var receipt = await _receiptRepository.GetByIdAsync(receiptId);
            
            if (receipt == null)
                throw new Exception($"Receipt with ID {receiptId} not found.");
                
            receipt.Cancel(reason);
            
            await _receiptRepository.UpdateAsync(receipt);
            
            await _eventBus.PublishAsync(new ReceiptCancelledIntegrationEvent(
                receipt.Id,
                receipt.ReceiptNumber,
                receipt.PurchaseOrderId,
                reason));
            
            _logger.LogInformation("Receipt {Id} cancelled. Reason: {Reason}", receiptId, reason);
        }

        private PurchaseOrderDto MapToDto(PurchaseOrder entity)
        {
            return new PurchaseOrderDto
            {
                Id = entity.Id,
                Number = entity.Number,
                VendorId = entity.VendorId,
                VendorName = entity.VendorName,
                OrderDate = entity.OrderDate,
                ExpectedDeliveryDate = entity.ExpectedDeliveryDate,
                Status = entity.Status,
                Notes = entity.Notes,
                CreatedAt = entity.CreatedAt,
                ModifiedAt = entity.ModifiedAt,
                TotalValue = entity.TotalValue,
                OrderLines = entity.OrderLines.Select(line => new PurchaseOrderLineDto
                {
                    Id = line.Id,
                    Sku = line.Sku,
                    ItemName = line.ItemName,
                    Quantity = line.Quantity,
                    UnitPrice = line.UnitPrice,
                    LineTotal = line.LineTotal
                }).ToList()
            };
        }

        private ReceiptDto MapToDto(Receipt entity)
        {
            return new ReceiptDto
            {
                Id = entity.Id,
                PurchaseOrderId = entity.PurchaseOrderId,
                ReceiptNumber = entity.ReceiptNumber,
                ReceivedDate = entity.ReceivedDate,
                ReceivedBy = entity.ReceivedBy,
                Status = entity.Status,
                Notes = entity.Notes,
                CreatedAt = entity.CreatedAt,
                ModifiedAt = entity.ModifiedAt,
                ReceiptLines = entity.ReceiptLines.Select(line => new ReceiptLineDto
                {
                    Id = line.Id,
                    Sku = line.Sku,
                    ItemName = line.ItemName,
                    QuantityExpected = line.QuantityExpected,
                    QuantityReceived = line.QuantityReceived,
                    LocationCode = line.LocationCode,
                    Status = line.Status,
                    Notes = line.Notes
                }).ToList()
            };
        }
    }
}
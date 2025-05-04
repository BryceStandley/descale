using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WMS.Receiving.Application.Commands;
using WMS.Receiving.Application.DTOs;

namespace WMS.Receiving.Application.Services
{
    public interface IPurchaseOrderService
    {
        Task<IEnumerable<PurchaseOrderDto>> GetPurchaseOrdersAsync(int pageSize, int pageNumber, string status);
        Task<PurchaseOrderDto> GetPurchaseOrderByIdAsync(Guid id);
        Task<PurchaseOrderDto> CreatePurchaseOrderAsync(CreatePurchaseOrderCommand command);
        Task AddOrderLineAsync(AddPurchaseOrderLineCommand command);
        Task UpdateOrderLineAsync(UpdatePurchaseOrderLineCommand command);
        Task RemoveOrderLineAsync(Guid purchaseOrderId, string sku);
        Task ConfirmPurchaseOrderAsync(Guid purchaseOrderId);
        Task CancelPurchaseOrderAsync(Guid purchaseOrderId, string reason);
        Task<ReceiptDto> CreateReceiptAsync(CreateReceiptCommand command);
        Task<IEnumerable<ReceiptDto>> GetPurchaseOrderReceiptsAsync(Guid purchaseOrderId);
        Task<ReceiptDto> GetReceiptByIdAsync(Guid receiptId);
        Task CompleteReceiptAsync(Guid receiptId);
        Task CancelReceiptAsync(Guid receiptId, string reason);
    }
}
using WMS.SharedKernel.Entities;

namespace WMS.Shipping.Domain.Entities
{
    public class Shipment : Entity
    {
        public string Number { get; private set; }
        public string OrderNumber { get; private set; } // Reference to external order system
        public string Status { get; private set; } // Draft, Ready for Processing, In Processing, Shipped, Cancelled
        public string CarrierName { get; private set; }
        public string TrackingNumber { get; private set; }
        public decimal FreightCost { get; private set; }
        public string ShippingMethod { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? ModifiedAt { get; private set; }
        public DateTime? ShippedDate { get; private set; }
        public string Notes { get; private set; }
        
        // Shipping address
        public string ShipToName { get; private set; }
        public string ShipToAddress1 { get; private set; }
        public string ShipToAddress2 { get; private set; }
        public string ShipToCity { get; private set; }
        public string ShipToState { get; private set; }
        public string ShipToZipCode { get; private set; }
        public string ShipToCountry { get; private set; }
        
        private readonly List<ShipmentItem> _items = new();
        public IReadOnlyCollection<ShipmentItem> Items => _items.AsReadOnly();
        
        private readonly List<ShipmentPackage> _packages = new();
        public IReadOnlyCollection<ShipmentPackage> Packages => _packages.AsReadOnly();

        // For EF Core
        private Shipment() { }

        public Shipment(
            string number, 
            string orderNumber, 
            string shipToName, 
            string shipToAddress1, 
            string shipToCity, 
            string shipToState, 
            string shipToZipCode, 
            string shipToCountry, 
            string shippingMethod,
            string shipToAddress2 = null,
            string notes = null)
        {
            Number = number;
            OrderNumber = orderNumber;
            Status = "Draft";
            ShippingMethod = shippingMethod;
            CreatedAt = DateTime.UtcNow;
            Notes = notes;
            
            ShipToName = shipToName;
            ShipToAddress1 = shipToAddress1;
            ShipToAddress2 = shipToAddress2;
            ShipToCity = shipToCity;
            ShipToState = shipToState;
            ShipToZipCode = shipToZipCode;
            ShipToCountry = shipToCountry;
            
            AddDomainEvent(new ShipmentCreatedEvent(this));
        }

        public void AddItem(string sku, string itemName, int quantity)
        {
            if (Status != "Draft")
                throw new InvalidOperationException("Cannot modify a shipment that is not in Draft status");
                
            if (_items.Any(i => i.Sku == sku))
                throw new InvalidOperationException($"SKU {sku} already exists in this shipment");
                
            _items.Add(new ShipmentItem(this.Id, sku, itemName, quantity));
            ModifiedAt = DateTime.UtcNow;
            
            AddDomainEvent(new ShipmentItemAddedEvent(this, sku, quantity));
        }

        public void UpdateItem(string sku, int quantity)
        {
            if (Status != "Draft")
                throw new InvalidOperationException("Cannot modify a shipment that is not in Draft status");
                
            var item = _items.FirstOrDefault(i => i.Sku == sku);
            if (item == null)
                throw new InvalidOperationException($"SKU {sku} does not exist in this shipment");
                
            item.UpdateQuantity(quantity);
            ModifiedAt = DateTime.UtcNow;
            
            AddDomainEvent(new ShipmentItemUpdatedEvent(this, sku, quantity));
        }

        public void RemoveItem(string sku)
        {
            if (Status != "Draft")
                throw new InvalidOperationException("Cannot modify a shipment that is not in Draft status");
                
            var item = _items.FirstOrDefault(i => i.Sku == sku);
            if (item == null)
                throw new InvalidOperationException($"SKU {sku} does not exist in this shipment");
                
            _items.Remove(item);
            ModifiedAt = DateTime.UtcNow;
            
            AddDomainEvent(new ShipmentItemRemovedEvent(this, sku));
        }

        public void ReadyForProcessing()
        {
            if (Status != "Draft")
                throw new InvalidOperationException("Only Draft shipments can be set to Ready for Processing");
                
            if (!_items.Any())
                throw new InvalidOperationException("Cannot process a shipment with no items");
                
            Status = "Ready for Processing";
            ModifiedAt = DateTime.UtcNow;
            
            AddDomainEvent(new ShipmentReadyForProcessingEvent(this));
        }

        public void StartProcessing()
        {
            if (Status != "Ready for Processing")
                throw new InvalidOperationException("Only Ready for Processing shipments can be started");
                
            Status = "In Processing";
            ModifiedAt = DateTime.UtcNow;
            
            AddDomainEvent(new ShipmentProcessingStartedEvent(this));
        }

        public ShipmentPackage AddPackage(string packageNumber, string packageType, decimal weight, decimal length, decimal width, decimal height)
        {
            if (Status != "In Processing")
                throw new InvalidOperationException("Packages can only be added to shipments that are In Processing");
                
            var package = new ShipmentPackage(this.Id, packageNumber, packageType, weight, length, width, height);
            _packages.Add(package);
            ModifiedAt = DateTime.UtcNow;
            
            AddDomainEvent(new ShipmentPackageAddedEvent(this, packageNumber));
            
            return package;
        }

        public void RemovePackage(string packageNumber)
        {
            if (Status != "In Processing")
                throw new InvalidOperationException("Packages can only be removed from shipments that are In Processing");
                
            var package = _packages.FirstOrDefault(p => p.PackageNumber == packageNumber);
            if (package == null)
                throw new InvalidOperationException($"Package {packageNumber} does not exist in this shipment");
                
            _packages.Remove(package);
            ModifiedAt = DateTime.UtcNow;
            
            AddDomainEvent(new ShipmentPackageRemovedEvent(this, packageNumber));
        }

        public void Ship(string carrierName, string trackingNumber, decimal freightCost)
        {
            if (Status != "In Processing")
                throw new InvalidOperationException("Only In Processing shipments can be shipped");
                
            if (!_packages.Any())
                throw new InvalidOperationException("Cannot ship a shipment with no packages");
                
            Status = "Shipped";
            CarrierName = carrierName;
            TrackingNumber = trackingNumber;
            FreightCost = freightCost;
            ShippedDate = DateTime.UtcNow;
            ModifiedAt = DateTime.UtcNow;
            
            AddDomainEvent(new ShipmentShippedEvent(this));
        }

        public void Cancel(string reason)
        {
            if (Status == "Shipped" || Status == "Cancelled")
                throw new InvalidOperationException("Cannot cancel a shipment that is already shipped or cancelled");
                
            Status = "Cancelled";
            Notes = string.IsNullOrEmpty(Notes) ? reason : $"{Notes}\nCancelled: {reason}";
            ModifiedAt = DateTime.UtcNow;
            
            AddDomainEvent(new ShipmentCancelledEvent(this, reason));
        }

        public void UpdateShippingAddress(
            string shipToName,
            string shipToAddress1,
            string shipToCity,
            string shipToState,
            string shipToZipCode,
            string shipToCountry,
            string shipToAddress2 = null)
        {
            if (Status != "Draft")
                throw new InvalidOperationException("Cannot modify shipping address when shipment is not in Draft status");
                
            ShipToName = shipToName;
            ShipToAddress1 = shipToAddress1;
            ShipToAddress2 = shipToAddress2;
            ShipToCity = shipToCity;
            ShipToState = shipToState;
            ShipToZipCode = shipToZipCode;
            ShipToCountry = shipToCountry;
            
            ModifiedAt = DateTime.UtcNow;
            
            AddDomainEvent(new ShipmentAddressUpdatedEvent(this));
        }

        public void UpdateShippingMethod(string shippingMethod)
        {
            if (Status != "Draft")
                throw new InvalidOperationException("Cannot modify shipping method when shipment is not in Draft status");
                
            ShippingMethod = shippingMethod;
            ModifiedAt = DateTime.UtcNow;
            
            AddDomainEvent(new ShipmentMethodUpdatedEvent(this, shippingMethod));
        }
    }
}
using System;
using System.Collections.Generic;

namespace ECommerce.Models.Domain;

public partial class ShippingOrder
{
    public Guid OrderId { get; set; }

    public Guid CustomerId { get; set; }

    public string ShippingAddress { get; set; } = null!;

    public DateTime OrderDate { get; set; }

    public Guid? OrderStatusId { get; set; }

    public Guid? OrderTypeId { get; set; }

    public decimal OrderTotal { get; set; }

    public virtual CustomerCredential Customer { get; set; } = null!;

    public virtual ICollection<OrderLine> OrderLines { get; set; } = new List<OrderLine>();

    public virtual OrderStatus? OrderStatus { get; set; }

    public virtual OrderType? OrderType { get; set; }
}

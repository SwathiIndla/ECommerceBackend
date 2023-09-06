using System;
using System.Collections.Generic;

namespace ECommerce.Models.Domain;

public partial class OrderStatus
{
    public Guid OrderStatusId { get; set; }

    public string OrderStatus1 { get; set; } = null!;

    public virtual ICollection<ShippingOrder> ShippingOrders { get; set; } = new List<ShippingOrder>();
}

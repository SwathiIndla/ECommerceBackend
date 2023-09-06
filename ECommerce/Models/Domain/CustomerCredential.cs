using System;
using System.Collections.Generic;

namespace ECommerce.Models.Domain;

public partial class CustomerCredential
{
    public Guid CustomerId { get; set; }

    public string EmailId { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<CustomerAddress> CustomerAddresses { get; set; } = new List<CustomerAddress>();

    public virtual ICollection<ProductItemReview> ProductItemReviews { get; set; } = new List<ProductItemReview>();

    public virtual ICollection<ShippingOrder> ShippingOrders { get; set; } = new List<ShippingOrder>();
}

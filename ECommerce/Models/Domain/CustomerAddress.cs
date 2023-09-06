using System;
using System.Collections.Generic;

namespace ECommerce.Models.Domain;

public partial class CustomerAddress
{
    public Guid Id { get; set; }

    public Guid AddressId { get; set; }

    public Guid CustomerId { get; set; }

    public bool IsDefault { get; set; }

    public virtual Address Address { get; set; } = null!;

    public virtual CustomerCredential Customer { get; set; } = null!;
}

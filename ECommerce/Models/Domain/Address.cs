using System;
using System.Collections.Generic;

namespace ECommerce.Models.Domain;

public partial class Address
{
    public Guid AddressId { get; set; }

    public string CustomerName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string StreetAddress { get; set; } = null!;

    public string City { get; set; } = null!;

    public string StateProvince { get; set; } = null!;

    public string Country { get; set; } = null!;

    public string PostalCode { get; set; } = null!;

    public string AddressType { get; set; } = null!;

    public bool IsDefault { get; set; }

    public Guid CustomerId { get; set; }

    public virtual CustomerCredential Customer { get; set; } = null!;
}
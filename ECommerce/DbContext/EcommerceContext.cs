using System;
using System.Collections.Generic;
using ECommerce.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace ECommerce_WebAPI.DbContext;

public partial class EcommerceContext : Microsoft.EntityFrameworkCore.DbContext
{
    public EcommerceContext()
    {
    }

    public EcommerceContext(DbContextOptions<EcommerceContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartProductItem> CartProductItems { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<CategoryHierarchy> CategoryHierarchies { get; set; }

    public virtual DbSet<CustomerAddress> CustomerAddresses { get; set; }

    public virtual DbSet<CustomerCredential> CustomerCredentials { get; set; }

    public virtual DbSet<OrderLine> OrderLines { get; set; }

    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

    public virtual DbSet<OrderType> OrderTypes { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductItemConfiguration> ProductItemConfigurations { get; set; }

    public virtual DbSet<ProductItemDetail> ProductItemDetails { get; set; }

    public virtual DbSet<ProductItemReview> ProductItemReviews { get; set; }

    public virtual DbSet<PropertyName> PropertyNames { get; set; }

    public virtual DbSet<PropertyValue> PropertyValues { get; set; }

    public virtual DbSet<ShippingOrder> ShippingOrders { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.AddressId).HasName("PK__Address__CAA247C824C09024");

            entity.ToTable("Address");

            entity.Property(e => e.AddressId)
                .ValueGeneratedNever()
                .HasColumnName("address_id");
            entity.Property(e => e.AddressType)
                .HasMaxLength(50)
                .HasColumnName("address_type");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .HasColumnName("city");
            entity.Property(e => e.Country)
                .HasMaxLength(100)
                .HasColumnName("country");
            entity.Property(e => e.CustomerName)
                .HasMaxLength(255)
                .HasColumnName("customer_name");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(50)
                .HasColumnName("phone_number");
            entity.Property(e => e.PostalCode)
                .HasMaxLength(10)
                .HasColumnName("postal_code");
            entity.Property(e => e.StateProvince)
                .HasMaxLength(50)
                .HasColumnName("state_province");
            entity.Property(e => e.StreetAddress).HasColumnName("street_address");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PK_cart");

            entity.ToTable("Cart");

            entity.Property(e => e.CartId)
                .ValueGeneratedNever()
                .HasColumnName("cart_id");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");

            entity.HasOne(d => d.Customer).WithMany(p => p.Carts)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_cart_CustomerCredential");
        });

        modelBuilder.Entity<CartProductItem>(entity =>
        {
            entity.Property(e => e.CartProductItemId)
                .ValueGeneratedNever()
                .HasColumnName("cart_product_item_id");
            entity.Property(e => e.CartId).HasColumnName("cart_id");
            entity.Property(e => e.ProductItemId).HasColumnName("product_item_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Cart).WithMany(p => p.CartProductItems)
                .HasForeignKey(d => d.CartId)
                .HasConstraintName("FK_CartProductItems_cart");

            entity.HasOne(d => d.ProductItem).WithMany(p => p.CartProductItems)
                .HasForeignKey(d => d.ProductItemId)
                .HasConstraintName("FK_CartProductItems_product_item_details");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__D54EE9B4C33017FF");

            entity.Property(e => e.CategoryId)
                .ValueGeneratedNever()
                .HasColumnName("category_id");
            entity.Property(e => e.CategoryImgUrl)
                .HasMaxLength(255)
                .HasColumnName("category_img_url");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(255)
                .HasColumnName("category_name");
        });

        modelBuilder.Entity<CategoryHierarchy>(entity =>
        {
            entity.ToTable("CategoryHierarchy");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AncestorCategoryId).HasColumnName("ancestor_category_id");
            entity.Property(e => e.DescendantCategoryId).HasColumnName("descendant_category_id");

            entity.HasOne(d => d.AncestorCategory).WithMany(p => p.CategoryHierarchyAncestorCategories)
                .HasForeignKey(d => d.AncestorCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CategoryH__ances__5FB337D6");

            entity.HasOne(d => d.DescendantCategory).WithMany(p => p.CategoryHierarchyDescendantCategories)
                .HasForeignKey(d => d.DescendantCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CategoryH__desce__60A75C0F");
        });

        modelBuilder.Entity<CustomerAddress>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Customer__3213E83F01EFF5D1");

            entity.ToTable("CustomerAddress");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AddressId).HasColumnName("address_id");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.IsDefault).HasColumnName("is_default");

            entity.HasOne(d => d.Address).WithMany(p => p.CustomerAddresses)
                .HasForeignKey(d => d.AddressId)
                .HasConstraintName("FK_CustomerAddress_Address");

            entity.HasOne(d => d.Customer).WithMany(p => p.CustomerAddresses)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_CustomerAddress_CustomerCredential");
        });

        modelBuilder.Entity<CustomerCredential>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__CD65CB85C5538D97");

            entity.ToTable("CustomerCredential");

            entity.HasIndex(e => e.EmailId, "UQ__Customer__3FEF876706C22233").IsUnique();

            entity.Property(e => e.CustomerId)
                .ValueGeneratedNever()
                .HasColumnName("customer_id");
            entity.Property(e => e.EmailId)
                .HasMaxLength(255)
                .HasColumnName("email_id");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
        });

        modelBuilder.Entity<OrderLine>(entity =>
        {
            entity.ToTable("OrderLine");

            entity.Property(e => e.OrderLineId)
                .ValueGeneratedNever()
                .HasColumnName("order_line_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(19, 4)")
                .HasColumnName("price");
            entity.Property(e => e.ProductItemId).HasColumnName("product_item_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderLines)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_OrderLine_ShippingOrder");

            entity.HasOne(d => d.ProductItem).WithMany(p => p.OrderLines)
                .HasForeignKey(d => d.ProductItemId)
                .HasConstraintName("FK_OrderLine_ProductItemDetails");
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.ToTable("OrderStatus");

            entity.Property(e => e.OrderStatusId)
                .ValueGeneratedNever()
                .HasColumnName("order_status_id");
            entity.Property(e => e.OrderStatus1)
                .HasMaxLength(50)
                .HasColumnName("order_status");
        });

        modelBuilder.Entity<OrderType>(entity =>
        {
            entity.ToTable("OrderType");

            entity.Property(e => e.OrderTypeId)
                .ValueGeneratedNever()
                .HasColumnName("order_type_id");
            entity.Property(e => e.OrderType1)
                .HasMaxLength(50)
                .HasColumnName("order_type");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasIndex(e => e.ProductName, "IX_Products").IsUnique();

            entity.Property(e => e.ProductId)
                .ValueGeneratedNever()
                .HasColumnName("product_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.ProductDescription).HasColumnName("product_description");
            entity.Property(e => e.ProductImage)
                .HasMaxLength(255)
                .HasColumnName("product_image");
            entity.Property(e => e.ProductName)
                .HasMaxLength(255)
                .HasColumnName("product_name");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_Products_Categories");
        });

        modelBuilder.Entity<ProductItemConfiguration>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_product_item_variation_junction");

            entity.ToTable("ProductItemConfiguration");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.ProductItemId).HasColumnName("product_item_id");
            entity.Property(e => e.PropertyValueId).HasColumnName("property_value_id");

            entity.HasOne(d => d.ProductItem).WithMany(p => p.ProductItemConfigurations)
                .HasForeignKey(d => d.ProductItemId)
                .HasConstraintName("FK_product_item_variation_junction_product_item_details");

            entity.HasOne(d => d.PropertyValue).WithMany(p => p.ProductItemConfigurations)
                .HasForeignKey(d => d.PropertyValueId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductItemConfiguration_PropertyValues");
        });

        modelBuilder.Entity<ProductItemDetail>(entity =>
        {
            entity.HasKey(e => e.ProductItemId).HasName("PK_product_item_details");

            entity.Property(e => e.ProductItemId)
                .ValueGeneratedNever()
                .HasColumnName("product_item_id");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(19, 4)")
                .HasColumnName("price");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.QtyInStock).HasColumnName("qty_in_stock");
            entity.Property(e => e.Sku)
                .HasMaxLength(255)
                .HasColumnName("sku");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductItemDetails)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_product_item_details_Products");
        });

        modelBuilder.Entity<ProductItemReview>(entity =>
        {
            entity.HasKey(e => e.ProductReviewId);

            entity.ToTable("ProductItemReview");

            entity.Property(e => e.ProductReviewId)
                .ValueGeneratedNever()
                .HasColumnName("product_review_id");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Rating)
                .HasColumnType("decimal(2, 1)")
                .HasColumnName("rating");
            entity.Property(e => e.Review).HasColumnName("review");

            entity.HasOne(d => d.Customer).WithMany(p => p.ProductItemReviews)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_ProductItemReview_CustomerCredential");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductItemReviews)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_ProductItemReview_Products");
        });

        modelBuilder.Entity<PropertyName>(entity =>
        {
            entity.HasKey(e => e.PropertyId).HasName("PK_PropertiesName");

            entity.ToTable("PropertyName");

            entity.HasIndex(e => e.PropertyName1, "IX_PropertiesName").IsUnique();

            entity.Property(e => e.PropertyId)
                .ValueGeneratedNever()
                .HasColumnName("property_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.PropertyName1)
                .HasMaxLength(225)
                .HasColumnName("property_name");

            entity.HasOne(d => d.Category).WithMany(p => p.PropertyNames)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_PropertiesName_Categories");
        });

        modelBuilder.Entity<PropertyValue>(entity =>
        {
            entity.HasKey(e => e.PropertyValueId).HasName("PK_PropertyValue");

            entity.HasIndex(e => e.PropertyValue1, "IX_PropertyValue").IsUnique();

            entity.Property(e => e.PropertyValueId)
                .ValueGeneratedNever()
                .HasColumnName("property_value_id");
            entity.Property(e => e.PropertyNameId).HasColumnName("property_name_id");
            entity.Property(e => e.PropertyValue1)
                .HasMaxLength(255)
                .HasColumnName("property_value");

            entity.HasOne(d => d.PropertyName).WithMany(p => p.PropertyValues)
                .HasForeignKey(d => d.PropertyNameId)
                .HasConstraintName("FK_PropertyValue_PropertiesName");
        });

        modelBuilder.Entity<ShippingOrder>(entity =>
        {
            entity.HasKey(e => e.OrderId);

            entity.ToTable("ShippingOrder");

            entity.Property(e => e.OrderId)
                .ValueGeneratedNever()
                .HasColumnName("order_id");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.OrderDate)
                .HasColumnType("datetime")
                .HasColumnName("order_date");
            entity.Property(e => e.OrderStatusId).HasColumnName("order_status_id");
            entity.Property(e => e.OrderTotal)
                .HasColumnType("decimal(19, 4)")
                .HasColumnName("order_total");
            entity.Property(e => e.OrderTypeId).HasColumnName("order_type_id");
            entity.Property(e => e.ShippingAddress).HasColumnName("shipping_address");

            entity.HasOne(d => d.Customer).WithMany(p => p.ShippingOrders)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_ShippingOrder_CustomerCredential");

            entity.HasOne(d => d.OrderStatus).WithMany(p => p.ShippingOrders)
                .HasForeignKey(d => d.OrderStatusId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_ShippingOrder_OrderStatus");

            entity.HasOne(d => d.OrderType).WithMany(p => p.ShippingOrders)
                .HasForeignKey(d => d.OrderTypeId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_ShippingOrder_OrderType");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

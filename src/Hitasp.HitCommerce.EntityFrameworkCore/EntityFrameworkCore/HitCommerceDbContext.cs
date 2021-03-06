using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.IdentityServer.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using Hitasp.HitCommerce.ActivityLog.EntityFrameworkCore;
using Hitasp.HitCommerce.Catalog.EntityFrameworkCore;
using Hitasp.HitCommerce.Cms.EntityFrameworkCore;
using Hitasp.HitCommerce.Contacts.EntityFrameworkCore;
using Hitasp.HitCommerce.Core.EntityFrameworkCore;
using Hitasp.HitCommerce.Inventory.EntityFrameworkCore;
using Hitasp.HitCommerce.Orders.EntityFrameworkCore;
using Hitasp.HitCommerce.Payments.EntityFrameworkCore;
using Hitasp.HitCommerce.Pricing.EntityFrameworkCore;
using Hitasp.HitCommerce.ProductComparison.EntityFrameworkCore;
using Hitasp.HitCommerce.ProductRecentlyViewed.EntityFrameworkCore;
using Hitasp.HitCommerce.Search.EntityFrameworkCore;
using Hitasp.HitCommerce.Shipments.EntityFrameworkCore;
using Hitasp.HitCommerce.Shipping.EntityFrameworkCore;
using Hitasp.HitCommerce.ShoppingCart.EntityFrameworkCore;
using Hitasp.HitCommerce.Storage.EntityFrameworkCore;
using Hitasp.HitCommerce.Tax.EntityFrameworkCore;
using Hitasp.HitCommerce.Vendors.EntityFrameworkCore;

namespace Hitasp.HitCommerce.EntityFrameworkCore
{
    [ReplaceDbContext(typeof(IIdentityDbContext))]
    [ReplaceDbContext(typeof(ITenantManagementDbContext))]
    [ConnectionStringName("Default")]
    public class HitCommerceDbContext :
        AbpDbContext<HitCommerceDbContext>,
        IIdentityDbContext,
        ITenantManagementDbContext
    {
        /* Add DbSet properties for your Aggregate Roots / Entities here. */

        #region Entities from the modules

        /* Notice: We only implemented IIdentityDbContext and ITenantManagementDbContext
         * and replaced them for this DbContext. This allows you to perform JOIN
         * queries for the entities of these modules over the repositories easily. You
         * typically don't need that for other modules. But, if you need, you can
         * implement the DbContext interface of the needed module and use ReplaceDbContext
         * attribute just like IIdentityDbContext and ITenantManagementDbContext.
         *
         * More info: Replacing a DbContext of a module ensures that the related module
         * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
         */

        //Identity
        public DbSet<IdentityUser> Users { get; set; }
        public DbSet<IdentityRole> Roles { get; set; }
        public DbSet<IdentityClaimType> ClaimTypes { get; set; }
        public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
        public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
        public DbSet<IdentityLinkUser> LinkUsers { get; set; }

        // Tenant Management
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

        #endregion

        public HitCommerceDbContext(DbContextOptions<HitCommerceDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            /* Include modules to your migration db context */

            builder.ConfigurePermissionManagement();
            builder.ConfigureSettingManagement();
            builder.ConfigureBackgroundJobs();
            builder.ConfigureAuditLogging();
            builder.ConfigureIdentity();
            builder.ConfigureIdentityServer();
            builder.ConfigureFeatureManagement();
            builder.ConfigureTenantManagement();

            /* Configure your own tables/entities inside here */

            //builder.Entity<YourEntity>(b =>
            //{
            //    b.ToTable(HitCommerceConsts.DbTablePrefix + "YourEntities", HitCommerceConsts.DbSchema);
            //    b.ConfigureByConvention(); //auto configure for the base class props
            //    //...
            //});
            builder.ConfigureActivityLog();
            builder.ConfigureCatalog();
            builder.ConfigureCms();
            builder.ConfigureContacts();
            builder.ConfigureCore();
            builder.ConfigureInventory();
            builder.ConfigureOrders();
            builder.ConfigurePayments();
            builder.ConfigurePricing();
            builder.ConfigureProductComparison();
            builder.ConfigureProductRecentlyViewed();
            builder.ConfigureSearch();
            builder.ConfigureShipments();
            builder.ConfigureShipping();
            builder.ConfigureShoppingCart();
            builder.ConfigureStorage();
            builder.ConfigureTax();
            builder.ConfigureVendors();
        }
    }
}
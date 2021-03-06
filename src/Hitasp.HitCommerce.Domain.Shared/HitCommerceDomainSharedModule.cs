using Hitasp.HitCommerce.Localization;
using Volo.Abp.AuditLogging;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.IdentityServer;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;
using Hitasp.HitCommerce.ActivityLog;
using Hitasp.HitCommerce.Catalog;
using Hitasp.HitCommerce.Cms;
using Hitasp.HitCommerce.Contacts;
using Hitasp.HitCommerce.Core;
using Hitasp.HitCommerce.Inventory;
using Hitasp.HitCommerce.Orders;
using Hitasp.HitCommerce.Payments;
using Hitasp.HitCommerce.Pricing;
using Hitasp.HitCommerce.ProductComparison;
using Hitasp.HitCommerce.ProductRecentlyViewed;
using Hitasp.HitCommerce.Search;
using Hitasp.HitCommerce.Shipments;
using Hitasp.HitCommerce.Shipping;
using Hitasp.HitCommerce.ShoppingCart;
using Hitasp.HitCommerce.Storage;
using Hitasp.HitCommerce.Tax;
using Hitasp.HitCommerce.Vendors;

namespace Hitasp.HitCommerce
{
    [DependsOn(
        typeof(AbpAuditLoggingDomainSharedModule),
        typeof(AbpBackgroundJobsDomainSharedModule),
        typeof(AbpFeatureManagementDomainSharedModule),
        typeof(AbpIdentityDomainSharedModule),
        typeof(AbpIdentityServerDomainSharedModule),
        typeof(AbpPermissionManagementDomainSharedModule),
        typeof(AbpSettingManagementDomainSharedModule),
        typeof(AbpTenantManagementDomainSharedModule)
    )]
    [DependsOn(typeof(ActivityLogDomainSharedModule))]
    [DependsOn(typeof(CatalogDomainSharedModule))]
    [DependsOn(typeof(CmsDomainSharedModule))]
    [DependsOn(typeof(ContactsDomainSharedModule))]
    [DependsOn(typeof(CoreDomainSharedModule))]
    [DependsOn(typeof(InventoryDomainSharedModule))]
    [DependsOn(typeof(OrdersDomainSharedModule))]
    [DependsOn(typeof(PaymentsDomainSharedModule))]
    [DependsOn(typeof(PricingDomainSharedModule))]
    [DependsOn(typeof(ProductComparisonDomainSharedModule))]
    [DependsOn(typeof(ProductRecentlyViewedDomainSharedModule))]
    [DependsOn(typeof(SearchDomainSharedModule))]
    [DependsOn(typeof(ShipmentsDomainSharedModule))]
    [DependsOn(typeof(ShippingDomainSharedModule))]
    [DependsOn(typeof(ShoppingCartDomainSharedModule))]
    [DependsOn(typeof(StorageDomainSharedModule))]
    [DependsOn(typeof(TaxDomainSharedModule))]
    [DependsOn(typeof(VendorsDomainSharedModule))]
    public class HitCommerceDomainSharedModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            HitCommerceGlobalFeatureConfigurator.Configure();
            HitCommerceModuleExtensionConfigurator.Configure();
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<HitCommerceDomainSharedModule>();
            });

            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Add<HitCommerceResource>("en")
                    .AddBaseTypes(typeof(AbpValidationResource))
                    .AddVirtualJson("/Localization/HitCommerce");

                options.DefaultResourceType = typeof(HitCommerceResource);
            });

            Configure<AbpExceptionLocalizationOptions>(options =>
            {
                options.MapCodeNamespace("HitCommerce", typeof(HitCommerceResource));
            });
        }
    }
}
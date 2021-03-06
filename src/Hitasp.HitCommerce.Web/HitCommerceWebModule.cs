using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Hitasp.HitCommerce.Localization;
using Hitasp.HitCommerce.MultiTenancy;
using Hitasp.HitCommerce.Web.Menus;
using StackExchange.Redis;
using Microsoft.OpenApi.Models;
using Volo.Abp;
using Volo.Abp.AspNetCore.Authentication.OpenIdConnect;
using Volo.Abp.AspNetCore.Mvc.Client;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Toolbars;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.AutoMapper;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.Http.Client.IdentityModel.Web;
using Volo.Abp.Identity.Web;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement.Web;
using Volo.Abp.SettingManagement.Web;
using Volo.Abp.Swashbuckle;
using Volo.Abp.TenantManagement.Web;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.UI;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;
using Hitasp.HitCommerce.ActivityLog.Web;
using Hitasp.HitCommerce.Catalog.Web;
using Hitasp.HitCommerce.Cms.Web;
using Hitasp.HitCommerce.Contacts.Web;
using Hitasp.HitCommerce.Core.Web;
using Hitasp.HitCommerce.Inventory.Web;
using Hitasp.HitCommerce.Orders.Web;
using Hitasp.HitCommerce.Payments.Web;
using Hitasp.HitCommerce.Pricing.Web;
using Hitasp.HitCommerce.ProductComparison.Web;
using Hitasp.HitCommerce.ProductRecentlyViewed.Web;
using Hitasp.HitCommerce.Search.Web;
using Hitasp.HitCommerce.Shipments.Web;
using Hitasp.HitCommerce.Shipping.Web;
using Hitasp.HitCommerce.ShoppingCart.Web;
using Hitasp.HitCommerce.Storage.Web;
using Hitasp.HitCommerce.Tax.Web;
using Hitasp.HitCommerce.Vendors.Web;

namespace Hitasp.HitCommerce.Web
{
    [DependsOn(
        typeof(HitCommerceHttpApiModule),
        typeof(HitCommerceHttpApiClientModule),
        typeof(AbpAspNetCoreAuthenticationOpenIdConnectModule),
        typeof(AbpAspNetCoreMvcClientModule),
        typeof(AbpAspNetCoreMvcUiBasicThemeModule),
        typeof(AbpAutofacModule),
        typeof(AbpCachingStackExchangeRedisModule),
        typeof(AbpSettingManagementWebModule),
        typeof(AbpHttpClientIdentityModelWebModule),
        typeof(AbpIdentityWebModule),
        typeof(AbpTenantManagementWebModule),
        typeof(AbpAspNetCoreSerilogModule),
        typeof(AbpSwashbuckleModule)
    )]
    [DependsOn(typeof(ActivityLogWebModule))]
    [DependsOn(typeof(CatalogWebModule))]
    [DependsOn(typeof(CmsWebModule))]
    [DependsOn(typeof(ContactsWebModule))]
    [DependsOn(typeof(CoreWebModule))]
    [DependsOn(typeof(InventoryWebModule))]
    [DependsOn(typeof(OrdersWebModule))]
    [DependsOn(typeof(PaymentsWebModule))]
    [DependsOn(typeof(PricingWebModule))]
    [DependsOn(typeof(ProductComparisonWebModule))]
    [DependsOn(typeof(ProductRecentlyViewedWebModule))]
    [DependsOn(typeof(SearchWebModule))]
    [DependsOn(typeof(ShipmentsWebModule))]
    [DependsOn(typeof(ShippingWebModule))]
    [DependsOn(typeof(ShoppingCartWebModule))]
    [DependsOn(typeof(StorageWebModule))]
    [DependsOn(typeof(TaxWebModule))]
    [DependsOn(typeof(VendorsWebModule))]
    public class HitCommerceWebModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
            {
                options.AddAssemblyResource(
                    typeof(HitCommerceResource),
                    typeof(HitCommerceDomainSharedModule).Assembly,
                    typeof(HitCommerceApplicationContractsModule).Assembly,
                    typeof(HitCommerceWebModule).Assembly
                );
            });
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var hostingEnvironment = context.Services.GetHostingEnvironment();
            var configuration = context.Services.GetConfiguration();

            ConfigureBundles();
            ConfigureCache();
            ConfigureRedis(context, configuration, hostingEnvironment);
            ConfigureUrls(configuration);
            ConfigureAuthentication(context, configuration);
            ConfigureAutoMapper();
            ConfigureVirtualFileSystem(hostingEnvironment);
            ConfigureNavigationServices(configuration);
            ConfigureMultiTenancy();
            ConfigureSwaggerServices(context.Services);
        }

        private void ConfigureBundles()
        {
            Configure<AbpBundlingOptions>(options =>
            {
                options.StyleBundles.Configure(
                    BasicThemeBundles.Styles.Global,
                    bundle => { bundle.AddFiles("/global-styles.css"); }
                );
            });
        }

        private void ConfigureCache()
        {
            Configure<AbpDistributedCacheOptions>(options => { options.KeyPrefix = "HitCommerce:"; });
        }

        private void ConfigureUrls(IConfiguration configuration)
        {
            Configure<AppUrlOptions>(options =>
            {
                options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
            });
        }

        private void ConfigureMultiTenancy()
        {
            Configure<AbpMultiTenancyOptions>(options => { options.IsEnabled = MultiTenancyConsts.IsEnabled; });
        }

        private void ConfigureAuthentication(ServiceConfigurationContext context, IConfiguration configuration)
        {
            context.Services.AddAuthentication(options =>
                {
                    options.DefaultScheme = "Cookies";
                    options.DefaultChallengeScheme = "oidc";
                })
                .AddCookie("Cookies", options => { options.ExpireTimeSpan = TimeSpan.FromDays(365); })
                .AddAbpOpenIdConnect("oidc", options =>
                {
                    options.Authority = configuration["AuthServer:Authority"];
                    options.RequireHttpsMetadata = Convert.ToBoolean(configuration["AuthServer:RequireHttpsMetadata"]);
                    options.ResponseType = OpenIdConnectResponseType.CodeIdToken;

                    options.ClientId = configuration["AuthServer:ClientId"];
                    options.ClientSecret = configuration["AuthServer:ClientSecret"];

                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;

                    options.Scope.Add("role");
                    options.Scope.Add("email");
                    options.Scope.Add("phone");
                    options.Scope.Add("HitCommerce");
                    options.Scope.Add("HitCommercePublic");
                });
        }

        private void ConfigureAutoMapper()
        {
            Configure<AbpAutoMapperOptions>(options => { options.AddMaps<HitCommerceWebModule>(); });
        }

        private void ConfigureVirtualFileSystem(IWebHostEnvironment hostingEnvironment)
        {
            if (hostingEnvironment.IsDevelopment())
            {
                Configure<AbpVirtualFileSystemOptions>(options =>
                {
                    options.FileSets.ReplaceEmbeddedByPhysical<HitCommerceDomainSharedModule>(
                        Path.Combine(hostingEnvironment.ContentRootPath,
                            $"..{Path.DirectorySeparatorChar}Hitasp.HitCommerce.Domain"));
                    options.FileSets.ReplaceEmbeddedByPhysical<HitCommerceApplicationContractsModule>(
                        Path.Combine(hostingEnvironment.ContentRootPath,
                            $"..{Path.DirectorySeparatorChar}Hitasp.HitCommerce.Application.Contracts"));
                    options.FileSets.ReplaceEmbeddedByPhysical<HitCommerceWebModule>(hostingEnvironment
                        .ContentRootPath);
                });
            }
        }

        private void ConfigureNavigationServices(IConfiguration configuration)
        {
            Configure<AbpNavigationOptions>(options =>
            {
                options.MenuContributors.Add(new HitCommerceMenuContributor(configuration));
            });

            Configure<AbpToolbarOptions>(options => { options.Contributors.Add(new HitCommerceToolbarContributor()); });
        }

        private void ConfigureSwaggerServices(IServiceCollection services)
        {
            services.AddAbpSwaggerGen(
                options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo {Title = "HitCommerce API", Version = "v1"});
                    options.DocInclusionPredicate((docName, description) => true);
                    options.CustomSchemaIds(type => type.FullName);
                }
            );
        }

        private void ConfigureRedis(
            ServiceConfigurationContext context,
            IConfiguration configuration,
            IWebHostEnvironment hostingEnvironment)
        {
            if (!hostingEnvironment.IsDevelopment())
            {
                var redis = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]);
                context.Services
                    .AddDataProtection()
                    .PersistKeysToStackExchangeRedis(redis, "HitCommerce-Protection-Keys");
            }
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAbpRequestLocalization();

            if (!env.IsDevelopment())
            {
                app.UseErrorPage();
            }

            app.UseCorrelationId();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();

            if (MultiTenancyConsts.IsEnabled)
            {
                app.UseMultiTenancy();
            }

            app.UseAuthorization();
            app.UseSwagger();
            app.UseAbpSwaggerUI(options => { options.SwaggerEndpoint("/swagger/v1/swagger.json", "HitCommerce API"); });
            app.UseAbpSerilogEnrichers();
            app.UseConfiguredEndpoints();
        }
    }
}
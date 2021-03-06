using Hitasp.HitCommerce.Core.Permissions;
using Hitasp.HitCommerce.Core.Localization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Volo.Abp.UI.Navigation;

namespace Hitasp.HitCommerce.Core.Web.Menus
{
    public class CoreMenuContributor : IMenuContributor
    {
        public async Task ConfigureMenuAsync(MenuConfigurationContext context)
        {
            if (context.Menu.Name == StandardMenus.Main)
            {
                await ConfigureMainMenuAsync(context);
            }

            var moduleMenu = AddModuleMenuItem(context);
            await AddMenuItemCountries(context, moduleMenu);
        }

        private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
        {
            //Add main menu items.
            context.Menu.AddItem(new ApplicationMenuItem(CoreMenus.Prefix, displayName: "Core", "~/Core", icon: "fa fa-globe"));

            return Task.CompletedTask;
        }

        private static ApplicationMenuItem AddModuleMenuItem(MenuConfigurationContext context)
        {
            var moduleMenu = new ApplicationMenuItem(
                CoreMenus.Prefix,
                context.GetLocalizer<CoreResource>()["Menu:Core"],
                icon: "fa fa-folder"
            );

            context.Menu.Items.AddIfNotContains(moduleMenu);
            return moduleMenu;
        }
        private static async Task AddMenuItemCountries(MenuConfigurationContext context, ApplicationMenuItem parentMenu)
        {
            parentMenu.AddItem(
                new ApplicationMenuItem(
                    Menus.CoreMenus.Countries,
                    context.GetLocalizer<CoreResource>()["Menu:Countries"],
                    "/Core/Countries",
                    icon: "fa fa-file-alt",
                    requiredPermissionName: CorePermissions.Countries.Default
                )
            );
        }
    }
}
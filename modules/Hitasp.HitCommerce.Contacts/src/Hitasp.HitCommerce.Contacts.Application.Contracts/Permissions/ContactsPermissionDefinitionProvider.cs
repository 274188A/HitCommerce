﻿using Hitasp.HitCommerce.Contacts.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Hitasp.HitCommerce.Contacts.Permissions
{
    public class ContactsPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var myGroup = context.AddGroup(ContactsPermissions.GroupName, L("Permission:Contacts"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<ContactsResource>(name);
        }
    }
}
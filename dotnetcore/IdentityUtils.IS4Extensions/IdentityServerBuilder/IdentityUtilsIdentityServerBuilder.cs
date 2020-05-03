﻿using IdentityUtils.Core.Contracts.Roles;
using IdentityUtils.Core.Contracts.Users;
using IdentityUtils.IS4Extensions.ProfileServices;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityUtils.IS4Extensions.IdentityServerBuilder
{
    public class IdentityUtilsIdentityServerBuilder
    {
        private readonly IIdentityServerBuilder identityServerBuilder;

        public IdentityUtilsIdentityServerBuilder(IIdentityServerBuilder identityServerBuilder)
        {
            this.identityServerBuilder = identityServerBuilder;
        }

        public IdentityUtilsIdentityServerBuilder AddDefaultClientConfiguration()
        {
            identityServerBuilder
                .AddInMemoryIdentityResources(IdentityServerDefaultConfig.Ids)
                .AddInMemoryApiResources(IdentityServerDefaultConfig.Apis)
                .AddInMemoryClients(IdentityServerDefaultConfig.Clients);

            return this;
        }

        public IdentityUtilsIdentityServerBuilder AddIdentityAndProfileService<TUser, TUserDto, TRole>()
            where TUser : IdentityManagerUser
            where TUserDto : class, IIdentityManagerUserDto
            where TRole : IdentityManagerRole
        {
            identityServerBuilder
                .AddAspNetIdentity<TUser>()
                .AddProfileService<IdentityUtilsProfileService<TUser, TUserDto, TRole>>();

            return this;
        }
    }
}
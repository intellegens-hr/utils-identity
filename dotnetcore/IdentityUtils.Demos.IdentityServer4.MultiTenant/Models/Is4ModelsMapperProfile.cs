﻿using AutoMapper;
using IdentityUtils.Core.Contracts.Roles;
using IdentityUtils.Core.Contracts.Tenants;
using IdentityUtils.Core.Contracts.Users;
using System.Collections.Generic;

namespace IdentityUtils.Demos.IdentityServer4.MultiTenant.Models
{
    public class Is4ModelsMapperProfile : Profile
    {
        public Is4ModelsMapperProfile()
        {
            CreateMap<IdentityManagerUser, UserDto>();
            CreateMap<UserDto, IdentityManagerUser>();

            CreateMap<IdentityManagerRole, RoleDto>();
            CreateMap<RoleDto, IdentityManagerRole>();

            CreateMap<IdentityManagerTenant, TenantDto>();
            CreateMap<TenantDto, IdentityManagerTenant>();
        }
    }
}
using AutoMapper;
using IdentityUtils.Core.Contracts.Roles;
using IdentityUtils.Core.Contracts.Tenants;
using IdentityUtils.Core.Contracts.Users;
using System.Collections.Generic;

namespace IdentityUtils.Core.Services.Tests.Setup.DtoModels
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<IdentityManagerUser, UserDto>();
            CreateMap<UserDto, IdentityManagerUser>();
            CreateMap<List<IdentityManagerUser>, List<UserDto>>();

            CreateMap<IdentityManagerRole, RoleDto>();
            CreateMap<RoleDto, IdentityManagerRole>();
            CreateMap<List<IdentityManagerRole>, List<RoleDto>>();

            CreateMap<IdentityManagerTenant, TenantDto>();
            CreateMap<TenantDto, IdentityManagerTenant>();
            CreateMap<List<IdentityManagerTenant>, List<TenantDto>>();
        }
    }
}
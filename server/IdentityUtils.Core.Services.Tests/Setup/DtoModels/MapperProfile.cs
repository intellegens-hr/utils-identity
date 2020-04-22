using AutoMapper;
using IdentityUtils.Core.Contracts.Roles;
using IdentityUtils.Core.Contracts.Tenants;
using IdentityUtils.Core.Contracts.Users;
using IdentityUtils.Core.Services.Tests.Setup.DbModels;
using System.Collections.Generic;

namespace IdentityUtils.Core.Services.Tests.Setup.DtoModels
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<UserDb, UserDto>();
            CreateMap<UserDto, UserDb>();
            //CreateMap<List<IdentityManagerUser>, List<UserDto>>();

            CreateMap<RoleDb, RoleDto>();
            CreateMap<RoleDto, RoleDb>();
            //CreateMap<List<IdentityManagerRole>, List<RoleDto>>();

            CreateMap<TenantDb, TenantDto>();
            CreateMap<TenantDto, TenantDb>();
            //CreateMap<List<IdentityManagerTenant>, List<TenantDto>>();
        }
    }
}
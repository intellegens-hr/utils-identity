using AutoMapper;
using IdentityUtils.Core.Contracts.Roles;
using IdentityUtils.Core.Contracts.Users;
using System.Collections.Generic;

namespace IdentityUtils.Demos.IdentityServer4.SingleTenant.Models
{
    public class Is4ModelsMapperProfile : Profile
    {
        public Is4ModelsMapperProfile()
        {
            CreateMap<IdentityManagerUser, UserDto>();
            CreateMap<UserDto, IdentityManagerUser>();
            CreateMap<List<IdentityManagerUser>, List<UserDto>>();

            CreateMap<IdentityManagerRole, RoleDto>();
            CreateMap<RoleDto, IdentityManagerRole>();
            CreateMap<List<IdentityManagerRole>, List<RoleDto>>();
        }
    }
}
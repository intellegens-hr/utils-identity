using AutoMapper;
using IdentityUtils.Core.Services.Tests.Setup.DbModels;

namespace IdentityUtils.Core.Services.Tests.Setup.DtoModels
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<UserDb, UserDto>();
            CreateMap<UserDto, UserDb>();

            CreateMap<RoleDb, RoleDto>();
            CreateMap<RoleDto, RoleDb>();

            CreateMap<TenantDb, TenantDto>();
            CreateMap<TenantDto, TenantDb>();
        }
    }
}
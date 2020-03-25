# IdentityUtils.Core.Contracts

This project contains basic Identity classes which inherit .NET Core Identity and add multitenancy features. Since all entities are going to use Guid as ID, this also helps to reduce number of typed parameteres.

Project contains customized identity context, database models and DTO models. Main difference is that database model should use sequential Guids instead of regular ones to avoid performance issues with clustered primary keys (or other indexes).

## Project structure:
### Context 
Most important class is `IdentityManagerDbContext` which inherits `IdentityDbContext` with typed parameters and adds additional required DbSets

### Roles 
Specifies role db model (`IdentityManagerRole`) and interface for DTO model implementations (`IIdentityManagerRoleDto`)

### Tenants 
Specifies tenant db model (`IdentityManagerTenant`) and interface for DTO model implementations (`IIdentityManagerTenantDto`)

### Users 
Specifies user db model (`IdentityManagerUser`). User db model inherits IdentityUser and extends it with `AdditionalDataJson` member. This member can be used by DTO implementations to store or fetch additional user settings without extending database model. 
Due to this feature, user model has two separate DTO interfaces:
  + `IIdentityManagerUserDto`
  + `IIdentityManagerUserDto<T>` where T is type to which `AdditionalDataJson` can be serialized/deserialized

Apart from these two interfaces, it contains base class for other user DTO objects:
   + `IdentityManagerUserDtoBase` which implements `IIdentityManagerUserDto`
   + `IdentityManagerUserDtoBase<T>` which inherits `IdentityManagerUserDtoBase` and implements `IIdentityManagerUserDto<T>`. This class is usefull since it contains property `public T AdditionalData { get; set; }` which is getter/setter with serialization/deserialization for `AdditionalDataJson` member.

### Commons
To have standardized response, all identity services should return same result class `IdentityUtilsResult`. This class also has typed variant `IdentityUtilsResult<T>` in case service needs to return an object.

To make integration with .Net Core Identity easier, various extension methods are added to quickly transform `IdentityResult` to `IdentityUtilsResult`.
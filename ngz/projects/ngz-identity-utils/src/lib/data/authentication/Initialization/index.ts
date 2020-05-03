// Initialization data model
// ----------------------------------------------------------------------------

// Import dependencies
import { EnTT, Serializable, Validate } from '@ofzza/entt-rxjs';
import * as Yup from 'yup';

// Import child models
import { UserModel } from '../User';
import { TenantModel } from '../Tenant';
import { RoleModel } from '../Role';

/**
 * Initialization data model
 */
export class InitializationModel extends EnTT {
  constructor () { super(); super.entt(); }

  @Serializable({ cast: UserModel })
  public user = undefined as string;

  @Serializable({ cast: [TenantModel] })
  public tenants = [] as TenantModel[];

  @Serializable({ cast: [RoleModel] })
  public roles = [] as RoleModel[];

}

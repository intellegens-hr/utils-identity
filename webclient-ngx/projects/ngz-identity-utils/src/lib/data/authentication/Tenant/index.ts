// Tenant data model
// ----------------------------------------------------------------------------

// Import dependencies
import { EnTT, Serializable, Validate } from '@ofzza/entt-rxjs';
import * as Yup from 'yup';

// Import child models
import { RoleModel } from '../Role';

/**
 * Tenant data model
 */
export class TenantModel extends EnTT {
  constructor () { super(); super.entt(); }

  // TODO: ... define model

  @Serializable({ cast: [RoleModel] })
  public roles = [] as RoleModel[];

}

// User data model
// ----------------------------------------------------------------------------

// Import dependencies
import { EnTT, Serializable, Validate } from '@ofzza/entt-rxjs';
import * as Yup from 'yup';

// Import child models
import { RoleModel } from '../Role';

/**
 * User data model
 */
export class UserModel extends EnTT {
  constructor () { super(); super.entt(); }

  @Validate({ provider: Yup.string().required() })
  public id = undefined as string;

  @Validate({ provider: Yup.string().required() })
  public username = undefined as string;

  // TODO: ... define reset of model

  @Serializable({ cast: [RoleModel] })
  public roles = [] as RoleModel[];

}

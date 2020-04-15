// User data-model
// ----------------------------------------------------------------------------

// Import dependencies
import { EnTT, Validate } from 'entt-rxjs';
import * as Yup from 'yup';

/**
 * User data-model
 */
export class UserModel extends EnTT {
  constructor () { super(); super.entt(); }

  // @Validate({ provider: Yup.string().required().email() })
  @Validate({ provider: Yup.string().required() })
  public username = undefined as string;

}

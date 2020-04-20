// Login data models
// ----------------------------------------------------------------------------

// Import dependencies
import { EnTT, Serializable, Validate } from '@ofzza/entt-rxjs';
import * as Yup from 'yup';

/**
 * Login request data model
 */
export class LoginRequestModel extends EnTT {
  constructor () { super(); super.entt(); }

  @Validate({ provider: Yup.string().required().email() })
  public username = undefined as string;

  @Validate({ provider: Yup.string().required().min(6) })
  public password = undefined as string;

}

/**
 * Login token data model
 */
export class LoginAuthenticationToken extends EnTT {
  constructor () { super(); super.entt(); }

  @Serializable({ alias: 'access_token' })
  @Validate({ provider: Yup.string().required() })
  public accessToken = undefined as string;

  @Serializable({ alias: 'refresh_token' })
  @Validate({ provider: Yup.string().required() })
  public refreshToken = undefined as string;

  @Serializable({ alias: 'expires_in' })
  @Validate({ provider: Yup.number().required().integer().min(0) })
  public duration = undefined as number;

}

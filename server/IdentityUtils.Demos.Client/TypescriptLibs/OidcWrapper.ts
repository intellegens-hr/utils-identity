declare const Oidc: any;

interface RedirectParams {
    successUrl?: string;
    errorUrl?: string;
}

class OidcWrapper {
    private usermanagerConfig = {
        authority: ConfigAuthorizationAuthority,
        client_id: ConfigAuthorizationClientId,
        response_type: 'code',
        scope: ConfigAuthorizationClientScope,
        automaticSilentRenew: true,
        loadUserInfo: false,
        metadata: {
            issuer: ConfigAuthorizationAuthority,
            authorization_endpoint: ConfigIs4AuthorizationEndpoint,
            token_endpoint: ConfigIs4TokenEndpoint,
            end_session_endpoint: ConfigIs4EndSessionEndpoint,
            jwks_uri: ConfigIs4JwksEndpoint,
        }
    };

    public UserManager;

    constructor() {
        this.UserManager = new Oidc.UserManager(this.usermanagerConfig);
    }

    private async GetUser(): Promise<unknown> {
        return await this.UserManager.getUser();
    }

    public async IsUserLoggedIn(): Promise<boolean> {
        const user = await this.GetUser();
        return !(user === null)
    }
}
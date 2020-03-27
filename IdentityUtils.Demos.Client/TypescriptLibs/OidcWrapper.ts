declare var Oidc: any;

interface RedirectParams {
    successUrl?: string;
    errorUrl?: string;
}

class OidcWrapper {
    //public static REDIRECT_PARAMS_SESSION_KEY = "redirect_params";

    private usermanagerConfig = {
        authority: ConfigAuthorizationAuthority,
        client_id: ConfigAuthorizationClientId,
        //redirect_uri: 'https://localhost:5010/index.html',
        response_type: 'code',
        scope: ConfigAuthorizationClientScope,
        automaticSilentRenew: true,
        //expires_in: 5,
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

    //public async Login(settings?: RedirectParams) {
    //    const defaultSettings: RedirectParams = {
    //        successUrl: window.location.href,
    //        errorUrl: "https://localhost:5005/login_failed.html"
    //    }

    //    settings = Object.assign(defaultSettings, settings);

    //    sessionStorage.setItem(OidcWrapper.REDIRECT_PARAMS_SESSION_KEY, JSON.stringify(settings));
    //    await this.UserManager.signinRedirect({ state: settings });
    //}
}
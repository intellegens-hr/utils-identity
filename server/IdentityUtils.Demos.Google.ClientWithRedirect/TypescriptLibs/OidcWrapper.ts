declare const Oidc: any;

interface RedirectParams {
    successUrl?: string;
    errorUrl?: string;
}

class OidcWrapper {
    private usermanagerConfig = {
        authority: ConfigAuthorizationAuthority,
        client_id: ConfigAuthorizationClientId,
        response_type: 'id_token token',
        scope: ConfigAuthorizationClientScope,
        automaticSilentRenew: true,
        loadUserInfo: true,
        redirect_uri: "https://localhost:5002/login_callback.html"
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
class OidcWrapper {
    constructor() {
        this.usermanagerConfig = {
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
        this.UserManager = new Oidc.UserManager(this.usermanagerConfig);
    }
    async GetUser() {
        return await this.UserManager.getUser();
    }
    async IsUserLoggedIn() {
        const user = await this.GetUser();
        return !(user === null);
    }
}
function AjaxCall(url, method, payload, authToken) {
    return new Promise(function (resolve, reject) {
        const xhr = new XMLHttpRequest();
        xhr.onload = () => {
            const result = xhr.response;
            resolve(JSON.parse(result));
        };
        xhr.open(method, url);
        xhr.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
        if (authToken)
            xhr.setRequestHeader("Authorization", "Bearer " + authToken);
        if (payload)
            xhr.send(JSON.stringify(payload));
        else
            xhr.send();
    });
}
//TODO: require ...
class IndexView {
    constructor() {
        this.oidcWrapper = new OidcWrapper();
        this.SetupIdToken();
        this.SetupLogout();
        window["debug_userManager"] = this.oidcWrapper.UserManager;
        this.LoginCheck();
    }
    Log(log) {
        window["claimsTextArea"].value += `${log}}\r\n\r\n`;
    }
    LogHeader(title) {
        window["claimsTextArea"].value += `\r\n\r\n${title}:\r\n------------\r\n\r\n`;
    }
    JsonPrettyStringify(object) {
        return JSON.stringify(object, undefined, 2);
    }
    LoginCheck() {
        this.oidcWrapper.IsUserLoggedIn().then(isLoggedIn => {
            const statusH2 = window["loginStatus"];
            if (isLoggedIn) {
                statusH2.innerHTML = "You are logged-in";
                statusH2.style.color = "green";
            }
            else {
                statusH2.innerHTML = "You are not logged-in";
                statusH2.style.color = "red";
            }
        });
    }
    async Logout() {
        const user = await this.oidcWrapper.UserManager.getUser();
        const tokenData = {
            "refreshToken": user.refresh_token,
            "accessToken": user.access_token
        };
        return await AjaxCall(ConfigApiLogoutEndpoint, "POST", tokenData, user.access_token);
    }
    async UserProfile() {
        const user = await this.oidcWrapper.UserManager.getUser();
        let token = "";
        if (user)
            token = user.access_token;
        const data = await AjaxCall(ConfigApiUserProfileEndpoint, "GET", null, token);
        this.LogHeader("User status");
        this.Log(this.JsonPrettyStringify(data));
    }
    SetupIdToken() {
        document
            .getElementById("btnIdCredentials")
            .addEventListener("click", () => {
            this.UserProfile();
        });
        document
            .getElementById("btnIdToken")
            .addEventListener("click", async () => {
            const loginData = {
                "username": window["username"].value,
                "password": window["password"].value
            };
            const data = await AjaxCall(ConfigApiLoginEndpoint, "POST", loginData);
            const token = JSON.parse(data.tokenData);
            const user = new Oidc.User(token);
            user.expires_in = token.expires_in;
            console.log(token);
            const wrapper = new OidcWrapper();
            wrapper.UserManager.storeUser(user);
            this.LoginCheck();
            setTimeout(() => {
                this.UserProfile();
            }, 100);
        });
    }
    SetupLogout() {
        document
            .getElementById("btnLogoutAjax")
            .addEventListener("click", async () => {
            const result = await this.Logout();
            this.LogHeader("Logout");
            this.Log(this.JsonPrettyStringify(result));
            this.oidcWrapper.UserManager.removeUser();
            this.LoginCheck();
        });
    }
}
//# sourceMappingURL=app.js.map
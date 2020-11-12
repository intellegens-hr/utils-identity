class OidcWrapper {
    constructor() {
        this.usermanagerConfig = {
            authority: ConfigAuthorizationAuthority,
            client_id: ConfigAuthorizationClientId,
            response_type: 'id_token token',
            scope: ConfigAuthorizationClientScope,
            automaticSilentRenew: true,
            loadUserInfo: true,
            redirect_uri: "https://localhost:5002/login_callback.html"
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
        this.SetupLogin();
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
    async LoginCheck() {
        const isLoggedIn = await this.oidcWrapper.IsUserLoggedIn();
        const statusH2 = window["loginStatus"];
        if (isLoggedIn) {
            statusH2.innerHTML = "You are logged-in";
            statusH2.style.color = "green";
            const user = await this.oidcWrapper.UserManager.getUser();
            this.LogHeader("User profile");
            this.Log(JSON.stringify(user.profile));
            const claims = await AjaxCall("/api/profile", "GET", null, user.access_token);
            this.LogHeader("Claims");
            this.Log(JSON.stringify(claims));
        }
        else {
            statusH2.innerHTML = "You are not logged-in";
            statusH2.style.color = "red";
        }
    }
    async Logout() {
        const user = await this.oidcWrapper.UserManager.getUser();
    }
    SetupLogin() {
        document
            .getElementById("btnLogin")
            .addEventListener("click", async () => {
            this.oidcWrapper.UserManager.signinRedirect({
                extraQueryParams: {
                    foo: 'bar',
                    batz: 'quux',
                },
            });
        });
        document
            .getElementById("btnLoginGoogle")
            .addEventListener("click", async () => {
            this.oidcWrapper.UserManager.signinRedirect({
                extraQueryParams: {
                    provider: 'Google'
                },
            });
        });
    }
    SetupLogout() {
        //document
        //    .getElementById("btnLogoutAjax")
        //    .addEventListener("click", async () => {
        //        //const result = await this.Logout();
        //        //this.LogHeader("Logout")
        //        //this.Log(this.JsonPrettyStringify(result));
        //        //this.oidcWrapper.UserManager.removeUser();
        //        //this.LoginCheck();
        //    })
    }
}
//# sourceMappingURL=app.js.map
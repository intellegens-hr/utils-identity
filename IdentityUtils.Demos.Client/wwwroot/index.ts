//TODO: require ...

class IndexView {
    private oidcWrapper = new OidcWrapper();

    private Log(log: string) {
        window["claimsTextArea"].value += `${log}}\r\n\r\n`;
    }

    private LogHeader(title: string) {
        window["claimsTextArea"].value += `\r\n\r\n${title}:\r\n------------\r\n\r\n`;
    }

    constructor() {
        this.SetupIdToken();
        this.SetupLogout();

        window["debug_userManager"] = this.oidcWrapper.UserManager;

        this.LoginCheck();
    }

    private JsonPrettyStringify(object: Object) {
        return JSON.stringify(object, undefined, 2);
    }

    private LoginCheck() {
        this.oidcWrapper.IsUserLoggedIn().then(isLoggedIn => {
            const statusH2: HTMLHeadingElement = window["loginStatus"];

            if (isLoggedIn) {
                statusH2.innerHTML = "You are logged-in";
                statusH2.style.color = "green";
            }
            else {
                statusH2.innerHTML = "You are not logged-in";
                statusH2.style.color = "red";
            }
        })
    }

    private async Logout() {
        const user = await this.oidcWrapper.UserManager.getUser();

        const tokenData = {
            "refreshToken": user.refresh_token,
            "accessToken": user.access_token
        };

        return await AjaxCall<any>(ConfigApiLogoutEndpoint, "POST", tokenData, user.access_token);
    }

    private async UserProfile() {
        const user = await this.oidcWrapper.UserManager.getUser();
        let token = "";
        if (user)
            token = user.access_token;

        const data = await AjaxCall<any>(ConfigApiUserProfileEndpoint, "GET", null, token);

        this.LogHeader("User status");

        this.Log(this.JsonPrettyStringify(data));
    }

    private SetupIdToken() {
        document
            .getElementById("btnIdCredentials")
            .addEventListener("click", () => {
                this.UserProfile();
            })

        document
            .getElementById("btnIdToken")
            .addEventListener("click", async () => {
                const loginData = {
                    "username": window["username"].value,
                    "password": window["password"].value
                };

                const data = await AjaxCall<any>(ConfigApiLoginEndpoint, "POST", loginData);

                const token = JSON.parse(data.tokenData);
                const user = new Oidc.User(token);
                //user.expires_in = token.expires_in;
                user.expires_in = 30;

                console.log(token)
                window["debug_user"] = user;

                const wrapper = new OidcWrapper();
                wrapper.UserManager.storeUser(user);

                this.LoginCheck()

                setTimeout(() => {
                    this.UserProfile();
                }, 100)
            })
    }

    private SetupLogout() {
        document
            .getElementById("btnLogoutAjax")
            .addEventListener("click", async () => {
                const result = await this.Logout();

                this.LogHeader("Logout")
                this.Log(this.JsonPrettyStringify(result));

                this.oidcWrapper.UserManager.removeUser();

                this.LoginCheck();
            })
    }
}
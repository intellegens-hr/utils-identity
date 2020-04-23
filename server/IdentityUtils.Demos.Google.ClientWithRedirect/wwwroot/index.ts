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
        this.SetupLogin();
        this.SetupLogout();

        window["debug_userManager"] = this.oidcWrapper.UserManager;

        this.LoginCheck();
    }

    private JsonPrettyStringify(object: Object) {
        return JSON.stringify(object, undefined, 2);
    }

    private async LoginCheck() {
        const isLoggedIn = await this.oidcWrapper.IsUserLoggedIn();

        const statusH2: HTMLHeadingElement = window["loginStatus"];

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

    private async Logout() {
        const user = await this.oidcWrapper.UserManager.getUser();

    }

    private SetupLogin() {
        document
            .getElementById("btnLogin")
            .addEventListener("click", async () => {
                this.oidcWrapper.UserManager.signinRedirect({
                    extraQueryParams: { //your params go here
                        foo: 'bar',
                        batz: 'quux',
                    },
                });
            });

        document
            .getElementById("btnLoginGoogle")
            .addEventListener("click", async () => {
                this.oidcWrapper.UserManager.signinRedirect({
                    extraQueryParams: { //your params go here
                        provider: 'Google'
                    },
                });
            })
    }

    private SetupLogout() {
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
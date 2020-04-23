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

    }

    private SetupLogin() {
        document
            .getElementById("btnLogin")
            .addEventListener("click", async () => {
                const result = await this.Logout();

                this.oidcWrapper.UserManager.signinRedirect();
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
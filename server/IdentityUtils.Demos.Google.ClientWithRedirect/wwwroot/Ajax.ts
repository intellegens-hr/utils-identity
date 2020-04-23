function AjaxCall<T>(url, method: "GET" | "POST", payload: object, authToken?) {
    return new Promise<T>(function (resolve, reject) {
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
    })
}
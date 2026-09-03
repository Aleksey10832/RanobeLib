export default async function req(url: string, method: string = "GET", value?: object){
    const accToken = localStorage.getItem("accessToken");
    if(accToken){
        const headers = {"Authorization": "Bearer " + accToken, "Content-Type": "application/json"};
        const response = await fetch('/api/back/' + url, {method: method, headers: headers, body: JSON.stringify(value)})
        if(response.status == 401){
            const refTokens = await fetch('/api/back/user/token/refersh/' + localStorage.getItem("refershToken"), {method: "POST", headers: headers})
            if(refTokens.status == 401){
                localStorage.clear()
                return 401
            } else{
                const tokens = await refTokens.json()
                localStorage.setItem("refershToken", tokens.refershToken)
                localStorage.setItem("accessToken", tokens.accessToken)
                return await req(url, method, value)
            }
        } else if(response.status < 300){
            return await response.json()
        }
        return response.status
    } else {
        const response = await fetch('/api/back/' + url, {method: method, headers: {"Content-Type": "application/json"}, body: JSON.stringify(value)})
        if(response.status == 401){
            return 401
        }
        return await response.json()
    }
}
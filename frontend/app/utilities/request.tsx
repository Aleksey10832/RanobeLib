export default async function req(url: string, method: string = "GET", value?: object){
    const accToken = localStorage.getItem("accessToken");
    if(accToken){
        const response = await fetch('/api/back/' + url, {method: method, headers: {"Authorization": accToken, "Content-Type": "application/json"}, body: JSON.stringify(value)})
        if(response.status == 401){
            const refTokens = await fetch('/api/back/user/token/refersh', {method: "POST", headers: {"Authorization": accToken, "Content-Type": "application/json"}, body: JSON.stringify({refershToken: localStorage.getItem("refershToken")})})
            if(refTokens.status == 401){
                localStorage.clear()
                return 401
            } else{
                const tokens = await refTokens.json()
                localStorage.setItem("refershToken", tokens.refershToken)
                localStorage.setItem("accessToken", tokens.accessToken)
                req(url, method, value)
            }
        }
        return await response.json()
    } else {
        const response = await fetch('/api/back/' + url, {method: method, headers: {"Content-Type": "application/json"}, body: JSON.stringify(value)})
        if(response.status == 401){
            return 401
        }
        return await response.json()
    }
}
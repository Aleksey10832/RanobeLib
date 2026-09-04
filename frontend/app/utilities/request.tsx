'use client'
export default async function req(url: string, method: string = "GET", value?: object){
    const accToken = localStorage.getItem("accessToken");
    if(accToken){
        const headers = {"Authorization": "Bearer " + accToken, "Content-Type": "application/json"};
        const response = await fetch('/api/back/' + url, {method: method, headers: headers, body: JSON.stringify(value)})
        if(response.status == 401){
            const confirmToken = localStorage.getItem("confirmToken")
            if(confirmToken){
                const confTokenStatus = (await fetch("/api/back/session/confirm/" + confirmToken, {method: "POST", headers: headers})).status
                if(confTokenStatus == 200){
                    localStorage.removeItem("confirmToken")
                    return await req(url, method, value)
                } else if(confTokenStatus == 401){
                    localStorage.removeItem("confirmToken")
                } else {
                    return 500
                }
            } else {
                if(!await refershToken()){
                    return 401
                }
                return await req(url, method, value)

            }
        } else if(response.status < 300){
            return await response.json()
        }
        return await req(url, method, value)
    } else {
        const response = await fetch('/api/back/' + url, {method: method, headers: {"Content-Type": "application/json"}, body: JSON.stringify(value)})
        if(response.status == 401){
            return 401
        }
        return await response.json()
    }
}
let refershPromise: Promise<boolean> | null = null;
async function refershToken() {
    if(refershPromise){
        return refershPromise
    }
    refershPromise = (async () => {
        try{
            const headers = {"Authorization": "Bearer " + localStorage.getItem("accessToken"), "Content-Type": "application/json"};
            const refTokens = await fetch('/api/back/session/refersh/' + localStorage.getItem("refershToken"), {method: "POST", headers: headers})
            if(refTokens.status == 401){
                localStorage.clear()
                return false
            } else{
                const tokens = await refTokens.json()
                localStorage.setItem("refershToken", tokens.refershToken)
                localStorage.setItem("accessToken", tokens.accessToken)
                localStorage.setItem("confirmToken", tokens.confirmToken)
                console.log(tokens)
                const conTokenRespons = await fetch("/api/back/session/confirm/" + tokens.confirmToken, {method: "POST", headers: headers})
                if(!conTokenRespons.ok){
                    return false
                }
                localStorage.removeItem("confirmToken")
                return true
            }
        } catch {
            return false
        } finally {
            refershPromise = null
        }
        
    })()
    return refershPromise
}
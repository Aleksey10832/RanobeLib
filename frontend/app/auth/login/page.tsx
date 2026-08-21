'use client'
import { useRouter } from "next/navigation";
import { useState } from "react";
import "../auth.css"
import req from "@/app/utilities/request";

export default function Login(){
    const [login, setLogin] = useState("");
    const [password, setPassword] = useState("");
    const [errorMessage, setErrMessage] = useState("")
    const router = useRouter()
    async function logUser() {
        
        const tokens = await req('user/login', 'POST', {
                    login: login, 
                    password: password
                })
        if(tokens != 401){
            localStorage.setItem("refershToken", tokens.refershToken)
            localStorage.setItem("accessToken", tokens.accessToken)
            router.push('/')
        } else{
            setErrMessage("Логин или пароль не верен")
        }
    }
    return (
        <section>
            <input type="text" onChange={(e) => setLogin(e.target.value)} placeholder="Логин" />
            <input type="password"  onChange={(e) => setPassword(e.target.value)} placeholder="Пароль" />
            <button onClick={logUser}>Вход</button>
            <div className="bg-red-400 w-1/3 text-center error-message">{errorMessage}</div>
        </section>
    )
}
"use client"
import { JSX, useEffect, useState } from "react"
import req from "../utilities/request";
import Role from "../models/role";
import User from "../models/user";

export default function AdminPanel() {
    const [rolePage, setRolePage] = useState<JSX.Element>();
    useEffect(() => {
        req("role/get").then(response => {
            if(response != 403){
                setRolePage(<div>
                    {response.map((role: Role) => {
                        return (
                        <div className="border-t" key={role.id}>
                            <h1>Имя роли: {role.name}</h1>
                            {role.rules.map((rule, index) => {
                                return (<div key={index}>{rule}</div>)
                            })}
                        </div>)
                    })}
                </div>)
            }
        })
        req("user/getAll").then((users: User) => {
            if(typeOf(users) != "number"){
                users.map(user => {
                    return (<div>
                        <div> Индификатор пользователя: {user.Id} </div>
                        <div> Имя пользователя: {user.Name} </div>
                        <div> Роль пользователя: {user.Role} </div>
                    </div>)
                })
            }
        })
    }, [])
    return (
    <section>
        {rolePage}
    </section>)
}

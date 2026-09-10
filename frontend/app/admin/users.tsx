'use client'

import { JSX, useEffect, useState } from "react"
import User from "../models/user"
import req from "../utilities/request"

export default function UsersComponent(){
    const [users, setUsers] = useState<JSX.Element>()
    useEffect(() => {
        req("user/getAll/0").then((response: User[]) => {
            if(typeof(response) != "number"){
                setUsers((
                    <div>
                        {response.map((user: User) => {
                        return (
                            <div key={user.id}>
                                <div> Индификатор пользователя: {user.id} </div>
                                <div> Имя пользователя: {user.name} </div>
                                <div> Роль пользователя: {user.role} </div>
                            </div>
                        )
                        })}
                    </div> 
                ))
                
            }
        })
    }, [])
    
    return (
        users
    )
}
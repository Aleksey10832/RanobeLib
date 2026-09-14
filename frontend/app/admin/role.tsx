"use client"

import req from "../utilities/request"

import Role from "../models/role";


export default async function UsersComponent(){
    const response: Role[] = await req("role/get");
    if(typeof(response) != "number"){
        return (
            <div>
                {response.map((role: Role) => {
                return (
                        <div className="border-t" key={role.id}>
                            <h1>Имя роли: {role.name}</h1>
                            {role.rules.map((rule, index) => {
                                return (<div key={index}>{rule}</div>)
                            })}
                        </div>
                    )
                })}
            </div> 
        )
    } else {
        return (<></>)
    }
}
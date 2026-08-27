'use client'
import Link from "next/link";
import { useEffect, useState } from "react";
import req from "./utilities/request";

export default function Header (){
    const [links, setLinks] = useState([{name: "Главная", href: "/"}])
    useEffect(() => {
        const Tlinks = [ {
                name: "Главная", 
                href: "/"
            }, {
                name: "Вход", 
                href: "/auth/login"
            }
        ]
        req("user/profile").then(profile =>{
            if(typeof(profile) != "number"){
                if(profile.role == "Admin"){
                    Tlinks.push({name: "Админ панель", href: "admin"})
                }
            }
            setLinks(Tlinks)
        })
        
    }, [])
    
    return (
        <header className="mb-5 flex gap-15 m-3/4">
          {links.map((link, index) => {
            return (<Link key={index} href={link.href}>{link.name}</Link>)
          })}
        </header>
    )
}
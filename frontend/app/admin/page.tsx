"use client"
import UsersComponent from "./users";
import RolesComponent from "./role";
import { JSX, useState } from "react";

export default function AdminPanel() {
    const [mainSection, setMainSection] = useState<JSX.Element>()
    function setRolePage(){
        RolesComponent().then(value => {
            setMainSection(value);
        })
    }
    function setUserPage(){
        UsersComponent().then(value => {
            setMainSection(value);
        })
    }

    return (
    <section className="flex flex-row">
        <nav className="nav-buttons">
            <button onClick={setRolePage}>Roles</button>
            <button onClick={setUserPage}>Users</button>
        </nav>
        {mainSection}
    </section>)
}

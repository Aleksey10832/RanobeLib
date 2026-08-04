"use client";
import Image from "next/image";
import { useEffect, useState } from "react";

interface Ranobe {
  id: string;
  name: string
}
export default function Home() {
  const [ranobeList, setRanobeList] = useState([{}]);
  useEffect(() => {
    fetch("http://127.0.0.1:9000/ranobe/0").then(response => {
      response.json().then((value) => {
        const ranobe: Ranobe[] = value.value.ranobe
        setRanobeList(ranobe);
      })
      // setRanobeList(response.value);
    })
  });
  return (
    <div className="flex flex-row gap-10 mx-auto w-150">
      {ranobeList.map(el => {
        return (<div className="cart basis-1/3"><a href={"ranobe/" + el.id}>{el.name}</a></div>)
      })}
    </div>
  );
}

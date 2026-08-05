"use client";
import Ranobe from "@/app/models/ranobe";
import { useRouter } from "next/router";

import { useEffect, useState } from "react";

export default function RanobeF() {
  const [ranobeInfo, setRanobeList] = useState({id: "", name: "", pages: 0});
  
  useEffect(() => {
    fetch("http://127.0.0.1:9000/ranobe/").then(response => {
      response.json().then((value) => {
        const ranobeR: Ranobe = value.value.ranobe
        setRanobeList(ranobeR);
      })
    })
  });
  return (
    <div className="flex flex-row gap-10 mx-auto w-150">
        <div className="cart basis-1/3"><a href={"ranobe/" + ranobeInfo.id}>{ranobeInfo.name}</a></div>
    </div>
  );
}

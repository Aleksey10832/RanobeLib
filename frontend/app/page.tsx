'use client';
import Ranobe from "./models/ranobe";
import { useEffect, useState } from "react"


export default function RanobeAll() {
  const [ranobeList, setRanobeList] = useState([{id: "", name: "", pages: 0}]);
  useEffect(() => {
    fetch("http://localhost:9000/ranobe").then(response => 
      response.json()).then((value) => {
      const ranobeR: Ranobe[] = value.value.ranobe
      setRanobeList(ranobeR);
    })
  }, []);
  return (
    <div className="flex flex-row gap-10 mx-auto w-150">
      {ranobeList.map(el => {
        return (<div key={el.id} className="cart basis-1/3"><a href={"ranobe/" + el.id + "/0"}>{el.name}</a></div>)
      })}
    </div>
  );
}

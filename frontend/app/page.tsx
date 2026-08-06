'use client';
import { useRouter } from "next/navigation";
import Ranobe from "./models/ranobe";
import { useEffect, useState } from "react"


export default function RanobeAll() {
  const [ranobeList, setRanobeList] = useState<Ranobe[]>([{id: "", name: "", pages: 0}]);
  const router = useRouter()
  function toRanobe(rId: string){
    router.push("/ranobe/" + rId + "/0")
  }
  useEffect(() => {
    fetch("/api/back/ranobe").then(response => 
      response.json()).then((value) => {
      const ranobeR: Ranobe[] = value.value.ranobe
      setRanobeList(ranobeR);
    })
  }, []);
  return (
    <div className="flex flex-row gap-10 mx-auto w-150">
      {ranobeList.map(el => {
        return (
          <div key={1} className="cart basis-1 cursor-pointer" onClick={() => toRanobe(el.id)}>{el.name}</div>
      )
      })}
    </div>
  );
}

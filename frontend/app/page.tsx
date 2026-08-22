'use client';
import { useRouter } from "next/navigation";
import Ranobe from "./models/ranobe";
import { ReactElement, useEffect, useState } from "react"
import req from "./utilities/request";


export default function RanobeAll() {
  const [ranobeList, setRanobeList] = useState<Ranobe[]>([{id: "", name: "", pages: 0}]);
  // const [carts, setCarts] = useState<Array<ReactElement>>()
  const router = useRouter()
  function toRanobe(rId: string){
    router.push("/ranobe/" + rId + "/0")
  }
  useEffect(() => {
    req("ranobe/0").then(response => {
      if(response == 401){
        router.replace('login')
      }
      const ranobeR: Ranobe[] = response.ranobe
      setRanobeList(ranobeR);
    })
  }, []);
  return (
    <div className="grid md:grid-cols-3 sm:grid-cols-1 gap-10 mx-auto w-4/5">
      {ranobeList.map((el) => {
        return (
          <div key={el.id} className="cart cursor-pointer" onClick={() => toRanobe(el.id)}>{el.name}</div>
        )
      })}
    </div>
  );
}

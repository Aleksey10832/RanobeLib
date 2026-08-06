"use client";
import Ranobe from "@/app/models/ranobe";
import { useParams } from "next/navigation";
import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";

export default function RanobeF() {
  const [ranobeInfo, setRanobeList] = useState<Ranobe>({id: "", name: "", chapters: [], pages: 0});
  const [ranobeId, page] = [useParams().ranobeId, useParams().page]
  const router = useRouter()
  function toChapter(cId: number){
    router.push("chapter/" + cId.toString())
  }
  useEffect(() => {
    fetch(`/api/back/ranobe/${ranobeId}/${page}`).then(response => {
      response.json().then((value) => {
        
        const ranobeR: Ranobe = value.value
        setRanobeList(ranobeR);
      })
    })
  }, []);
  return (
    <div>
        <div className="text-3xl text-center pt-5 mb-5"><a href={"ranobe/" + ranobeInfo.id}>{ranobeInfo.name}</a></div>
        <div>
          {ranobeInfo.chapters?.map(chapter => {
            return <div onClick={() => toChapter(chapter.number)} className="mb-1 m-auto p-2 text-center text-1xl bg-gray-700 w-2xl rounded-sm hover:text-2xl cursor-pointer hover:transition-transform" key={chapter.id}>{chapter.name}</div>
          })}
        </div>
    </div>
  );
}

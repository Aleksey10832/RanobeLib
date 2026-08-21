"use client";
import Ranobe from "@/app/models/ranobe";
import { useParams } from "next/navigation";
import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";

export default function RanobeF() {
  const [ranobeInfo, setRanobeList] = useState<Ranobe>({id: "", name: "", chapters: [], pages: 0});
  const [ranobeId, page] = [useParams().ranobeId, Number(useParams().page)]
  const router = useRouter()
  function toChapter(cId: number){
    router.push("chapter/" + cId.toString())
  }
  function newPage(page: number){
    router.push(page.toString())
  }
  useEffect(() => {
    fetch(`/api/back/ranobe/${ranobeId}/${page}`).then(response => {
      if(response.status > 299){
        router.back();
      }
      response.json().then((value) => {
        if(value.chapters.length < 1){
          router.push("0");
        }
        setRanobeList(value);
      })
    })
  }, []);
  return (
    <div>
        <div className="text-3xl text-center pt-5 mb-1 select-none">{ranobeInfo.name}</div>
        <div className="flex mb-5">
            <button onClick={() => newPage(page - 1)} className="cursor-pointer m-auto px-10 py-3 bg-mauve-400 rounded-2xl">{"<-"}</button>
            <button onClick={() => newPage(page + 1)} className="cursor-pointer m-auto px-10 py-3 bg-mauve-400 rounded-2xl">{"->"}</button>
        </div>
        <div>
          {ranobeInfo.chapters?.map(chapter => {
            return <div onClick={() => toChapter(chapter.number)} className="select-none mb-1 m-auto p-2 text-center text-1xl bg-gray-700 w-7/8 rounded-sm hover:text-2xl cursor-pointer hover:transition-transform" key={chapter.id}>{chapter.name}</div>
          })}
        </div>
        <div className="flex mt-3 mb-3">
            <button onClick={() => newPage(page - 1)} className="cursor-pointer m-auto px-10 py-3 bg-mauve-400 rounded-2xl">{"<-"}</button>
            <button onClick={() => newPage(page + 1)} className="cursor-pointer m-auto px-10 py-3 bg-mauve-400 rounded-2xl">{"->"}</button>
        </div>
    </div>
  );
}
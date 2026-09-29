module RankFusion

let k = 60.0

let fuse (rankings: string[][]) =
    rankings
    |> Array.collect (Array.mapi (fun rank id -> id, 1.0 / (k + float rank + 1.0)))
    |> Array.groupBy fst
    |> Array.map (fun (id, scores) -> id, Array.sumBy snd scores)
    |> Array.sortByDescending snd
    |> Array.map fst

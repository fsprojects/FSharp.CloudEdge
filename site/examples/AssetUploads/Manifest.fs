module Manifest

open System.IO
open FSharp.CloudEdge.Core.Api.Types

type Asset = { Key: string; Path: string; Hash: string; Size: int }

let scan (folder: string) =
    [ for path in Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories) ->
          { Key = "/" + Path.GetRelativePath(folder, path).Replace('\\', '/')
            Path = path
            Hash = FileHash.hashFile path
            Size = int (FileInfo path).Length } ]

let manifest (assets: Asset list) =
    assets
    |> List.map (fun asset -> asset.Key, workers_manifest_u002D_value.Create(asset.Hash, asset.Size))
    |> Map.ofList

module ChangeCheck

open System
open System.IO
open System.Security.Cryptography
open System.Text

let lastUpload = "dist/last-upload.txt"

let fingerprint (metadata: string) (bundle: byte[]) =
    Array.append (Encoding.UTF8.GetBytes metadata) bundle
    |> SHA256.HashData
    |> Convert.ToHexString

let unchanged (fingerprint: string) =
    File.Exists lastUpload && File.ReadAllText lastUpload = fingerprint

let remember (fingerprint: string) =
    File.WriteAllText(lastUpload, fingerprint)

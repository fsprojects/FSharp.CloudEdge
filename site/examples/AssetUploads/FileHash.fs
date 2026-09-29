module FileHash

open System
open System.IO
open System.Security.Cryptography
open System.Text

let hashFile (path: string) =
    let base64 = Convert.ToBase64String(File.ReadAllBytes path)
    let extension = Path.GetExtension(path).TrimStart '.'
    let digest = SHA256.HashData(Encoding.UTF8.GetBytes(base64 + extension))
    Convert.ToHexString(digest, 0, 16).ToLowerInvariant()

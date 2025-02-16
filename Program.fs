open System
open NUnit.Framework
open FsUnit

[<Test>]
let ``test hello`` () =
    5 + 2 |> should equal 6

[<EntryPoint>]
let main argv =
    let who = Console.ReadLine()
    printfn "Hello world from %s" who
    0 // return an integer exit code
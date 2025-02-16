open System

[<EntryPoint>]
let main argv =
    let who = Console.ReadLine()
    printfn "Hello world from %s" who
    0 // return an integer exit code
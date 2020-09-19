
#time "on"
#r "nuget: Akka.FSharp" 
#r "nuget: Akka.TestKit" 

open System
open Akka.Actor
open Akka.Configuration
open Akka.FSharp
open Akka.TestKit

open Akka.FSharp

type ProcessorMessage = ProcessJob of int * int * int  

let perfectSquare n =
    let h = n &&& 0xF
    if (h > 9) then false
    else
        if ( h <> 2 && h <> 3 && h <> 5 && h <> 6 && h <> 7 && h <> 8 ) then
            let t = ((n |> double |> sqrt) + 0.5) |> floor|> int
            t*t = n
        else false

let echo (mailbox:Actor<'a>) =
    let rec loop () = actor {
        let! ProcessJob(x, y, z) = mailbox.Receive ()
        for i = x to y do
            let mutable k = 0
            for j = i to i+z-1 do 
            //printfn "%d" j
                k <- k + j*j
            if (perfectSquare k) then 
                 printfn "%d " i
        return! loop ()
    }
    loop ()
                            
let args = fsi.CommandLineArgs
let parseParams (args:string []) =
    let n=(int) args.[1]
    let k=(int) args.[2]
    let coreCount = Environment.ProcessorCount
    let mutable numberOfCores= 10*coreCount
    let range= (n/numberOfCores)
    let arange=(ceil (range|>float))|>int
    let system = System.create "system" <| Configuration.defaultConfig()
    let echoActors = 
        [1 .. numberOfCores]
        |> List.map(fun id ->   let properties = string(id) 
                                spawn system properties echo)

    let mutable s=1
    let mutable e=arange
    for id in [0 .. numberOfCores-1] do
       
        if id = numberOfCores-1 then
            (id) |> List.nth echoActors <! ProcessJob(e+1, n, k)
        else 
            (id) |> List.nth echoActors <! ProcessJob(s, e, k)
        s <- e + 1
        e <- e + arange

match args.Length with
    | 3 -> parseParams args |> ignore    
    | _ ->  failwith "You need to pass two parameters!" 

#time "on"
#r "nuget: Akka.FSharp" 
#r "nuget: Akka.TestKit" 

open System
open Akka.Actor
open Akka.Configuration
open Akka.FSharp
open Akka.TestKit

open Akka.FSharp

type ProcessorMessage = ProcessJobNew of int * int * int
let mutable counter=0  

let system = System.create "system" <| Configuration.defaultConfig()

let perfectSquare n =
    let h = n &&& 0xF
    if (h > 9) then false
    else
        if ( h <> 2 && h <> 3 && h <> 5 && h <> 6 && h <> 7 && h <> 8 ) then
            let t = ((n |> double |> sqrt) + 0.5) |> floor|> int
            t*t = n
        else false

let compute x y z =
    for i = x to y do
        let mutable k = 0
        for j = i to i+z-1 do 
            //printfn "%d" j
            k <- k + j*j
        if (perfectSquare k) then 
            printfn "%d " i
    
let echo (mailbox:Actor<_>) =
    let rec loop () = actor {
        let! ProcessJobNew(x, y, z) = mailbox.Receive ()
        //let sender = mailbox.Sender()
        // let response = "hello"
        // counter<-counter+1
        printfn "in child"
        compute x y z
        //sender <! counter
        return! loop ()
    }
    loop ()

type masterMessage = ProcessJob of int * int * int
    
let master (mailbox:Actor<_>) = 
    let rec masterLoop () = actor {
        let! ProcessJob(numberOfCores, arange, k) = mailbox.Receive ()
        //let sender = mailbox.Sender()
        let echoActors = 
            [1 .. numberOfCores]
            |> List.map(fun id -> spawn system (string(id)) echo)  
                       
        let mutable s=1
        let mutable e=arange
        let n = 100
        let mutable response = ""
        for id in [0 .. numberOfCores-1] do   
            if id = numberOfCores-1 then
                (id) |> List.nth echoActors <! ProcessJobNew(e+1, 1000, k)
                // let task =  (echoActors.[id] <? ProcessJobNew(e+1, 1000, k))
                // response <- Async.RunSynchronously(task)
            else 
                (id) |> List.nth echoActors <! ProcessJob(s, e, k)
            s <- e + 1
            e <- e + arange
        return! masterLoop()
    } 
    masterLoop()


                            
let args = fsi.CommandLineArgs
let parseParams (args:string []) =
    let n=(int) args.[1]
    let k=(int) args.[2]
    let coreCount = Environment.ProcessorCount
    let mutable numberOfCores= 10*coreCount
    let range= (n/numberOfCores)
    let arange=(ceil (range|>float))|>int

    let echoActor = spawn system "master" master
    echoActor <! ProcessJob(numberOfCores, arange, k)

    System.Console.ReadKey() |> ignore
    // let echoActors = 
    //     [1 .. numberOfCores]
    //     |> List.map(fun id ->   let properties = string(id) 
    //                             spawn system properties echo)

    // let mutable s=1
    // let mutable e=arange
    // for id in [0 .. numberOfCores-1] do
       
    //     if id = numberOfCores-1 then
    //         (id) |> List.nth echoActors <! ProcessJob(e+1, n, k)
    //     else 
    //         (id) |> List.nth echoActors <! ProcessJob(s, e, k)
    //     s <- e + 1
    //     e <- e + arange
    
match args.Length with
    | 3 -> parseParams args |> ignore    
    | _ ->  failwith "You need to pass two parameters!" 
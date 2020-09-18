// #time "on"
// // #load "Bootstrap.fsx"

// #r "nuget: Akka.FSharp" 
// #r "nuget: Akka.TestKit" 

// open System
// open Akka.Actor
// open Akka.Configuration
// open Akka.FSharp
// open Akka.TestKit

// // #Actor Implementation
// // An Actor is more lightweight than a thread. Millions of actors can be generated in Akka,
// // the secret is that an Actor can reuse a thread.
// //
// // The mapping relationship between an Actor and a Thread is decided by a Dispatcher.
// // 
// // This example creates 10 Actors, and prints its thread name when invoked.
// //
// // You will find there is no fixed mapping relationship between Actors and Threads. 
// // An Actor can use many threads. And a thread can be used by many Actors.
// let perfectSquare n =
//     let h = n &&& 0xF
//     if (h > 9) then false
//     else
//         if ( h <> 2 && h <> 3 && h <> 5 && h <> 6 && h <> 7 && h <> 8 ) then
//             let t = ((n |> double |> sqrt) + 0.5) |> floor|> int
//             t*t = n
//         else false

// let perfectSquares num1 num2 =
//   //function body
//     for i = 1 to num1 do
//         let mutable k = 0
//         for j = i to i+num2-1 do 
//             //printfn "%d" j
//             k <- k + j*j
//         if (perfectSquare k) then 
//             printfn "%d" i

// let system = ActorSystem.Create("FSharp")

// type EchoServer(name) =
//     inherit Actor()

//     override x.OnReceive message =
//         let tid = Threading.Thread.CurrentThread.ManagedThreadId
//         match message with
//         | :? string as msg -> printfn "Hello %s from %s at #%d thread" msg name tid
//         | _ ->  failwith "unknown message"

// let echoServers = 
//     [1 .. 1000]
//     |> List.map(fun id ->   let properties = [| string(id) :> obj |]
//                             system.ActorOf(Props(typedefof<EchoServer>, properties)))

// let rand = Random(1234)

// for id in [1 .. 1000] do
//     (rand.Next() % 10) |> List.nth echoServers <! perfectSquares 3 2

// system.Terminate()

// open System

// printfn "%d" a
// let n=1000000
// let k=24
// let  coreCount = 6
// let numberOfCores=10*6
// let range= (n/numberOfCores)
// let arange=(ceil (range|>float))|>int


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
        
        // printfn "Received message %d" x
        for i = x to y do
            let mutable k = 0
            for j = i to i+z-1 do 
            //printfn "%d" j
                k <- k + j*j
            if (perfectSquare k) then 
                printfn "%d " i
                printfn "aaaaaaaaaaaaaaaaaaaaaaaa"
        return! loop ()
    }
    loop ()
//[<EntryPoint>]
// printfn "LOL"
let args = fsi.CommandLineArgs
let parseParams (args:string []) =
    let n=(int) args.[1]
    let k=(int) args.[2]
    let coreCount = 6
    let mutable numberOfCores=10*6
    let range= (n/numberOfCores)
    let arange=(ceil (range|>float))|>int

    let system = System.create "system" <| Configuration.defaultConfig()
    let echoActors = 
        [1 .. numberOfCores+1]
        |> List.map(fun id ->   let properties = string(id) 
                                spawn system properties echo)
                
    let mutable s=1
    let mutable e=s+arange
    for id in [0 .. numberOfCores] do
        if id<>numberOfCores then
            (id) |> List.nth echoActors <! ProcessJob(e+1, n, k)
        (id) |> List.nth echoActors <! ProcessJob(s, e, k)
        s<-e+1
        e<-e+arange
        numberOfCores<-numberOfCores-1

    
    // (0)|>echoActors<!ProcessJob(e+1,n,k)


match args.Length with
    | 3 -> parseParams args |> ignore    
    | _ ->  failwith "You need to pass two parameters!" 


                              
// printfn "echoActors:%A" echoActors
    
//open System

// for item in new System.Management.ManagementObjectSearcher("Select * from Win32_Processor").Get())
// {
//     coreCount<-coreCount+int.Parse(item["NumberOfCores"].ToString());
// }
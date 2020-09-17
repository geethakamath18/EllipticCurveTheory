
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
        return! loop ()
    }
    loop ()
                            
let system = System.create "system" <| Configuration.defaultConfig()
                        
// use spawn in conjunction with actor computation expression
let echoActor1 = spawn system "echo1" echo
let echoActor2 = spawn system "echo2" echo
let echoActor3 = spawn system "echo3" echo
let echoActor4 = spawn system "echo4" echo
let echoActor5 = spawn system "echo5" echo
let echoActor6 = spawn system "echo6" echo                
// tell a message
echoActor1 <! ProcessJob(1, 1000000000, 2)
// echoActor2 <! ProcessJob(501, 1000, 10)
// echoActor3 <! ProcessJob(1001, 1500, 10)
// echoActor4 <! ProcessJob(1501, 2000, 10)
// echoActor5 <! ProcessJob(2001, 2500, 10)
// echoActor6 <! ProcessJob(2501, 3000, 10)
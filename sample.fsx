
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

let echoActors = 
    [1 .. 10000]
    |> List.map(fun id ->   let properties = string(id) 
                            spawn system properties echo)
// printfn "echoActors:%A" echoActors
for id in [0 .. 9999] do
    (id) |> List.nth echoActors <! ProcessJob(1, 10000,  2)

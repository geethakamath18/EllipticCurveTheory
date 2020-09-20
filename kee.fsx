
#time "on"
#r "nuget: Akka.FSharp" 
#r "nuget: Akka.TestKit" 

open System
open Akka.Actor
open Akka.Configuration
open Akka.FSharp
open Akka.TestKit

open Akka.FSharp


let system = ActorSystem.Create("FSharp")

type EchoServer(name) =
    inherit Actor()

    override x.OnReceive message =
        let tid = Threading.Thread.CurrentThread.ManagedThreadId
        match message with
        | :? string as msg -> printfn "Hello %s from %s at #%d thread" msg name tid
        | _ ->  failwith "unknown message"

let echoServers = 
    [1 .. 10]
    |> List.map(fun id ->   let properties = [| string(id) :> obj |]
                            system.ActorOf(Props(typedefof<EchoServer>, properties)))

let rand = Random(1234)

for id in [1 .. 10] do
    (rand.Next() % 10) |> List.nth echoServers <! sprintf "F# request %d!" id
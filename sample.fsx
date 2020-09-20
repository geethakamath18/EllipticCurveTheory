
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
type MasterMessage = MasterJob of int * int * int * int

let system = System.create "system" <| Configuration.defaultConfig()

let mutable counter = 0

let coreCount = Environment.ProcessorCount
let mutable numberOfCores= 2000*coreCount

type Geetha = 
    | Keerthi of string

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
            k <- k + j*j
        if (perfectSquare k) then 
            printfn "%d " i

let echo (mailbox:Actor<_>) =
    let rec loop () = actor {
        let! ProcessJob(x, y, z) = mailbox.Receive ()
        let sender = mailbox.Sender()
        compute x y z
        sender <! Keerthi "message recived"
        return! loop ()
    }
    loop ()

let master (mailbox: Actor<_>) = 
    let rec masterloop() = actor{

        let mutable s=1
        let args = fsi.CommandLineArgs
        let n=(int) args.[1]
        let k=(int) args.[2]

        let range= (n/numberOfCores)
        let arange=(ceil (range|>float))|>int

        let mutable e=arange
        let echoActors = 
            [1 .. numberOfCores]
            |> List.map(fun id ->   let properties = string(id) 
                                    spawn system properties echo)

        for id in [0 .. numberOfCores-1] do
            if id = numberOfCores-1 then
                (id) |> List.nth echoActors <! ProcessJob(e+1, n, k)
            else 
                (id) |> List.nth echoActors <! ProcessJob(s, e, k)
            s <- e + 1
            e <- e + arange
            let! gee  =  mailbox.Receive ()
            match gee with 
            |Keerthi gee ->
                counter <- counter+1 
        return! masterloop ()
    }      
    masterloop() 

let args = fsi.CommandLineArgs
let main (args:string []) =
    let mutable flag=true
    let masterActor = spawn system "master" master 
    masterActor <! "Start Master Actor"

    while flag do
        if numberOfCores=counter then
            flag<-false

match args.Length with
    | 3 -> main args    
    | _ ->  failwith "You need to pass two parameters!" 
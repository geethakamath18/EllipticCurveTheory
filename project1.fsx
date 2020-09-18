open System
let perfectSquare n =
    let h = n &&& 0xF
    if (h > 9) then false
    else
        if ( h <> 2 && h <> 3 && h <> 5 && h <> 6 && h <> 7 && h <> 8 ) then
            let t = ((n |> double |> sqrt) + 0.5) |> floor|> int
            t*t = n
        else false

let perfectSquares num1 num2 =
  //function body
    for i = 1 to num1 do
        let mutable k = 0
        for j = i to i+num2-1 do 
            //printfn "%d" j
            k <- k + j*j
        if (perfectSquare k) then 
            printfn "%d" i

// let res = perfectSquares 3 2
// [<EntryPoint>]
// let main=
//     //for arg in fsi.CommandLineArgs do
//         //printfn "LALAL"
//     printfn "lala"
//     let args = fsi.CommandLineArgs

//     let parseParams (args:string []) =
//         let param1=(int) args.[1]
//         let param2=(int) args.[2]
//         perfectSquares param1 param2

//     match args.Length with
//         | 3 -> parseParams args |> ignore
        
//         | _ ->  failwith "You need to pass two parameters!"


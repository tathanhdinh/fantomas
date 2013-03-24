﻿module Fantomas.Tests.CodeFormatterExtExtExtTests

open NUnit.Framework
open FsUnit

open Fantomas.FormatConfig
open Fantomas.CodeFormatter

let config = FormatConfig.Default
let newline = System.Environment.NewLine

let inline prepend s content = s + content
let inline append s content = content + s

[<Test>]
let ``type providers``() =
    formatSourceString """
type Northwind = ODataService<"http://services.odata.org/Northwind/Northwind.svc/">""" config
    |> prepend newline
    |> should equal """
type Northwind = ODataService<"http://services.odata.org/Northwind/Northwind.svc/">
"""

[<Test>]
let ``named arguments``() =
    formatSourceString """
type SpeedingTicket() =
    member this.GetMPHOver(speed: int, limit: int) = speed - limit

let CalculateFine (ticket : SpeedingTicket) =
    let delta = ticket.GetMPHOver(limit = 55, speed = 70)
    if delta < 20 then 50.0 else 100.0""" config
    |> prepend newline
    |> should equal """
type SpeedingTicket() = 
    member this.GetMPHOver(speed : int, limit : int) = speed - limit

let CalculateFine(ticket : SpeedingTicket) = 
    let delta = ticket.GetMPHOver(limit = 55, speed = 70)
    if delta < 20
    then 50.0
    else 100.0
"""

[<Test>]
let ``array indices``() =
    formatSourceString """
let array1 = [| 1; 2; 3 |]
array1.[0..2] 
array2.[2.., 0..]
array2.[..3, ..1] 
array1.[1] <- 3
    """ config
    |> prepend newline
    |> should equal """
let array1 = [|1; 2; 3|]

array1.[0..2]
array2.[2.., 0..]
array2.[..3, ..1]
array1.[1] <- 3"""

[<Test>]
let ``array values``() =
    formatSourceString """
let arr = [|(1, 1, 1); (1, 2, 2); (1, 3, 3); (2, 1, 2); (2, 2, 4); (2, 3, 6); (3, 1, 3);
  (3, 2, 6); (3, 3, 9)|]
    """ config
    |> prepend newline
    |> should equal """
let arr = 
    [|(1, 1, 1)
      (1, 2, 2)
      (1, 3, 3)
      (2, 1, 2)
      (2, 2, 4)
      (2, 3, 6)
      (3, 1, 3)
      (3, 2, 6)
      (3, 3, 9)|]
"""

[<Test>]
let ``comments on local let bindings``() =
    formatSourceString """
let print_30_permut() = 

    /// declare and initialize
    let permutation : int array = Array.init n (fun i -> Console.Write(i+1); i)
    permutation
    """ config
    |> prepend newline
    |> should equal """
let print_30_permut() = 
    /// declare and initialize
    let permutation : int array = 
        Array.init n (fun i -> 
            Console.Write(i + 1)
            i)
    permutation
"""

[<Test>]
let ``multiline strings``() =
    formatSourceString """
let alu =
        "GGCCGGGCGCGGTGGCTCACGCCTGTAATCCCAGCACTTTGG\
        GAGGCCGAGGCGGGCGGATCACCTGAGGTCAGGAGTTCGAGA\
        CCAGCCTGGCCAACATGGTGAAACCCCGTCTCTACTAAAAAT\
        ACAAAAATTAGCCGGGCGTGGTGGCGCGCGCCTGTAATCCCA\
        GCTACTCGGGAGGCTGAGGCAGGAGAATCGCTTGAACCCGGG\
        AGGCGGAGGTTGCAGTGAGCCGAGATCGCGCCACTGCACTCC\
  AGCCTGGGCGACAGAGCGAGACTCCGTCTCAAAAA"B
    """ config
    |> prepend newline
    |> should equal """
let alu = "GGCCGGGCGCGGTGGCTCACGCCTGTAATCCCAGCACTTTGG\
        GAGGCCGAGGCGGGCGGATCACCTGAGGTCAGGAGTTCGAGA\
        CCAGCCTGGCCAACATGGTGAAACCCCGTCTCTACTAAAAAT\
        ACAAAAATTAGCCGGGCGTGGTGGCGCGCGCCTGTAATCCCA\
        GCTACTCGGGAGGCTGAGGCAGGAGAATCGCTTGAACCCGGG\
        AGGCGGAGGTTGCAGTGAGCCGAGATCGCGCCACTGCACTCC\
  AGCCTGGGCGACAGAGCGAGACTCCGTCTCAAAAA"B
"""

[<Test>]
let ``indexed property``() =
    formatSourceString """
type NumberStrings() =
   let mutable ordinals = [| "one"; |]
   let mutable cardinals = [| "first"; |]
   member this.Item
      with get index = ordinals.[index]
      and set index value = ordinals.[index] <- value
   member this.Ordinal
      with get(index) = ordinals.[index]
      and set index value = ordinals.[index] <- value
   member this.Cardinal
      with get(index) = cardinals.[index]
      and set index value = cardinals.[index] <- value""" config
    |> prepend newline
    |> should equal """
type NumberStrings() = 
    let mutable ordinals = [|"one"|]
    let mutable cardinals = [|"first"|]
    member this.Item with get index = ordinals.[index]
    member this.Item with set index value = ordinals.[index] <- value
    member this.Ordinal with get (index) = ordinals.[index]
    member this.Ordinal with set index value = ordinals.[index] <- value
    member this.Cardinal with get (index) = cardinals.[index]
    member this.Cardinal with set index value = cardinals.[index] <- value
"""

[<Test>]
let ``complex indexed property``() =
    formatSourceString """
open System.Collections.Generic
type SparseMatrix() =
    let mutable table = new Dictionary<int * int, float>()
    member this.Item
        with get(key1, key2) = table.[(key1, key2)]
        and set (key1, key2) value = table.[(key1, key2)] <- value

let matrix1 = new SparseMatrix()
for i in 1..1000 do
    matrix1.[i, i] <- float i * float i
    """ config
    |> prepend newline
    |> should equal """
open System.Collections.Generic

type SparseMatrix() = 
    let mutable table = new Dictionary<int * int, float>()
    member this.Item with get (key1, key2) = table.[(key1, key2)]
    member this.Item with set (key1, key2) value = table.[(key1, key2)] <- value

let matrix1 = new SparseMatrix()

for i in 1..1000 do
    matrix1.[i, i] <- float i * float i"""
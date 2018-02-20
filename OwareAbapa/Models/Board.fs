module Board

type board = int*int*int * int*int*int * int*int*int * int*int*int
type case = C1 | C2 | C3 | C4 | C5 | C6 | C7 | C8 | C9 | C10 | C11 | C12

let empty : board = (0,0,0, 0,0,0,  0,0,0, 0,0,0)
let initial : board = (4,4,4, 4,4,4,  4,4,4, 4,4,4)

let getCase(board, case) =
    let (c1,c2,c3,c4,c5,c6, c7,c8,c9,c10,c11,c12) = board
    match case with
    | C1 -> c1
    | C2 -> c2
    | C3 -> c3
    | C4 -> c4
    | C5 -> c5
    | C6 -> c6
    | C7 -> c7
    | C8 -> c8
    | C9 -> c9
    | C10 -> c10
    | C11 -> c11
    | C12 -> c12

let map(board, f) =
    let (c1,c2,c3,c4,c5,c6, c7,c8,c9,c10,c11,c12) = board
    (f(C1,c1),f(C2,c2),f(C3,c3),f(C4,c4),f(C5,c5),f(C6,c6), f(C7,c7),f(C8,c8),f(C9,c9),f(C10,c10),f(C11,c11),f(C12,c12))

let emptyCase(board, case) =
    let f(c, _) =
        if c = case
        then 0
        else getCase(board, c)
    map(board, f)

let incrementCase(board, case) =
    let f(c, _) =
        if c = case
        then getCase(board, c) + 1
        else getCase(board, c)
    map(board, f)

let nextCase(case) =
    match case with
    | C1 -> C2
    | C2 -> C3
    | C3 -> C4
    | C4 -> C5
    | C5 -> C6
    | C6 -> C7
    | C7 -> C8
    | C8 -> C9
    | C9 -> C10
    | C10 -> C11
    | C11 -> C12
    | C12 -> C1

let previousCase(case) =
    match case with
    | C1 -> C12
    | C2 -> C1
    | C3 -> C2
    | C4 -> C3
    | C5 -> C4
    | C6 -> C5
    | C7 -> C6
    | C8 -> C7
    | C9 -> C8
    | C10 -> C9
    | C11 -> C10
    | C12 -> C11
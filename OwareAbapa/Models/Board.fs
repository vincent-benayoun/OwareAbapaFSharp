namespace OwareAbapa.Models

module Board =

    [<CLIMutable>]
    type board = { C1:int; C2:int; C3:int; C4:int; C5:int; C6:int;
                   C7:int; C8:int; C9:int; C10:int; C11:int; C12:int }
    type case = C1 | C2 | C3 | C4 | C5 | C6 | C7 | C8 | C9 | C10 | C11 | C12

    let empty : board = {C1=0;C2=0;C3=0; C4=0;C5=0;C6=0;  C7=0;C8=0;C9=0; C10=0;C11=0;C12=0}
    let initial : board = {C1=4;C2=4;C3=4; C4=4;C5=4;C6=4;  C7=4;C8=4;C9=4; C10=4;C11=4;C12=4}

    let allCases = [C1;C2;C3;C4;C5;C6;C7;C8;C9;C10;C11;C12]
    let casesOfPlayer player =
        match player with
        | Player.Player1 -> [C1;C2;C3;C4;C5;C6]
        | Player.Player2 -> [C7;C8;C9;C10;C11;C12]
    let lastCaseOfPlayerIndex player =
        match player with
        | Player.Player1 -> 6
        | Player.Player2 -> 12

    let indexOfCase case =
        match case with
        | C1 -> 1
        | C2 -> 2
        | C3 -> 3
        | C4 -> 4
        | C5 -> 5
        | C6 -> 6
        | C7 -> 7
        | C8 -> 8
        | C9 -> 9
        | C10 -> 10
        | C11 -> 11
        | C12 -> 12

    let getCase board case =
        match case with
        | C1 -> board.C1
        | C2 -> board.C2
        | C3 -> board.C3
        | C4 -> board.C4
        | C5 -> board.C5
        | C6 -> board.C6
        | C7 -> board.C7
        | C8 -> board.C8
        | C9 -> board.C9
        | C10 -> board.C10
        | C11 -> board.C11
        | C12 -> board.C12

    let map(board: board, f) =
        { C1 = f(C1,board.C1);
          C2 = f(C2,board.C2);
          C3 = f(C3,board.C3);
          C4 = f(C4,board.C4);
          C5 = f(C5,board.C5);
          C6 = f(C6,board.C6);
          C7 = f(C7,board.C7);
          C8 = f(C8,board.C8);
          C9 = f(C9,board.C9);
          C10 = f(C10,board.C10);
          C11 = f(C11,board.C11);
          C12 = f(C12,board.C12);}

    let emptyCase(board, case) =
        let f(c, _) =
            if c = case
            then 0
            else getCase board  c 
        map(board, f)

    let incrementCase(board, case) =
        let f(c, _) =
            if c = case
            then getCase board c + 1
            else getCase board c
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
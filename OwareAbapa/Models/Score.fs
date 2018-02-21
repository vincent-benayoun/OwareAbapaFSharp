namespace OwareAbapa.Models

module Score =

    open Player
    
    [<CLIMutable>]
    type score = { Player1: int; Player2: int }

    let initial = { Player1 = 0; Player2 = 0 }

    let addPoints(score, player, numberOfPoints) =
        match player with
        | Player1 -> { score with Player1 = score.Player1 + numberOfPoints }
        | Player2 -> { score with Player2 = score.Player2 + numberOfPoints }

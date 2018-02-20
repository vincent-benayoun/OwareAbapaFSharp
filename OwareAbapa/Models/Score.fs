namespace OwareAbapa.Models

module Score =

    open Player

    type score = { firstPlayer: int; secondPlayer: int }

    let initial = { firstPlayer = 0; secondPlayer = 0 }

    let addPoints(score, player, numberOfPoints) =
        match player with
        | Player1 -> { score with firstPlayer = score.firstPlayer + numberOfPoints }
        | Player2 -> { score with secondPlayer = score.secondPlayer + numberOfPoints }

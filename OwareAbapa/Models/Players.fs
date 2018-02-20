namespace OwareAbapa.Models

module Players =

    type player = Player1 | Player2

    let nextPlayer =
        function Player1 -> Player2
               | Player2 -> Player1

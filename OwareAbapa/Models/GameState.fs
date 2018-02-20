namespace OwareAbapa.Models

module GameState =

    type game_state = { currentPlayer: Player.player;
                        board: Board.board;
                        score: Score.score; }

    let initial = { currentPlayer = Player.Player1;
                    board = Board.initial;
                    score = Score.initial }

    type winner = Winner of Player.player | Exaequo
    type win_status = StillPlaying | GameEnded of winner

    let getWinStatus(gameState) =
        let firstPlayerWon = gameState.score.firstPlayer > 24
        let secondPlayerWon = gameState.score.secondPlayer > 24
        let exaequo =
            gameState.score.secondPlayer = 24
            && gameState.score.secondPlayer = 24

        if firstPlayerWon then
            GameEnded(Winner(Player.Player1))
        else if secondPlayerWon then
            GameEnded(Winner(Player.Player2))
        else if exaequo then
            GameEnded(Exaequo)
        else
            StillPlaying




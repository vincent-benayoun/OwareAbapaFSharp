namespace OwareAbapa.DTO

open OwareAbapa.Models

module GameStateDTO =
    [<CLIMutable>]
    type game_state =
        { board: Board.board;
          currentPlayer: string;
          score: Score.score;
          nbSteps: int;
          winStatus: string; }

    module Encode =
        let player player =
            match player with | Player.Player1 -> "Player1" | Player.Player2 -> "Player2"
            
        let winner winner =
            match winner with | GameState.Winner(winPlayer) -> sprintf "%s won!" (player winPlayer) | GameState.Exaequo -> "Exaequo"

        let winStatus winStatus =
            match winStatus with | GameState.StillPlaying -> "" | GameState.GameEnded(gameWinner) -> winner gameWinner

        let gameState (gameState:GameState.game_state) =
            { board = gameState.board;
              currentPlayer = player gameState.currentPlayer;
              score = gameState.score;
              nbSteps = gameState.nbSteps;
              winStatus = winStatus (GameState.getWinStatus gameState); }


    module Decode =
        let player player =
            match player with | "Player1" -> Player.Player1 | "Player2" -> Player.Player2 | _ -> failwith "invalid decoding"

        let gameState gameState =
            { GameState.game_state.board = gameState.board;
              GameState.game_state.currentPlayer = player gameState.currentPlayer;
              GameState.game_state.nbSteps = gameState.nbSteps;
              GameState.game_state.score = gameState.score }

        let case case =
            match case with
            | "C1" -> Board.C1
            | "C2" -> Board.C2
            | "C3" -> Board.C3
            | "C4" -> Board.C4
            | "C5" -> Board.C5
            | "C6" -> Board.C6
            | "C7" -> Board.C7
            | "C8" -> Board.C8
            | "C9" -> Board.C9
            | "C10" -> Board.C10
            | "C11" -> Board.C11
            | "C12" -> Board.C12
            | _ -> failwith "invalid decoding"
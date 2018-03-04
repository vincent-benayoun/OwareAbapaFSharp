namespace OwareAbapa.Models

module GameAIManager =
    let availableStrategies =
        ["dummy", GameAIDummy.chooseCaseToPlay;
         "one", GameAIDummy.chooseCaseToPlay] |> Map.ofList

    let playContest strategy1 strategy2 =
        let gameState = GameState.initial
        let rec playStep (gameState:GameState.game_state) =
            if gameState.nbSteps > 500 then GameState.Exaequo else
            match GameState.getWinStatus gameState with
            | GameState.GameEnded winner -> winner
            | GameState.StillPlaying ->
                let strategy =
                    match gameState.currentPlayer with
                    | Player.Player1 -> strategy1
                    | Player.Player2 -> strategy2
                let case = strategy gameState
                match GameLogic.play gameState case with
                | GameLogic.InvalidCase
                | GameLogic.EmptyCase
                | GameLogic.NeedsToFeed
                | GameLogic.InternalError -> GameState.Exaequo
                | GameLogic.Played(newGameState) -> playStep newGameState
        playStep gameState

    let playContestByStrategyNames strategyName1 strategyName2 =
        match availableStrategies.TryFind strategyName1, availableStrategies.TryFind strategyName2 with
        | (Some strategy1, Some strategy2) -> Some (playContest strategy1 strategy2)
        | _ -> None


namespace OwareAbapa.Models

open GameState

module GameAIMeir =

    let gameStateAfterPlaying gameState case =
        match GameLogic.play gameState case with
        | GameLogic.Played (gameStateAfter) -> [(case, gameStateAfter)]
        | GameLogic.InvalidCase
        | GameLogic.EmptyCase
        | GameLogic.NeedsToFeed
        | GameLogic.InternalError -> []

    let nextGameStates gameState =
        let casesOfPlayer = Board.casesOfPlayer gameState.currentPlayer
        let nextList = List.collect (gameStateAfterPlaying gameState) casesOfPlayer
        let nb = List.length nextList = 0
        nextList

    let chooseCaseToPlay gameState =
        let nextGameStatesList = nextGameStates gameState
        let random = new System.Random()
        let randomNextGameStateIndex = random.Next(List.length nextGameStatesList)
        let caseToPlay = fst (List.nth nextGameStatesList randomNextGameStateIndex)
        caseToPlay

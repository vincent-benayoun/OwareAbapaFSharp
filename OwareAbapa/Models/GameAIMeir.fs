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
        List.collect (gameStateAfterPlaying gameState) casesOfPlayer

    let progressRate gameStateBefore gameStateAfter player =
        let floatScoreBefore = float (Score.getScore gameStateBefore.score player)
        if floatScoreBefore = 25. then 0. else
        let floatScoreAfter = float (Score.getScore gameStateAfter.score player)
        (floatScoreAfter - floatScoreBefore) / (25. - floatScoreBefore)

    let interestToPlay gameStateBefore gameStateAfter player =
        let adversaryPlayer = Player.nextPlayer player
        progressRate gameStateBefore gameStateAfter player - progressRate gameStateBefore gameStateAfter adversaryPlayer

    type game_tree = GameTree of GameState.game_state * ((Board.case * game_tree) list)
    
    let buildGameTree depth gameState =
        let rec buildGameTree depth (case, gameState) =
            let _, subTrees =
                if depth < 0
                then case, []
                else
                    let nextGameStatesList = nextGameStates gameState
                    case, List.map (buildGameTree (depth - 1)) nextGameStatesList
            case, GameTree(gameState, subTrees)
        let _, gameTree = buildGameTree depth (Board.C1, gameState)
        gameTree
        
    let rec longTermInterest rootGameState gameTree =
        match gameTree with
        | GameTree(nextGameState, furtherGameStates) ->
            match furtherGameStates with
            | [] -> interestToPlay rootGameState nextGameState (Player.nextPlayer nextGameState.currentPlayer)
            | _ -> let subTreesInterests = List.map (snd >> longTermInterest rootGameState) furtherGameStates
                   - List.max subTreesInterests
        
    type game_tree_with_interest = { gameTree: game_tree; playedCase: Board.case; interest: float }

    let chooseCaseToPlay depth gameState =
        let gameTree = buildGameTree depth gameState
        match gameTree with
        | GameTree(gameState, subTrees) ->
            let subTreesWithInterest = List.map (fun (case,subTree) -> { gameTree = subTree; playedCase = case; interest = longTermInterest gameState subTree }) subTrees
            let sortedSubTreesWithInterest = List.sortBy (fun subTreeWithInterest -> -subTreeWithInterest.interest) subTreesWithInterest
            sortedSubTreesWithInterest.Head.playedCase

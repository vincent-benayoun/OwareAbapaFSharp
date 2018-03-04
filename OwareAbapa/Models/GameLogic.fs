namespace OwareAbapa.Models

module GameLogic =

    type distribute_result = InvalidArgument | Distrubuted of Board.board * Board.case
    
    let distributeFrom(board, originCase, currentCase, numberOfSeeds): Board.board * Board.case =
        let rec distributeFrom(board, currentCase, numberOfSeeds): Board.board * Board.case =
            if numberOfSeeds = 0 then
                (board, Board.previousCase currentCase)
            else
                let nextCase = Board.nextCase currentCase
                if currentCase = originCase then
                    distributeFrom(board, nextCase, numberOfSeeds)
                else
                    let newBoard = Board.incrementCase(board, currentCase)
                    distributeFrom(newBoard, nextCase, numberOfSeeds-1)
        distributeFrom(board, currentCase, numberOfSeeds)

    let distribute(board, originCase, numberOfSeeds) : distribute_result =
        if numberOfSeeds <= 0 then
            InvalidArgument
        else
            let currentCase = Board.nextCase originCase
            let (newBoard, lastCase) = distributeFrom(board, originCase, currentCase, numberOfSeeds)
            Distrubuted(newBoard, lastCase)

    open Player
    open Board

    let belongsTo(case, player) =
        match player with
        | Player1 -> (match case with | C1|C2|C3|C4|C5|C6 -> true | _ -> false)
        | Player2 -> (match case with | C7|C8|C9|C10|C11|C12 -> true | _ -> false)

    open GameState
    
    let hasNoSeeds board player =
        let casesOfPlayer = Board.casesOfPlayer player
        List.forall (fun case -> Board.getCase board case = 0) casesOfPlayer

    let rec capture(gameState, case) =
        let nextPlayer = Player.nextPlayer(gameState.currentPlayer)
        if not (belongsTo(case, nextPlayer)) then gameState else

        let numberOfSeeds = Board.getCase gameState.board case
        if numberOfSeeds <> 2 && numberOfSeeds <> 3 then gameState else

        let newGameState = {gameState with
                                board = Board.emptyCase(gameState.board, case);
                                score = Score.addPoints(gameState.score, gameState.currentPlayer, numberOfSeeds) }
        capture(newGameState, Board.previousCase case)

    let captureAll(gameState) = 
        let plus = (+)
        let getCase = Board.getCase gameState.board
        let numberOfSeeds = List.reduce (+) (List.map getCase Board.allCases)
        {gameState with board = Board.empty;
                        score = Score.addPoints(gameState.score, gameState.currentPlayer, numberOfSeeds) }

    let nothingFeeds gameState =
        let casesOfPlayer = Board.casesOfPlayer gameState.currentPlayer
        let lastCaseOfPlayerIndex = Board.lastCaseOfPlayerIndex gameState.currentPlayer
        let getCase = Board.getCase gameState.board
        List.forall (fun case -> Board.indexOfCase case + getCase case <= lastCaseOfPlayerIndex ) casesOfPlayer

    let seeds board originCase =
        let boardWithEmptyCase = Board.emptyCase(board, originCase)
        let numberOfSeeds = Board.getCase board originCase
        distribute(boardWithEmptyCase, originCase, numberOfSeeds)
        

    type play_result = InvalidCase | EmptyCase | NeedsToFeed | InternalError | Played of game_state

    // Current player plays case
    let play gameState case =
        let casesOfPlayer = Board.casesOfPlayer gameState.currentPlayer
        if not (List.exists ((=) case) casesOfPlayer) then InvalidCase else
        if Board.getCase gameState.board case = 0 then EmptyCase else

        let nextPlayer = Player.nextPlayer gameState.currentPlayer
        let adversaryHadNoSeed = hasNoSeeds gameState.board nextPlayer
        let nothingFeedsAdversary = nothingFeeds gameState

        if adversaryHadNoSeed && nothingFeedsAdversary
        then
            Played { captureAll gameState with currentPlayer = Player.nextPlayer gameState.currentPlayer }
        else
            match seeds gameState.board case with
            | InvalidArgument -> InternalError
            | Distrubuted(boardAfterSeeding, lastCase) ->
                let adversaryHadNoSeedAfterSeeding = hasNoSeeds boardAfterSeeding nextPlayer
                let gameStateAfterSeeding = { gameState with board = boardAfterSeeding; nbSteps = gameState.nbSteps + 1 }
                let gameStateAfterCapture = capture(gameStateAfterSeeding, lastCase)

                if not(hasNoSeeds gameStateAfterCapture.board nextPlayer) then 
                    Played { gameStateAfterCapture with currentPlayer = nextPlayer }
                else
                    if adversaryHadNoSeedAfterSeeding then NeedsToFeed else
                    Played { gameStateAfterSeeding with currentPlayer = nextPlayer }


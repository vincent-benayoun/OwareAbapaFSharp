namespace OwareAbapa.Controllers
open System
open System.Collections.Generic
open System.Linq
open System.Net.Http
open System.Web.Http
open OwareAbapa.Models
open OwareAbapa.DTO

          
// a player can win without playing ... but who cares?
[<CLIMutable>]
type play_argument = { gameState: GameStateDTO.game_state; caseId: string }
[<CLIMutable>]
type playAI_argument = { gameState: GameStateDTO.game_state; strategy: string }
[<CLIMutable>]
type playAIBenchmark_argument = { strategy1: string; strategy2: string }
[<CLIMutable>]
type playAIBenchmark_result = { nbTimesPlayer1Won: int; nbTimesExaequo: int; nbTimesPlayer2Won: int }
//[<CLIMutable>]
type play_result = { status: string; gameState: GameStateDTO.game_state }

/// Retrieves values.
type GameController() =
    inherit ApiController()

    /// Gets all values.
    member x.Get() = GameState.initial

    member x.Get(id:string) =
        match id with
        | "newGame" -> GameState.initial
        | _ -> GameState.initial

        
    [<HttpGet>]
    member x.NewGame() = GameStateDTO.Encode.gameState GameState.initial

    [<HttpPost>]
    member x.Play(data:play_argument) =
        let gameState = GameStateDTO.Decode.gameState data.gameState
        let case = GameStateDTO.Decode.case data.caseId
        match GameLogic.play gameState case with
        | GameLogic.InvalidCase     -> { status = "InvalidCase";   gameState = data.gameState }
        | GameLogic.EmptyCase       -> { status = "EmptyCase";     gameState = data.gameState }
        | GameLogic.NeedsToFeed     -> { status = "NeedsToFeed";   gameState = data.gameState }
        | GameLogic.InternalError   -> { status = "InternalError"; gameState = data.gameState }
        | GameLogic.Played(game_state) ->
            { status = "OK"; gameState = GameStateDTO.Encode.gameState game_state }
            
    [<HttpPost>]
    member x.PlayAI(data:playAI_argument) =
        let gameState = GameStateDTO.Decode.gameState data.gameState
        let strategyName = data.strategy
        match GameAIManager.availableStrategies.TryFind strategyName with
        | None -> { status = "UnknownAIStrategy";   gameState = data.gameState }
        | Some chooseCaseToPlay ->
            let case = chooseCaseToPlay gameState
            match GameLogic.play gameState case with
            | GameLogic.InvalidCase     -> { status = "InvalidCase";   gameState = data.gameState }
            | GameLogic.EmptyCase       -> { status = "EmptyCase";     gameState = data.gameState }
            | GameLogic.NeedsToFeed     -> { status = "NeedsToFeed";   gameState = data.gameState }
            | GameLogic.InternalError   -> { status = "InternalError"; gameState = data.gameState }
            | GameLogic.Played(game_state) ->
                { status = "OK"; gameState = GameStateDTO.Encode.gameState game_state }
            
    [<HttpGet>]
    member x.GetAIStrategies() = GameAIManager.availableStrategies

    [<HttpPost>]
    member x.BenchmarkTwoStrategies(data:playAIBenchmark_argument) =
        let strategyName1 = data.strategy1
        let strategyName2 = data.strategy2
        let rec benchmark strategyName1 strategyName2 nbTimes acc =
            if nbTimes <= 0 then acc else
            let acc =
                match GameAIManager.playContestByStrategyNames strategyName1 strategyName2 with
                | None -> acc
                | Some (GameState.Winner(Player.Player1)) -> {acc with nbTimesPlayer1Won = acc.nbTimesPlayer1Won + 1}
                | Some (GameState.Winner(Player.Player2)) -> {acc with nbTimesPlayer2Won = acc.nbTimesPlayer2Won + 1}
                | Some (GameState.Exaequo)                -> {acc with nbTimesExaequo = acc.nbTimesExaequo + 1}
            benchmark strategyName1 strategyName2 (nbTimes - 1) acc
        let resultPlayer1First = benchmark strategyName1 strategyName2 500 {nbTimesPlayer1Won = 0; nbTimesExaequo = 0; nbTimesPlayer2Won = 0}
        let resultPlayer2First = benchmark strategyName2 strategyName1 500 {nbTimesPlayer1Won = 0; nbTimesExaequo = 0; nbTimesPlayer2Won = 0}
        { nbTimesPlayer1Won = resultPlayer1First.nbTimesPlayer1Won + resultPlayer2First.nbTimesPlayer2Won;
          nbTimesExaequo = resultPlayer1First.nbTimesExaequo + resultPlayer2First.nbTimesExaequo;
          nbTimesPlayer2Won = resultPlayer1First.nbTimesPlayer2Won + resultPlayer2First.nbTimesPlayer1Won; }



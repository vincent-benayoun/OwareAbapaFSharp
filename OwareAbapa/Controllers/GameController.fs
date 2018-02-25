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
type playAI_argument = { gameState: GameStateDTO.game_state }
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
        let case = GameAIDummy.chooseCaseToPlay gameState
        match GameLogic.play gameState case with
        | GameLogic.InvalidCase     -> { status = "InvalidCase";   gameState = data.gameState }
        | GameLogic.EmptyCase       -> { status = "EmptyCase";     gameState = data.gameState }
        | GameLogic.NeedsToFeed     -> { status = "NeedsToFeed";   gameState = data.gameState }
        | GameLogic.InternalError   -> { status = "InternalError"; gameState = data.gameState }
        | GameLogic.Played(game_state) ->
            { status = "OK"; gameState = GameStateDTO.Encode.gameState game_state }
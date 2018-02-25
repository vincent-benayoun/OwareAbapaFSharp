namespace OwareAbapa.Models

module GameLogic =

    type distribute_result = InvalidArgument | Distrubuted of Board.board * Board.case
    
    type play_result = InvalidCase | EmptyCase | NeedsToFeed | InternalError | Played of GameState.game_state
    val play: GameState.game_state -> Board.case -> play_result
    
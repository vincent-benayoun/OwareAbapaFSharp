﻿namespace OwareAbapa.Controllers
open System.Web.Http
open OwareAbapa.Models

type GameController() =
    inherit ApiController()

    let values = [| Board.initial |]

    /// Gets all values.
    member x.Get() = values

namespace OwareAbapa.Models

module GameAIManager =
    let availableStrategies =
        ["dummy", GameAIDummy.chooseCaseToPlay;
         "one", GameAIDummy.chooseCaseToPlay] |> Map.ofList



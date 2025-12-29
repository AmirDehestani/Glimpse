namespace Processor

open System.Text.Json
open System.IO

module Serializer =

    let private path = "quotes.json" // TODO: Improve how json file path is set and how the file is created if it doesn't exist

    let private fileLock = obj ()

    let private serialize value = JsonSerializer.Serialize value

    let private deserialize (json: string) =
        JsonSerializer.Deserialize<ProcessedQuote list> json

    let private writeToFile jsonStr = File.WriteAllText(path, jsonStr)

    let private readFile () = File.ReadAllText path

    let updateQuoteOnFile incoming =
        lock fileLock (fun () ->
            let existing =
                let json = readFile ()
                deserialize json

            let isUnique value = value.Symbol <> incoming.Symbol
            let filteredQuotes = existing |> List.filter isUnique
            let updatedQuotes = incoming :: filteredQuotes
            writeToFile <| serialize updatedQuotes)

namespace Processor

open System.Text.Json.Serialization

type Currency =
    | USD
    | CAD

type RawQuote =
    { Code: string
      Close: decimal
      Change_p: decimal
      Currency: Currency option }

[<JsonFSharpConverter>]
type ProcessedQuote =
    { Symbol: string
      Price: decimal
      PercentageChange: decimal
      UpdatedAt: System.DateTime }

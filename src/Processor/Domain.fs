namespace Processor

type Currency =
    | USD
    | CAD

type RawQuote =
    { Code: string
      Close: decimal
      Change_p: decimal
      Currency: Currency option }

type ProcessedQuote =
    { Symbol: string
      Price: decimal
      PercentageChange: decimal
      UpdatedAt: System.DateTime }

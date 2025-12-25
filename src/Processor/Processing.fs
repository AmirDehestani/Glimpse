namespace Processor

module QuoteProcessor =

    // TODO: get real exchange rate dynamically
    let exchangeRate = 1.3m

    let getPriceCad (price: decimal) (currency: Currency) =
        match currency with
        | USD -> exchangeRate * price
        | CAD -> price

    let processQuote (quote: RawQuote) =
        let currency = quote.Currency |> Option.defaultValue CAD
        let price = getPriceCad quote.Close currency
        let updatedAt = System.DateTime.Now

        let processedQuote: ProcessedQuote =
            { Symbol = quote.Code
              Price = price
              PercentageChange = quote.Change_p
              UpdatedAt = updatedAt }

        processedQuote

namespace Processor


module QuoteProcessor =

    // TODO: get real exchange rate dynamically
    let exchangeRate = 1.3m

    let getPriceCad (price: decimal) (currency: Currency) =
        match currency with
        | USD -> exchangeRate * price
        | CAD -> price

    let processQuote (quote: RawQuote) : ProcessedQuote =
        let price =
            let currency = quote.Currency |> Option.defaultValue CAD
            getPriceCad quote.Close currency

        let processedQuote =
            { Symbol = quote.Code
              Price = price
              PercentageChange = quote.Change_p
              UpdatedAt = System.DateTime.Now }

        Serializer.updateQuoteOnFile processedQuote
        processedQuote

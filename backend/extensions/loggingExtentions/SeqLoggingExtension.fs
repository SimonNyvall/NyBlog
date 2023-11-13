module Backend.Extensions.Logging.SeqLoggingExtension

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection

let setupSeqLogging (builder: WebApplicationBuilder): WebApplicationBuilder =
      
  let seqUrl = builder.Configuration["SeqUrl"]

  builder.Services.AddLogging(fun loggingBuilder -> 
    loggingBuilder.AddSeq seqUrl |> ignore
  ) |> ignore

  builder

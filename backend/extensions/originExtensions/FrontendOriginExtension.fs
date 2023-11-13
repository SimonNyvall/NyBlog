module Backend.Extensions.Origin.FrontendOriginExtension 

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection

let useFrontendOrigin (builder: WebApplicationBuilder): WebApplicationBuilder =
  
  builder.Services.AddCors(fun options -> 
    let frontendOrigin = builder.Configuration["AllowedOrigins:Frontend"]

    options.AddPolicy("AllowFrontend", fun builder -> 
      builder.WithOrigins(
        frontendOrigin
        )
        .AllowAnyHeader() |> ignore
      )
    ) |> ignore

  builder


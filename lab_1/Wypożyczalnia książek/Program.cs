using Microsoft.AspNetCore.Mvc;
using TI_Lab01_Library.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = ctx =>
        {
            var problem = new ValidationProblemDetails(ctx.ModelState)
            {
                Status = StatusCodes.Status422UnprocessableEntity,
                Title = "Validation error"
            };
            return new UnprocessableEntityObjectResult(problem);
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<Db>();

var app = builder.Build();

app.Use(async (ctx, next) =>
{
    var start = DateTime.UtcNow;
    await next();
    var ms = (DateTime.UtcNow - start).TotalMilliseconds;
    app.Logger.LogInformation("{Method} {Path} -> {StatusCode} ({Ms:0} ms)",
        ctx.Request.Method, ctx.Request.Path, ctx.Response.StatusCode, ms);
});

app.Use(async (ctx, next) =>
{
    ctx.Response.Headers["X-Content-Type-Options"] = "nosniff";
    ctx.Response.Headers["Referrer-Policy"] = "no-referrer";

   
    var csp = "default-src 'self'; " +
              "script-src 'self' 'unsafe-inline'; " +
              "style-src 'self' 'unsafe-inline'; " +
              "img-src 'self';";


    if (app.Environment.IsDevelopment())
        csp += " connect-src 'self' https: wss: ws:;";

    ctx.Response.Headers["Content-Security-Policy"] = csp;

    await next();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

app.Run();

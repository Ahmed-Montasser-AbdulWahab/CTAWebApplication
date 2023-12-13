using HrWebApplication.Model;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using System.Runtime.CompilerServices;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

List<Bus> buses = new List<Bus>() {
    new Bus(){Line = 1083, Starting="Obour", Ending="Tahrir"},
    new Bus(){Line = 1056, Starting="Shorouq", Ending="Tahrir"}
};

app.Run(async ctx =>
{
    var msg = "";
    //Get Request : Retrieve Information
    if(ctx.Request.Method == "GET") {
        if (ctx.Request.Query.Count < 1 || !ctx.Request.Query.ContainsKey("line")) {
            ctx.Response.StatusCode = 400;
            msg = "Not illegal request";
            goto end;
        }

        var line = ctx.Request.Query["line"][0];

        foreach (var bus in buses)
        {
            if (bus.Line.ToString() == line)
            {
                msg = $"{bus.Line} : {bus.Starting} \\ {bus.Ending}";
                goto end; 
            }
        }

        ctx.Response.StatusCode = 404;
        msg = $"Bus {line} not found";
    }
    //Post Request : Sending Information
    else if(ctx.Request.Method == "POST")
    {
        //Reading Body By Stream
        var sr = new StreamReader(ctx.Request.Body);
        var input = await sr.ReadToEndAsync();

        //Convert to Key Value
        var keyValue = QueryHelpers.ParseQuery(input);

        if (!(keyValue.ContainsKey("line") && keyValue.ContainsKey("Starting") && keyValue.ContainsKey("Ending")))
        {
            ctx.Response.StatusCode = 400;
            msg = "Not illegal request";
            goto end;
        }

        buses.Add(new Bus() { Line = int.Parse(keyValue["line"][0]), Starting = keyValue["starting"][0], Ending = keyValue["Ending"][0] });
        msg = $"Saving bus {keyValue["line"][0]} is successful";
    }
    else
    {
        ctx.Response.StatusCode = 400;
        msg = "Not illegal request" ;
    }

end:
    await ctx.Response.WriteAsync(msg);
    await ctx.Response.WriteAsync("\nTHANK YOU");



});

app.Run();

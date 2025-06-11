using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;
using System.Collections.Generic;
using Solution.Ticket.models;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {
        options.RouteTemplate = "/openapi/{documentName}.json";
    });

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/openapi/v1.json", "API V1");
        c.RoutePrefix = "swagger";
    });

    app.MapScalarApiReference();
}



var users = new List<UserModel>
{
    new UserModel { Id = 1 , Name = "John", Email = "Johndoe@domain.be" },
    new UserModel { Id = 2, Name = "Hector", Email = "Hector@domain.be" }
};

app.MapGet("/users", () => users);

app.MapPost("/users", (UserModel user) =>
{
    user.Id = users.Any() ? users.Max(u => u.Id) + 1 : 1;
    users.Add(user);
    return Results.Created($"/users/{user.Id}", user);
});

var tickets = new List<Ticket>
{
    new Ticket { Id = 1, Create_at = DateTime.UtcNow.AddDays(-5), Title = "Connexion Wi-Fi impossible", Description = "Impossible de se connecter au Wi-Fi de l'entreprise", Status = "Open" },
    new Ticket { Id = 2, Create_at = DateTime.UtcNow.AddDays(-3), Title = "Imprimante hors service", Description = "L'imprimante du 3ème étage ne répond plus", Status = "InProgress" },
    new Ticket { Id = 3, Create_at = DateTime.UtcNow.AddDays(-2), Title = "Erreur 500 sur l'intranet", Description = "Le site interne renvoie une erreur 500", Status = "Resolved" },
    new Ticket { Id = 4, Create_at = DateTime.UtcNow.AddDays(-1), Title = "Ordinateur trop lent", Description = "Le PC de Sophie met 10 minutes à démarrer", Status = "Open" },
    new Ticket { Id = 5, Create_at = DateTime.UtcNow, Title = "Problème de mot de passe", Description = "Jean a oublié son mot de passe Windows", Status = "Open" }
};


app.MapGet("/tickets", () => tickets);

app.MapPost("/tickets", (Ticket ticket) =>
{
        ticket.Id = tickets.Any() ? tickets.Max(u => u.Id) + 1 : 1;
        tickets.Add(ticket);
        return Results.Created($"/tickets/{ticket.Id}", ticket);
});

app.MapGet("/tickets/status={Open}", (string status) => tickets.Where(t => t.Status == status));

app.MapDelete("/tickets/{id}", (int id) => tickets.RemoveAll(t => t.Id == id));


app.MapPost("/users/{id}/tickets", (int id, Ticket ticket) => tickets.Add(ticket));
//app.MapPut("/tickets/{id}", (int id, Ticket ticket) => tickets.FirstOrDefault(t => t.Id == id) = ticket);

// app.MapGet("users/{id}/tickets}"  );


app.Run();








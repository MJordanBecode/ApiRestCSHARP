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

app.MapGet("/users", () =>
{
    if (users.Any())
    {
        return Results.Ok(users);
    }
    else
    {
        return Results.BadRequest("Users not found.");
    }
    
});

app.MapPost("/users", (UserModel user) =>
{
    user.Id = users.Any() ? users.Max(u => u.Id) + 1 : 1;
    users.Add(user);
    if (users.Contains(user))
    {
        return Results.Created($"/users/{user.Id}", user);
    }
    return Results.BadRequest("User could not be added.");
});


var tickets = new List<Ticket>
{
    new Ticket { Id = Guid.Parse("d54f3b5e-6e56-4d8b-a42c-f33cf2785f4d"), Create_at = DateTime.UtcNow.AddDays(-5), Title = "Connexion Wi-Fi impossible", Description = "Impossible de se connecter au Wi-Fi de l'entreprise", Status = "Open", UserId = 1 },
    new Ticket { Id = Guid.Parse("29fbb67a-fc25-4bd7-8c2f-21f1b4a902f9"), Create_at = DateTime.UtcNow.AddDays(-3), Title = "Imprimante hors service", Description = "L'imprimante du 3ème étage ne répond plus", Status = "InProgress" },
    new Ticket { Id = Guid.Parse("a7a73cf6-ea8a-4a98-9007-56ec4fe88f85"), Create_at = DateTime.UtcNow.AddDays(-2), Title = "Erreur 500 sur l'intranet", Description = "Le site interne renvoie une erreur 500", Status = "Resolved" },
    new Ticket { Id = Guid.Parse("c3b9fc6e-8f78-4e50-a1c5-2f1d3ce4e49d"), Create_at = DateTime.UtcNow.AddDays(-1), Title = "Ordinateur trop lent", Description = "Le PC de Sophie met 10 minutes à démarrer", Status = "Open", UserId = 1 },
    new Ticket { Id = Guid.Parse("703abb4b-709d-4a91-bfa6-dc8a79bcf6e4"), Create_at = DateTime.UtcNow, Title = "Problème de mot de passe", Description = "Jean a oublié son mot de passe Windows", Status = "Open", UserId = 1}
};


app.MapGet("/tickets", () =>
{
    if (tickets.Any())
    {
            return Results.Ok(tickets) ; //201
    }
    return Results.BadRequest("Tickets not found."); //404
});

app.MapPost("/tickets", (Ticket ticket) =>
{
    ticket.Id = Guid.NewGuid();  // Génère un UUID
    tickets.Add(ticket);
    return Results.Created($"/tickets/{ticket.Id}", ticket);
});


app.MapGet("/tickets/status={status}", (string status) =>
{
    var filtered = tickets.Where(t => t.Status == status).ToList();
    if (!filtered.Any())
        return Results.BadRequest($"No tickets found with status '{status}'");

    return Results.Ok(filtered);
});


// app.MapDelete("/tickets/{id}", (int id) => tickets.RemoveAll(t => t.Id == id) Result.Ok);
app.MapDelete("/tickets/{id}", (Guid id) =>
{
    var removedCount = tickets.RemoveAll(t => t.Id == id);
    return removedCount > 0
        ? Results.Ok($"Le ticket avec l'ID {id} a été supprimé.")
        : Results.NotFound($"Aucun ticket trouvé avec l'ID {id}.");
});


 

app.MapPut("/tickets/{id}", (Guid id, Ticket updatedTicket) =>
{
    var ticket = tickets.FirstOrDefault(t => t.Id == id);
    if (ticket is null)
    {
        return Results.NotFound($"Le ticket avec l'id {id} n'existe pas.");
    }

    // Mettre à jour les propriétés
    ticket.Status = updatedTicket.Status;
    ticket.Description = updatedTicket.Description;

    return Results.Ok(ticket);
});


app.MapGet("users/{id}/tickets", (int id) => tickets.Where(t => t.UserId == id) );
app.MapDelete("users/{id}/tickets", (int id) => tickets.RemoveAll(t => t.UserId == id));
app.MapDelete("users/{id}/tickets/{ticketId}", (int id, Guid ticketId) => tickets.RemoveAll(t => t.UserId == id && t.Id == ticketId));

app.Run();








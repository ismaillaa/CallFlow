using Api.Hubs;
using Application.UseCases;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Api.Controllers;

public record EnregistrerAppelRequest(
    int ProspectId, int AgentId, ResultatAppel Resultat,
    int DureeSecondes, string? Commentaire, DateTime? DateRappel);

[Authorize]
[ApiController]
[Route("api/appels")]
public class AppelsController(EnregistrerAppelService service, IHubContext<DashboardHub> hub) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Enregistrer(EnregistrerAppelRequest req)
    {
        var resultat = await service.EnregistrerAsync(
            req.ProspectId, req.AgentId, req.Resultat, req.DureeSecondes, req.Commentaire, req.DateRappel);
        if(resultat == ResultatEnregistrement.Succes)
        {
            await hub.Clients.All.SendAsync("NouvelAppel", new
            {
                prospectId = req.ProspectId,
                agentId = req.AgentId,
                resultat = req.Resultat.ToString(),
                date = DateTime.UtcNow
            });
        }

        return resultat switch
        {
            ResultatEnregistrement.Succes => Created("", null),
            ResultatEnregistrement.ProspectIntrouvable => NotFound("Prospect introuvable"),
            ResultatEnregistrement.AgentIntrouvable => NotFound("Agent introuvable"),
            ResultatEnregistrement.DonneesInvalides => BadRequest("Donnees invalides"),
            ResultatEnregistrement.Conflit => Conflict("Conflit de concurrence"),
            _ => StatusCode(500)
        };
    }
}


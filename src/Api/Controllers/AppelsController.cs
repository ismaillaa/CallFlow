using Application.UseCases;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public record EnregistrerAppelRequest(
    int ProspectId, int AgentId, ResultatAppel Resultat,
    int DureeSecondes, string? Commentaire, DateTime? DateRappel);

[Authorize]
[ApiController]
[Route("api/appels")]
public class AppelsController(EnregistrerAppelService service) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Enregistrer(EnregistrerAppelRequest req)
    {
        var ok = await service.EnregistrerAsync(
            req.ProspectId, req.AgentId, req.Resultat, req.DureeSecondes, req.Commentaire, req.DateRappel);

        return ok ? Created("", null) : Conflict("Conflit ou prospect introuvable");
    }
}


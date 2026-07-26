using Application.UseCases;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public record EnregisterAppelRequest(
    int ProspectId, int AgnetId, ResultatAppel Resultat,
    int DureeSecondes, string? Commentaire);

[ApiController]
[Route("api/appels")]
public class AppelsController(EnregistrerAppelService service) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Enregistrer(EnregisterAppelRequest req)
    {
        var ok = await service.EnregisterAsync(
            req.ProspectId, req.AgnetId, req.Resultat, req.DureeSecondes, req.Commentaire);

        return ok ? Created("", null) : Conflict("Conflit ou prospect introuvable");
    }
}


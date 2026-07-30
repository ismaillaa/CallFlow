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
        var resultat = await service.EnregistrerAsync(
            req.ProspectId, req.AgentId, req.Resultat, req.DureeSecondes, req.Commentaire, req.DateRappel);

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


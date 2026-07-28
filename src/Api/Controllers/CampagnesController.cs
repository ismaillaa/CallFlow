using Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/campagnes")]
public class CampagnesController (ImporterProspectsService service) : ControllerBase
{
    [HttpPost("{campagneId}/prospects/import")]
    public async Task<IActionResult> Importer (int campagneId, IFormFile fichier)
    {
        using var reader = new StreamReader(fichier.OpenReadStream());
        var contenu = await reader.ReadToEndAsync();

        var rapport = await service.ImporterAsync(campagneId, contenu);
        return Ok(rapport);
    }
}


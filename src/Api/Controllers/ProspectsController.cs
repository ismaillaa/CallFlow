using Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api")]
public class ProspectsController(ReserverProspectService service ) : ControllerBase
{
    [HttpPost("campagnes/{campagneId}/file/suivant")]
    public async Task<IActionResult> Suivant(int campagneId, int agentId)
    {
        var p = await service.ReserverAsync(campagneId, agentId);
        return p is null? NotFound() : Ok(p);
    }
}


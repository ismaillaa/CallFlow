using Application.DTOs;
using Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("api")]
public class ProspectsController(ReserverProspectService service ) : ControllerBase
{
    [HttpPost("campagnes/{campagneId}/file/suivant")]
    public async Task<IActionResult> Suivant(int campagneId)
    {
        var agentId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var p = await service.ReserverAsync(campagneId, agentId);
        return p is null? NotFound() : Ok(ProspectDto.DepuisEntite(p));
    }
}


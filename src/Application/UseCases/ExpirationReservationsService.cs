
using Domain.Interfaces;

namespace Application.UseCases;

public class ExpirationReservationsService(IProspectRepository repo)
{
    public async Task<int> LibererExpireesAsync()
    {
        var limite = DateTime.UtcNow.AddMinutes(-15);
        var expirees = await repo.RecupererReservationsExpireesAsync(limite);
        
        foreach (var prospect in expirees)
        {
            prospect.Statut = Domain.Entities.StatutProspect.Nouveau;
            prospect.AgentReserveId = null;
            prospect.DateReservation = null;
        }

        await repo.SauvegarderAsync();
        return expirees.Count;
    }
}

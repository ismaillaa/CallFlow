using Domain.Entities;
using Domain.Interfaces;


namespace Application.UseCases;

public class ReserverProspectService(IProspectRepository repo)
{
    public async Task<Prospect?> ReserverAsync(int campagneId, int AgentId)
    {
        var p = await repo.ProchainProspectAsync(campagneId);
        if (p is null)
            return null;
        p.Statut = StatutProspect.Reserve;
        p.AgentReserveId = AgentId;
        p.DateReservation = DateTime.Now;
        var reussi = await repo.TryReserverAsync(p);
        return reussi ? p : null;
    }

}


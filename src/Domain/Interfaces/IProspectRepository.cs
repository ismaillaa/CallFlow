using Domain.Entities;

namespace Domain.Interfaces;

public interface IProspectRepository
{
    Task<Prospect?> ProchainProspectAsync(int campagneId);
    Task<Prospect?> GetByIdAsync(int id);
    Task<List<Prospect>> RechercherParTelephoneAsync(string telephone);
    Task AjouterAsync(Prospect prospect);
    Task<bool> TryReserverAsync(Prospect prosepect);
    Task AjouterAppelAsync(Appel appel);
    Task<bool> SauvegarderAsync();
    Task AjouterRappelAsync(Rappel rappel);
    Task<bool> AgentExisteAsync(int agentId);
    Task<List<Prospect>> RecupererReservationsExpireesAsync(DateTime limite);
}
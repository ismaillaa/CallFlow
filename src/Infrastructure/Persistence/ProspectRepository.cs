

using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ProspectRepository : IProspectRepository
{
    private readonly CallFlowDbContext _db;
    public ProspectRepository(CallFlowDbContext db) => _db = db;


    public async Task<Prospect?> ProchainProspectAsync(int campagneId)
    {
        return await _db.Prospects
            .Where(p => p.CampagneId == campagneId
                   && p.Statut == StatutProspect.Nouveau
                   && p.AgentReserveId == null)
            .OrderBy(p => p.Id)
            .FirstOrDefaultAsync();
    }
    public async Task<Prospect?> GetByIdAsync(int id)
    {
        return await _db.Prospects.FindAsync(id);
    }

    public async Task<List<Prospect>> RechercherParTelephoneAsync(string telephone)
    {
        return await _db.Prospects
            .AsNoTracking()
            .Where(p => p.Telephone == telephone)
            .ToListAsync();
    }

    public async Task AjouterAsync(Prospect prospect)
    {
        await _db.Prospects.AddAsync(prospect);
    }

    public async Task<bool> TryReserverAsync(Prospect prospect)
    {
        try
        {
            await _db.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            return false;
        }
    }

    public async Task AjouterAppelAsync(Appel appel)
    {
        await _db.Appels.AddAsync(appel);
    }

    public async Task<bool> SauvegarderAsync()
    {
        try
        {
            await _db.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            return false;
        }
    }

    public async Task AjouterRappelAsync(Rappel rappel)
    {
        await _db.AddAsync(rappel);
    }


}




using Domain.Entities;
using Domain.Interfaces;

namespace Application.UseCases;

public class EnregistrerAppelService(IProspectRepository repo)
{
    public async Task<ResultatEnregistrement> EnregistrerAsync(int prospectId, int agentId, ResultatAppel resultat, int dureeSecondes, string? commentaire, DateTime? dateRappel)
    {
        var prospect = await repo.GetByIdAsync(prospectId);
        if (prospect is null)
            return ResultatEnregistrement.ProspectIntrouvable;

        if (!await repo.AgentExisteAsync(agentId))
            return ResultatEnregistrement.AgentIntrouvable;

        if (dureeSecondes < 0)
            return ResultatEnregistrement.DonneesInvalides;

        var appel = new Appel
        {
            ProspectId = prospectId,
            AgentId = agentId,
            DateAppel = DateTime.UtcNow,
            DureeSecondes = dureeSecondes,
            Resultat = resultat,
            Commentaire = commentaire
        };
        await repo.AjouterAppelAsync(appel);

        prospect.NombreTentatives++;
        switch (resultat)
        {
            case ResultatAppel.Converti:
                prospect.Statut = StatutProspect.Converti;
                break;

            case ResultatAppel.NonInteresse:
                prospect.Statut = StatutProspect.NonInteresse;
                break;

            case ResultatAppel.NumeroInvalide:
                prospect.Statut = StatutProspect.Invalide;
                break;

            case ResultatAppel.RappelDemande:
                if (dateRappel is null || dateRappel <= DateTime.UtcNow)
                    return ResultatEnregistrement.DonneesInvalides;
                prospect.Statut = StatutProspect.RappelProgramme;
                await repo.AjouterRappelAsync(new Rappel
                {
                    ProspectId = prospectId,
                    AgentId = agentId,
                    DatePrevue = dateRappel.Value
                });
                break;

            case ResultatAppel.Injoignable:
                if (prospect.NombreTentatives >= 5) 
                    prospect.Statut = StatutProspect.Cloture;
                break;

            default:
                throw new ArgumentException("Resultat d 'appel inconnu");

        }
        if (!await repo.SauvegarderAsync())
            return ResultatEnregistrement.Conflit;
        else
            return ResultatEnregistrement.Succes;
    }
}


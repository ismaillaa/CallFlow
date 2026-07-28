

using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.UseCases;

public class ImporterProspectsService (IProspectRepository repo)
{
    public async Task<RapportImport> ImporterAsync(int campagneId, string contenuCsv)
    {
        var rapport = new RapportImport();
        var lignes = contenuCsv.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        var telephonesVus = new HashSet<string>();

        for (int i =1; i<lignes.Length; i++)
        {
            var colones = lignes[i].Split(',');

            if(colones.Length < 2)
            {
                rapport.Rejetees.Add(new LigneRejetee { Ligne = i + 1, Motif = "format invalide" });
                continue;
            }

            var nom = colones[0].Trim();
            var telephone = colones[1].Trim();

            if (string.IsNullOrWhiteSpace(nom) || string.IsNullOrWhiteSpace(telephone))
            {
                rapport.Rejetees.Add(new LigneRejetee { Ligne = i + 1, Motif = "nom ou telephone manquant" });
                continue;
            }

            var existants = await repo.RechercherParTelephoneAsync(telephone);
            if (existants.Any(p => p.CampagneId == campagneId))
            {
                rapport.Rejetees.Add(new LigneRejetee { Ligne = i + 1, Motif = "telephone en doublon" });
                continue;
            }

            if (!telephonesVus.Add(telephone))
            {
                rapport.Rejetees.Add(new LigneRejetee { Ligne = i + 1, Motif = "doublon dans le fichier" });
                continue;
            }

            var prospect = new Prospect
            {
                Nom = nom,
                Telephone = telephone,
                CampagneId = campagneId,
                Statut = StatutProspect.Nouveau
            };
            await repo.AjouterAsync(prospect);
            rapport.Acceptes++;

        }
        await repo.SauvegarderAsync();
        return rapport;
    }

}


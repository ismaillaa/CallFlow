

using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace Domain.Tests;

public class ReservationConcurrenceTests
{
    private const string ConnTest = "Host=localhost;Port=5433;Database=callflow_test;Username=postgres;Password=root04";

    private CallFlowDbContext NouveauContexte()
    {
        var options = new DbContextOptionsBuilder<CallFlowDbContext>().UseNpgsql(ConnTest).Options;
        return new CallFlowDbContext(options);
    }

    [Fact]
    public async Task DeuxAgents_UnSeulReserveLeProspect()
    {
        using (var setup = NouveauContexte())
        {
            await setup.Database.EnsureDeletedAsync();
            await setup.Database.EnsureCreatedAsync();
            var campagne = new Campagne { Nom = "Test", DateDebut = default, DateFin = default };
            setup.Campagnes.Add(campagne);
            await setup.SaveChangesAsync();
            setup.Prospects.Add(new Prospect
            {
                Nom = "Cible",
                Telephone = "0600",
                CampagneId = campagne.Id,
                Statut = StatutProspect.Nouveau
            });
            await setup.SaveChangesAsync();
        }

        using (var check = NouveauContexte())
        {
            var nb = await check.Prospects.CountAsync();
            Assert.True(nb > 0, $"Setup raté : {nb} prospect(s) en base");
        }

        // Les deux threads lisent AVANT que l'un sauvegarde → vraie collision
        async Task<bool> Reserver(int agentId, Task barriere)
        {
            using var ctx = NouveauContexte();
            var p = await ctx.Prospects.FirstAsync(x => x.Statut == StatutProspect.Nouveau);
            p.Statut = StatutProspect.Reserve;
            p.AgentReserveId = agentId;

            await barriere;   // on attend que les DEUX aient lu avant de sauvegarder

            try { await ctx.SaveChangesAsync(); return true; }
            catch (DbUpdateConcurrencyException) { return false; }
        }

        var top = new TaskCompletionSource();
        var t1 = Reserver(1, top.Task);
        var t2 = Reserver(2, top.Task);
        await Task.Delay(200);   // laisse les deux atteindre la barrière (avoir lu)
        top.SetResult();          // top départ : les deux sauvegardent maintenant

        var resultats = await Task.WhenAll(t1, t2);
        Assert.Equal(1, resultats.Count(r => r));
    }
}


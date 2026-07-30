

using Application.UseCases;
using Domain.Entities;
using Domain.Interfaces;
using Moq;

namespace Domain.Tests;

public class EnregistrerAppelServiceTests
{
    [Fact]
    public async Task Converti_PasseLeProspectEnConverti_EtIncrementeTentatives()
    {
        var prospect = new Prospect { Id = 1, Statut = StatutProspect.Nouveau, NombreTentatives = 0 };
        var repo = new Mock<IProspectRepository>();
        repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(prospect);
        repo.Setup(r => r.SauvegarderAsync()).ReturnsAsync(true);
        repo.Setup(r => r.AgentExisteAsync(It.IsAny<int>())).ReturnsAsync(true);
        var service = new EnregistrerAppelService(repo.Object);

        var r = await service.EnregistrerAsync(1, 1, ResultatAppel.Converti, 120, "ok", null);

        Assert.Equal(ResultatEnregistrement.Succes, r);
        Assert.Equal(StatutProspect.Converti, prospect.Statut);
        Assert.Equal(1, prospect.NombreTentatives);
    }

    [Fact]
    public async Task CinquiemeProspectInjoignable_ClotureduProspect()
    {
        var prospect = new Prospect { Id = 1, Statut = StatutProspect.Nouveau, NombreTentatives = 4 };
        var repo = new Mock<IProspectRepository>();
        repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(prospect);
        repo.Setup(r => r.SauvegarderAsync()).ReturnsAsync(true);
        repo.Setup(r => r.AgentExisteAsync(It.IsAny<int>())).ReturnsAsync(true);
        var service = new EnregistrerAppelService(repo.Object);

        var r = await service.EnregistrerAsync(1, 1, ResultatAppel.Injoignable, 120, "ok", null);

        Assert.Equal(ResultatEnregistrement.Succes, r);
        Assert.Equal(5, prospect.NombreTentatives);
        Assert.Equal(StatutProspect.Cloture, prospect.Statut);
    }


    [Fact]
    public async Task QuatriemeInjoignable_NeCloturePas()
    {
        var prospect = new Prospect { Id = 1, Statut = StatutProspect.Nouveau, NombreTentatives = 3 };
        var repo = new Mock<IProspectRepository>();
        repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(prospect);
        repo.Setup(r => r.SauvegarderAsync()).ReturnsAsync(true);
        repo.Setup(r => r.AgentExisteAsync(It.IsAny<int>())).ReturnsAsync(true);
        var service = new EnregistrerAppelService(repo.Object);

        var r = await service.EnregistrerAsync(1, 1, ResultatAppel.Injoignable, 120, "ok", null);

        Assert.Equal(ResultatEnregistrement.Succes, r);
        Assert.Equal(4, prospect.NombreTentatives);
        Assert.Equal(StatutProspect.Nouveau, prospect.Statut);
    }

    [Fact]
    public async Task RappelDemandeSansDate()
    {
        var prospect = new Prospect { Id = 1, Statut = StatutProspect.Nouveau, NombreTentatives = 3 };
        var repo = new Mock<IProspectRepository>();
        repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(prospect);
        repo.Setup(r => r.SauvegarderAsync()).ReturnsAsync(true);
        repo.Setup(r => r.AgentExisteAsync(It.IsAny<int>())).ReturnsAsync(true);
        var service = new EnregistrerAppelService(repo.Object);

        var r = await service.EnregistrerAsync(1, 1, ResultatAppel.RappelDemande, 120, "ok", null);

        Assert.NotEqual(ResultatEnregistrement.Succes, r);
        Assert.Equal(4, prospect.NombreTentatives);
        Assert.Equal(StatutProspect.Nouveau, prospect.Statut);
    }



}


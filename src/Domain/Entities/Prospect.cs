using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public enum StatutProspect
{
    Nouveau,
    Reserve,
    RappelProgramme,
    Converti,
    NonInteresse,
    Cloture,
    Invalide
}

public class Prospect
{
    public int Id { get; set; }
    [MaxLength(100)]
    public string Nom { get; set; } = string.Empty;
    [MaxLength(20)]
    public string Telephone { get; set; } = string.Empty;
    [MaxLength(150)]
    public string? Email { get; set; }
    [MaxLength(100)]
    public string? Ville { get; set; }
    public StatutProspect Statut { get; set; }
    public int NombreTentatives { get; set; } = 0;
    public uint RowVersion { get; set; }

    public int CampagneId { get; set; }
    public Campagne Campagne { get; set; } = null!;

    public int? AgentReserveId { get; set; }
    public DateTime? DateReservation { get; set; }

    public ICollection<Appel> Appels { get; set; } = [];
    public ICollection<Rappel> Rappels { get; set; } = [];
}
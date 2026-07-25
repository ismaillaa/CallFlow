

using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public enum ResultatAppel { 
    Converti,
    NonInteresse,
    RappelDemande,
    Injoignable,
    NumeroInvalide
}
public class Appel
{
    public int Id { get; set; }
    public DateTime DateAppel { get; set; }
    public int DureeSecondes { get; set; }
    public ResultatAppel Resultat {  get; set; }
    [MaxLength(500)]
    public string? Commentaire { get; set; }

    public int ProspectId { get; set; }
    public Prospect Prospect { get; set; } = null!;
    public int AgentId { get; set; }
    public Agent Agent { get; set; } = null!;
}


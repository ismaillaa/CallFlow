namespace Domain.Entities;

public enum SecteurCampagne
{
    Energie,
    Telecom,
    Assurance
}

public enum StatutCampagne
{
    Brouillon,
    Active,
    Suspendue,
    Termine
}

public class Campagne
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public SecteurCampagne Secteur { get; set; }
    public DateOnly DateDebut { get; set; }
    public DateOnly DateFin { get; set; }
    public StatutCampagne Statut { get; set; }
}
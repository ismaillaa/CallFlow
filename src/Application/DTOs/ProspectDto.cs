namespace Application.DTOs;

public record ProspectDto(
    int Id,
    string Nom,
    string Telephone,
    string? Email,
    string? Ville,
    string Statut,
    int NombreTentatives
    )
{
    public static ProspectDto DepuisEntite(Domain.Entities.Prospect p) =>
        new(p.Id, p.Nom, p.Telephone, p.Email, p.Ville, p.Statut.ToString(), p.NombreTentatives);
}
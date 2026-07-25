
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public enum RoleAgent
{
    Agent,
    Superviseur,
    Administrateur
}
public class Agent
{
    public int Id { get; set; }
    [MaxLength(100)]
    public string NomComplet { get; set; } = string.Empty;
    [MaxLength(50)]
    public string Identifiant { get; set; } = string.Empty;
    public string MotDePasseHash { get; set; } = string.Empty;
    public RoleAgent Role { get; set; }
    public bool Actif { get; set; } = true;


}




namespace Application.DTOs;

public class RapportImport
{
    public int Acceptes { get; set; }
    public List<LigneRejetee> Rejetees { get; set; } = [];
}

public class LigneRejetee
{
    public int Ligne { get; set; }
    public string Motif { get; set; } = string.Empty;
}


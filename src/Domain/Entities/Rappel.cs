

namespace Domain.Entities;

public class Rappel
{
   public int Id { get; set; }
   public DateTime DatePrevue { get; set; }
   public bool Honore { get; set; } = false;

   public int ProspectId { get; set; }
   public Prospect Prospect { get; set; } = null!;
   public int AgentId { get; set; }
   public Agent Agent { get; set; } = null!;


}


using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProtocolEditor.Entities;

namespace ProtocolEditor.Models;

public partial class Competition
{
    [Key]
    public int IDCompetition { get; set; }

    [StringLength(255)]
    public string CompetitionName { get; set; } = null!;

    [InverseProperty("IDCompetitionNavigation")]
    public virtual ICollection<CompetitionResult> CompetitionResults { get; set; } = new List<CompetitionResult>();
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProtocolEditor.Entities;

public partial class Command
{
    [Key]
    public int IDCommand { get; set; }

    [StringLength(255)]
    public string CommandName { get; set; } = null!;

    [InverseProperty("IDCommandNavigation")]
    public virtual ICollection<CombineRelay> CombineRelays { get; set; } = new List<CombineRelay>();

    [InverseProperty("IDCommandNavigation")]
    public virtual ICollection<CommandsForRelay> CommandsForRelays { get; set; } = new List<CommandsForRelay>();

    [InverseProperty("IDCommandNavigation")]
    public virtual ICollection<CompetitionSummary> CompetitionSummaries { get; set; } = new List<CompetitionSummary>();
}

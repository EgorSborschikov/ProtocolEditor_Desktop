using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProtocolEditor.Models;

public partial class CompetitionResult
{
    [Key]
    public int IDCompetitionResult { get; set; }

    public int IDCompetition { get; set; }

    public int? Place { get; set; }

    public int? Points { get; set; }

    [InverseProperty("IDCompetitionResultNavigation")]
    public virtual ICollection<CompetitionSummary> CompetitionSummaries { get; set; } = new List<CompetitionSummary>();

    [ForeignKey("IDCompetition")]
    [InverseProperty("CompetitionResults")]
    public virtual Competition IDCompetitionNavigation { get; set; } = null!;
}

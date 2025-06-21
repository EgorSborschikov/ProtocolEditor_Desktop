using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProtocolEditor.Models;

[Table("CompetitionSummary")]
public partial class CompetitionSummary
{
    [Key]
    public int IDCompetitionSummary { get; set; }

    public int IDCommand { get; set; }

    public int IDCompetitionResult { get; set; }

    public int? TotalPoints { get; set; }

    public int? Place { get; set; }

    [ForeignKey("IDCommand")]
    [InverseProperty("CompetitionSummaries")]
    public virtual Command IDCommandNavigation { get; set; } = null!;

    [ForeignKey("IDCompetitionResult")]
    [InverseProperty("CompetitionSummaries")]
    public virtual CompetitionResult IDCompetitionResultNavigation { get; set; } = null!;
}

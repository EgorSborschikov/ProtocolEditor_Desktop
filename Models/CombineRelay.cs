using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProtocolEditor.Entities;

[Table("CombineRelay")]
public partial class CombineRelay
{
    [Key]
    public int IDCombineRelay { get; set; }

    public int IDCommand { get; set; }

    [Precision(6, 0)]
    public DateTime Time { get; set; }

    public int Place { get; set; }

    [ForeignKey("IDCommand")]
    [InverseProperty("CombineRelays")]
    public virtual Command IDCommandNavigation { get; set; } = null!;
}

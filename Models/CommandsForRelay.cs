using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using ProtocolEditor.Entities;

namespace ProtocolEditor.Models;

[Table("CommandsForRelay")]
public partial class CommandsForRelay
{
    [Key]
    public int IDCommandForRelay { get; set; }

    public int IDCommand { get; set; }

    [Precision(6, 0)]
    public DateTime Time { get; set; }

    public int Place { get; set; }

    public int Points { get; set; }

    [InverseProperty("IDCommandForRelayNavigation")]
    public virtual ICollection<GroupsForRelay> GroupsForRelays { get; set; } = new List<GroupsForRelay>();

    [ForeignKey("IDCommand")]
    [InverseProperty("CommandsForRelays")]
    public virtual Command IDCommandNavigation { get; set; } = null!;
}

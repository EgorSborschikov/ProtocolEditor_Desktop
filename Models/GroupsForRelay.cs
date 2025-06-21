using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProtocolEditor.Models;

[Table("GroupsForRelay")]
public partial class GroupsForRelay
{
    [Key]
    public int IDGroupForRelay { get; set; }

    public int IDCommandForRelay { get; set; }

    [ForeignKey("IDCommandForRelay")]
    [InverseProperty("GroupsForRelays")]
    public virtual CommandsForRelay IDCommandForRelayNavigation { get; set; } = null!;
}

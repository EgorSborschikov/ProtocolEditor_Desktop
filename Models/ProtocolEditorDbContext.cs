using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ProtocolEditor.Entities;

public partial class ProtocolEditorDbContext : DbContext
{
    public ProtocolEditorDbContext()
    {
    }

    public ProtocolEditorDbContext(DbContextOptions<ProtocolEditorDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CombineRelay> CombineRelays { get; set; }

    public virtual DbSet<Command> Commands { get; set; }

    public virtual DbSet<CommandsForRelay> CommandsForRelays { get; set; }

    public virtual DbSet<Competition> Competitions { get; set; }

    public virtual DbSet<CompetitionResult> CompetitionResults { get; set; }

    public virtual DbSet<CompetitionSummary> CompetitionSummaries { get; set; }

    public virtual DbSet<GroupsForRelay> GroupsForRelays { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = Program.Configuration?.GetConnectionString("DefaultConnection");
        
        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException("Connection string is missing");
        
        optionsBuilder.UseNpgsql(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CombineRelay>(entity =>
        {
            entity.HasKey(e => e.IDCombineRelay).HasName("CombineRelay_pkey");

            entity.Property(e => e.IDCombineRelay).ValueGeneratedNever();

            entity.HasOne(d => d.IDCommandNavigation).WithMany(p => p.CombineRelays)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("CombineRelay_IDCommand_fkey");
        });

        modelBuilder.Entity<Command>(entity =>
        {
            entity.HasKey(e => e.IDCommand).HasName("Commands_pkey");

            entity.Property(e => e.IDCommand).ValueGeneratedNever();
        });

        modelBuilder.Entity<CommandsForRelay>(entity =>
        {
            entity.HasKey(e => e.IDCommandForRelay).HasName("CommandsForRelay_pkey");

            entity.Property(e => e.IDCommandForRelay).ValueGeneratedNever();

            entity.HasOne(d => d.IDCommandNavigation).WithMany(p => p.CommandsForRelays)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("CommandsForRelay_IDCommand_fkey");
        });

        modelBuilder.Entity<Competition>(entity =>
        {
            entity.HasKey(e => e.IDCompetition).HasName("Competitions_pkey");

            entity.Property(e => e.IDCompetition).ValueGeneratedNever();
        });

        modelBuilder.Entity<CompetitionResult>(entity =>
        {
            entity.HasKey(e => e.IDCompetitionResult).HasName("CompetitionResults_pkey");

            entity.Property(e => e.IDCompetitionResult).ValueGeneratedNever();

            entity.HasOne(d => d.IDCompetitionNavigation).WithMany(p => p.CompetitionResults)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("CompetitionResults_IDCompetition_fkey");
        });

        modelBuilder.Entity<CompetitionSummary>(entity =>
        {
            entity.HasKey(e => e.IDCompetitionSummary).HasName("CompetitionSummary_pkey");

            entity.Property(e => e.IDCompetitionSummary).ValueGeneratedNever();

            entity.HasOne(d => d.IDCommandNavigation).WithMany(p => p.CompetitionSummaries)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("CompetitionSummary_IDCommand_fkey");

            entity.HasOne(d => d.IDCompetitionResultNavigation).WithMany(p => p.CompetitionSummaries)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("CompetitionSummary_IDCompetitionResult_fkey");
        });

        modelBuilder.Entity<GroupsForRelay>(entity =>
        {
            entity.HasKey(e => e.IDGroupForRelay).HasName("GroupsForRelay_pkey");

            entity.Property(e => e.IDGroupForRelay).ValueGeneratedNever();

            entity.HasOne(d => d.IDCommandForRelayNavigation).WithMany(p => p.GroupsForRelays)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("GroupsForRelay_IDCommandForRelay_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

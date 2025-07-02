using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using ProtocolEditor.Models;

namespace ProtocolEditor.ViewModels;

/// <summary>
/// Интерфейс, содержащий логику управления опциями для работы со сводной ведомостью
/// </summary>

public class CompetitionSummaryViewModel : INotifyPropertyChanged
{
    private readonly ProtocolEditorDbContext _context;
    private Models.Competition? _selectedCompetition;

    public event Action? UpdateColumnsRequested;
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public ObservableCollection<TeamViewModel> Teams { get; } = new();
    public ObservableCollection<Competition> Competitions { get; } = new();
    public ObservableCollection<Models.Competition> AvailableCompetitions { get; } = new();

    public Models.Competition? SelectedCompetition
    {
        get => _selectedCompetition;
        set
        {
            _selectedCompetition = value;
            OnPropertyChanged();
        }
    }

    public CompetitionSummaryViewModel()
    {
        _context = new ProtocolEditorDbContext();
    }

    public void Initialize()
    {
        LoadData();
    }

    public void LoadData()
    {
        var teams = _context.Commands
            .Select(c => new TeamViewModel(c.IDCommand, c.CommandName))
            .ToList();

        Teams.Clear();
        foreach (var team in teams)
        {
            Teams.Add(team);
        }

        AvailableCompetitions.Clear();
        foreach (var comp in _context.Competitions)
        {
            AvailableCompetitions.Add(comp);
        }
    }

    public void AddCompetition()
    {
        if (SelectedCompetition != null && !Competitions.Contains(SelectedCompetition))
        {
            Competitions.Add(SelectedCompetition);
            
            // Инициализация значений для всех команд
            foreach (var team in Teams)
            {
                team.CompetitionPoints[SelectedCompetition.IDCompetition] = 0;
                team.CompetitionPlaces[SelectedCompetition.IDCompetition] = 0;
            }
            
            UpdateColumnsRequested?.Invoke();
        }
    }
    
    public void RemoveCompetition()
    {
        if (SelectedCompetition != null && Competitions.Contains(SelectedCompetition))
        {
            // Удаляем данные для этого соревнования у всех команд
            foreach (var team in Teams)
            {
                team.CompetitionPoints.Remove(SelectedCompetition.IDCompetition);
                team.CompetitionPlaces.Remove(SelectedCompetition.IDCompetition);
            }
            
            Competitions.Remove(SelectedCompetition);
            UpdateColumnsRequested?.Invoke();
        }
    }

    public void SortByPlace()
    {
        var sortedTeams = Teams
            .OrderByDescending(t => t.TotalPoints)
            .ToList();

        int place = 1;
        for (int i = 0; i < sortedTeams.Count; i++)
        {
            if (i > 0 && sortedTeams[i].TotalPoints != sortedTeams[i - 1].TotalPoints)
                place = i + 1;
            
            sortedTeams[i].Place = place;
        }

        // Обновление коллекции
        Teams.Clear();
        foreach (var team in sortedTeams)
            Teams.Add(team);
    }

    public void Save()
    {
        using var transaction = _context.Database.BeginTransaction();
        
        try
        {
            foreach (var team in Teams)
            {
                // Сохраняем результаты по каждому соревнованию
                foreach (var competition in Competitions)
                {
                    var result = _context.CompetitionResults
                        .FirstOrDefault(cr => 
                            cr.IDCompetition == competition.IDCompetition && 
                            cr.CompetitionSummaries.Any(cs => cs.IDCommand == team.TeamId));
                    
                    if (result == null)
                    {
                        result = new CompetitionResult
                        {
                            IDCompetition = competition.IDCompetition,
                            Points = team.CompetitionPoints.TryGetValue(competition.IDCompetition, out var points) ? points : 0,
                            Place = team.CompetitionPlaces.TryGetValue(competition.IDCompetition, out var place) ? place : 0
                        };
                        _context.Add(result);
                    }
                    else
                    {
                        result.Points = team.CompetitionPoints.TryGetValue(competition.IDCompetition, out var points) ? points : 0;
                        result.Place = team.CompetitionPlaces.TryGetValue(competition.IDCompetition, out var place) ? place : 0;
                    }
                }
                
                // Сохраняем сводные данные
                var summary = _context.CompetitionSummaries
                    .FirstOrDefault(cs => cs.IDCommand == team.TeamId);
                
                if (summary == null)
                {
                    summary = new CompetitionSummary
                    {
                        IDCommand = team.TeamId,
                        TotalPoints = team.TotalPoints,
                        Place = team.Place
                    };
                    _context.Add(summary);
                }
                else
                {
                    summary.TotalPoints = team.TotalPoints;
                    summary.Place = team.Place;
                }
            }
            
            _context.SaveChanges();
            transaction.Commit();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            // Обработка ошибок
            Console.WriteLine($"Ошибка сохранения: {ex.Message}");
        }
    }
}

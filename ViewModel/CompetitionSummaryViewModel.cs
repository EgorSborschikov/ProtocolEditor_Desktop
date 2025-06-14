using System;
using System.Collections.ObjectModel;

namespace ProtocolEditor.ViewModel;

public class CompetitionSummaryViewModel
{
    public ObservableCollection<Team> Teams { get; set; }

    public CompetitionSummaryViewModel()
    {
        Teams = new ObservableCollection<Team>();
    }

    public void AddTeam()
    {
        Teams.Add(new Team
        {
            TeamId = Teams.Count + 1,
            TeamName = "новая команда"
        });
    }

    public void AddCompetition()
    {
        foreach (var team in Teams)
        {
            team.Competitions.Add(new Competition());
        }
    }
}

public class Team
{
    public int TeamId { get; set; }
    public string TeamName { get; set; }
    public DateTime Time { get; set; }
    public int Place { get; set; }
    public int Points { get; set; }
    public int MinPoints { get; set; }
    public ObservableCollection<Competition> Competitions { get; set; }

    public Team()
    {
        Competitions = new ObservableCollection<Competition>();
    }
}

public class Competition
{
    public int Place { get; set; }
    public int Points { get; set; }
}

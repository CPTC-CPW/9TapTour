using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NineTapTour.Models;

public class Tournament
{
    [Key]
    public int Id { get; set; }
    [Required]
    public DateTime Date { get; set; }
    [Required]
    public string Location { get; set; }
    public string Event { get; set; }
    public string Notes { get; set; }
    public string Sponsors { get; set; }
    public List<Participant> Participant { get; set; }
   
    public int Squads { get; set; } 
    public bool Doubles { get; set; }
    public bool ThreeOutOf4 {get; set;}

    /// <summary>
    /// If set to true, this tournament will only use 3 games
    /// and will skip the 4th game
    /// </summary>
    public bool IsOnlyThreeGames { get; set; }

    public string  TourneyNameDate
    {
        get { return Location + " " +  Date.ToShortDateString(); }
    }

    public bool IsTournamentFinalized { get; set; }
    
}

public class TournamentDTO
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string Location { get; set; }
    public string Event { get; set; }
    public string Notes { get; set; }
    public string Sponsors { get; set; }
    public List<Participant> Participant { get; set; }
    public bool Doubles { get; set; }

    public string TourneyNameDate
    {
        get { return Location + " " + Date.ToShortDateString(); }

    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NineTapTour.Database
{
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
        public bool Doubles { get; set; }

        public string  TourneyNameDate
        {
            get { return Location + " " +  Date.ToShortDateString(); }
           
        }
        
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
}
namespace Assignment.Models
{
    public class PlanningModel
    {
        public int JobId { get; set; }

        public string Title { get; set; }

        public string TaskDescription { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public List<string> SelectedExecutives { get; set; }
            = new List<string>();
    }
}
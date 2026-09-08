namespace Assignment.Models
{
    public class JobModel
    {
        public int Id { get; set; }

        public string JobTitle { get; set; }
        public string Company { get; set; }
        public string HR { get; set; }
        public string SkillsSet { get; set; }
        public string Qualification { get; set; }

        public int Experience { get; set; }
        public decimal Salary { get; set; }

        public string JobLocation { get; set; }
        public string Industry { get; set; }

        public int NumberOfOpening { get; set; }

        public int OriginalNumberOfOpening { get; set; }
        public int AgeLimit { get; set; }

        public DateTime VacancyLiveDate { get; set; }

        public string InterviewAddress { get; set; }
        public string RecruiterLevel { get; set; }
        public string SpecialNote { get; set; }

        public string JobPriority { get; set; }

        public string JobDescription { get; set; }

        public string CreatedBy { get; set; }

        public int PlanningTaskCount { get; set; }
    }
}
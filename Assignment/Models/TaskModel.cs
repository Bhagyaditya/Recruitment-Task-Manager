namespace Assignment.Models
{
    public class TaskModel
    {
        public int Id { get; set; }

        public string AssignedTo { get; set; }

        public string TaskDescription { get; set; }

        public string AssignedBy { get; set; }

        public string Title { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Type { get; set; }

        public int CVRequired { get; set; }

        public string Status { get; set; }

        public string CompletionNote { get; set; }

        public int JobId { get; set; }

        public string JobTitle { get; set; }

        public int CVCount { get; set; }
    }
}

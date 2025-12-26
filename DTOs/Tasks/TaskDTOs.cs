namespace ERP.DTOs.Tasks
{
    public class TaskCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TaskType TaskType { get; set; } = TaskType.Administrative;
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public Helpers.Enums.TaskStatus Status { get; set; } = Helpers.Enums.TaskStatus.New;
        public string StartDate { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd");
        public string? DueDate { get; set; }
        public int? ProjectId { get; set; }
        public TaskReferenceType? ReferenceType { get; set; }
        public int? ReferenceId { get; set; }
        public int? AssignedToEmployeeId { get; set; }
        public int? AssignedPartnerId { get; set; }
    }

    public class TaskUpdateDto : TaskCreateDto
    {
        public int Id { get; set; }
    }

    public class TaskFilterDto
    {
        public int? ProjectId { get; set; }
        public TaskReferenceType? ReferenceType { get; set; }
        public int? ReferenceId { get; set; }
        public TaskType? TaskType { get; set; }
        public TaskPriority? Priority { get; set; }
        public Helpers.Enums.TaskStatus? Status { get; set; }
        public int? AssignedToEmployeeId { get; set; }
        public int? AssignedPartnerId { get; set; }
        public string? From { get; set; }
        public string? To { get; set; }
        public string? Keyword { get; set; }
    }

    public class TaskListDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TaskType TaskType { get; set; }
        public TaskPriority Priority { get; set; }
        public Helpers.Enums.TaskStatus Status { get; set; }
        public string StartDate { get; set; } = string.Empty;
        public string? DueDate { get; set; }
        public int? ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public TaskReferenceType? ReferenceType { get; set; }
        public int? ReferenceId { get; set; }
        public int? AssignedToEmployeeId { get; set; }
        public string? AssignedToEmployeeName { get; set; }
        public int? AssignedPartnerId { get; set; }
        public string? AssignedPartnerName { get; set; }
    }
}

using ERP.DTOs.Tasks;
using ERP.Models.ProjectsManagement;
using ERP.Repositories.Interfaces.ProjectsManagement;
using ERP.Services.Interfaces.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ERP.Services.Tasks
{
    public class TaskService : ITaskService
    {
        private readonly IProjectTaskRepository _taskRepo;
        private readonly IProjectRepository _projectRepo;
        private readonly IErrorRepository _errors;

        public TaskService(
            IProjectTaskRepository taskRepo,
            IProjectRepository projectRepo,
            IErrorRepository errors)
        {
            _taskRepo = taskRepo;
            _projectRepo = projectRepo;
            _errors = errors;
        }

        public async Task<ResponseDTO> CreateAsync(TaskCreateDto dto, int userId)
        {
            const string fn = nameof(CreateAsync);
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Title))
                    return new ResponseDTO { IsValid = false, Message = "Title is required" };

                if (!DateTime.TryParse(dto.StartDate, out var startDate))
                    return new ResponseDTO { IsValid = false, Message = "Invalid StartDate" };

                DateTime? dueDate = null;
                if (!string.IsNullOrWhiteSpace(dto.DueDate))
                {
                    if (!DateTime.TryParse(dto.DueDate, out var parsedDue))
                        return new ResponseDTO { IsValid = false, Message = "Invalid DueDate" };
                    dueDate = parsedDue;
                }

                if (dueDate.HasValue && dueDate.Value.Date < startDate.Date)
                    return new ResponseDTO { IsValid = false, Message = "DueDate cannot be before StartDate" };

                if (dto.AssignedToEmployeeId.HasValue && dto.AssignedPartnerId.HasValue)
                    return new ResponseDTO { IsValid = false, Message = "Assign to either an employee or a partner, not both" };

                if (dto.ReferenceType.HasValue != dto.ReferenceId.HasValue)
                    return new ResponseDTO { IsValid = false, Message = "ReferenceType and ReferenceId must be provided together" };

                if (dto.ProjectId.HasValue)
                {
                    var project = await _projectRepo.GetByIdAsync(dto.ProjectId.Value);
                    if (project == null)
                        return new ResponseDTO { IsValid = false, Message = "Project not found" };
                }

                var entity = new ProjectTask
                {
                    Title = dto.Title.Trim(),
                    Description = dto.Description ?? string.Empty,
                    TaskType = dto.TaskType,
                    Priority = dto.Priority,
                    Status = dto.Status,
                    StartDate = startDate,
                    DueDate = dueDate,
                    ProjectId = dto.ProjectId,
                    ReferenceType = dto.ReferenceType,
                    ReferenceId = dto.ReferenceId,
                    AssignedToId = dto.AssignedToEmployeeId,
                    AssignedPartnerId = dto.AssignedPartnerId
                };

                await _taskRepo.CreateAsync(entity, userId);

                return new ResponseDTO { IsValid = true, Data = entity.Id, Message = "Task created successfully." };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", userId);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }

        public async Task<ResponseDTO> UpdateAsync(TaskUpdateDto dto, int userId)
        {
            const string fn = nameof(UpdateAsync);
            try
            {
                var entity = await _taskRepo.GetByIdAsync(dto.Id);
                if (entity == null)
                    return new ResponseDTO { IsValid = false, Message = "Task not found" };

                if (string.IsNullOrWhiteSpace(dto.Title))
                    return new ResponseDTO { IsValid = false, Message = "Title is required" };

                if (!DateTime.TryParse(dto.StartDate, out var startDate))
                    return new ResponseDTO { IsValid = false, Message = "Invalid StartDate" };

                DateTime? dueDate = null;
                if (!string.IsNullOrWhiteSpace(dto.DueDate))
                {
                    if (!DateTime.TryParse(dto.DueDate, out var parsedDue))
                        return new ResponseDTO { IsValid = false, Message = "Invalid DueDate" };
                    dueDate = parsedDue;
                }

                if (dueDate.HasValue && dueDate.Value.Date < startDate.Date)
                    return new ResponseDTO { IsValid = false, Message = "DueDate cannot be before StartDate" };

                if (dto.AssignedToEmployeeId.HasValue && dto.AssignedPartnerId.HasValue)
                    return new ResponseDTO { IsValid = false, Message = "Assign to either an employee or a partner, not both" };

                if (dto.ReferenceType.HasValue != dto.ReferenceId.HasValue)
                    return new ResponseDTO { IsValid = false, Message = "ReferenceType and ReferenceId must be provided together" };

                if (dto.ProjectId.HasValue)
                {
                    var project = await _projectRepo.GetByIdAsync(dto.ProjectId.Value);
                    if (project == null)
                        return new ResponseDTO { IsValid = false, Message = "Project not found" };
                }

                entity.Title = dto.Title.Trim();
                entity.Description = dto.Description ?? string.Empty;
                entity.TaskType = dto.TaskType;
                entity.Priority = dto.Priority;
                entity.Status = dto.Status;
                entity.StartDate = startDate;
                entity.DueDate = dueDate;
                entity.ProjectId = dto.ProjectId;
                entity.ReferenceType = dto.ReferenceType;
                entity.ReferenceId = dto.ReferenceId;
                entity.AssignedToId = dto.AssignedToEmployeeId;
                entity.AssignedPartnerId = dto.AssignedPartnerId;

                await _taskRepo.UpdateAsync(entity, userId);

                return new ResponseDTO { IsValid = true, Message = "Task updated successfully." };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", userId);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }

        public async Task<ResponseDTO> DeleteAsync(int id, int userId)
        {
            const string fn = nameof(DeleteAsync);
            try
            {
                var entity = await _taskRepo.GetByIdAsync(id);
                if (entity == null)
                    return new ResponseDTO { IsValid = false, Message = "Task not found" };

                await _taskRepo.SoftDeleteAsync(id, userId);

                return new ResponseDTO { IsValid = true, Message = "Task deleted successfully." };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", userId);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }

        public async Task<ResponseDTO> GetAsync(int id)
        {
            const string fn = nameof(GetAsync);
            try
            {
                var task = await _taskRepo.Query()
                    .Include(t => t.Project)
                    .Include(t => t.AssignedTo)
                    .Include(t => t.AssignedPartner)
                    .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

                if (task == null)
                    return new ResponseDTO { IsValid = false, Message = "Task not found" };

                var dto = MapToDto(task);
                return new ResponseDTO { IsValid = true, Data = dto };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", 0);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }

        public async Task<ResponseDTO> GetAllAsync(TaskFilterDto filter)
        {
            const string fn = nameof(GetAllAsync);
            try
            {
                var query = _taskRepo.Query()
                    .Include(t => t.Project)
                    .Include(t => t.AssignedTo)
                    .Include(t => t.AssignedPartner)
                    .Where(t => !t.IsDeleted);

                if (filter.ProjectId.HasValue)
                    query = query.Where(t => t.ProjectId == filter.ProjectId.Value);

                if (filter.ReferenceType.HasValue)
                    query = query.Where(t => t.ReferenceType == filter.ReferenceType.Value);

                if (filter.ReferenceId.HasValue)
                    query = query.Where(t => t.ReferenceId == filter.ReferenceId.Value);

                if (filter.TaskType.HasValue)
                    query = query.Where(t => t.TaskType == filter.TaskType.Value);

                if (filter.Priority.HasValue)
                    query = query.Where(t => t.Priority == filter.Priority.Value);

                if (filter.Status.HasValue)
                    query = query.Where(t => t.Status == filter.Status.Value);

                if (filter.AssignedToEmployeeId.HasValue)
                    query = query.Where(t => t.AssignedToId == filter.AssignedToEmployeeId.Value);

                if (filter.AssignedPartnerId.HasValue)
                    query = query.Where(t => t.AssignedPartnerId == filter.AssignedPartnerId.Value);

                if (!string.IsNullOrWhiteSpace(filter.From) && !string.IsNullOrWhiteSpace(filter.To)
                    && DateTime.TryParse(filter.From, out var from)
                    && DateTime.TryParse(filter.To, out var to))
                {
                    query = query.Where(t => t.StartDate >= from && t.StartDate <= to);
                }

                if (!string.IsNullOrWhiteSpace(filter.Keyword))
                {
                    var keyword = filter.Keyword.ToLower();
                    query = query.Where(t =>
                        (!string.IsNullOrEmpty(t.Title) && t.Title.ToLower().Contains(keyword)) ||
                        (!string.IsNullOrEmpty(t.Description) && t.Description.ToLower().Contains(keyword)));
                }

                var list = await query
                    .OrderByDescending(t => t.Id)
                    .ToListAsync();

                var dtos = list.Select(MapToDto).ToList();
                return new ResponseDTO { IsValid = true, Data = dtos };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", 0);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }

        private static TaskListDto MapToDto(ProjectTask task)
        {
            return new TaskListDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                TaskType = task.TaskType,
                Priority = task.Priority,
                Status = task.Status,
                StartDate = task.StartDate.ToString("yyyy-MM-dd"),
                DueDate = task.DueDate?.ToString("yyyy-MM-dd"),
                ProjectId = task.ProjectId,
                ProjectName = task.Project?.ProjectName,
                ReferenceType = task.ReferenceType,
                ReferenceId = task.ReferenceId,
                AssignedToEmployeeId = task.AssignedToId,
                AssignedToEmployeeName = task.AssignedTo?.Name,
                AssignedPartnerId = task.AssignedPartnerId,
                AssignedPartnerName = task.AssignedPartner?.FullName
            };
        }
    }
}

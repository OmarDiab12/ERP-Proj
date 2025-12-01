using ERP.DTOs.EngineeringOffice;
using ERP.Helpers;
using ERP.Models.EngineeringOffice;
using ERP.Repositories.Interfaces.EngineeringOffice;

namespace ERP.Services.EngineeringOffice
{
    public class EngineeringOfficeService : IEngineeringOfficeService
    {
        private readonly IEngineeringProjectRepository _projectRepo;
        private readonly IEngineeringProjectAttachmentRepository _attachmentRepo;
        private readonly IFileStorageService _fileStorage;
        private readonly IErrorRepository _errors;

        public EngineeringOfficeService(
            IEngineeringProjectRepository projectRepo,
            IEngineeringProjectAttachmentRepository attachmentRepo,
            IFileStorageService fileStorage,
            IErrorRepository errors)
        {
            _projectRepo = projectRepo;
            _attachmentRepo = attachmentRepo;
            _fileStorage = fileStorage;
            _errors = errors;
        }

        public async Task<ResponseDTO> CreateAsync(CreateEngineeringProjectDTO dto, int userId)
        {
            const string fn = nameof(CreateAsync);
            try
            {
                if (string.IsNullOrWhiteSpace(dto.ProjectName))
                    return new ResponseDTO { IsValid = false, Message = "ProjectName is required" };

                if (string.IsNullOrWhiteSpace(dto.ClientName))
                    return new ResponseDTO { IsValid = false, Message = "ClientName is required" };

                var project = new EngineeringProject
                {
                    ProjectName = dto.ProjectName.Trim(),
                    ClientName = dto.ClientName.Trim(),
                    ClientPhone = dto.ClientPhone?.Trim(),
                    ClientEmail = dto.ClientEmail?.Trim(),
                    ClientAddress = dto.ClientAddress?.Trim(),
                    Description = dto.Description?.Trim()
                };

                await _projectRepo.CreateAsync(project, userId);

                if (dto.Attachments != null)
                {
                    foreach (var file in dto.Attachments)
                    {
                        var saved = await _fileStorage.SaveFileAsync(file, $"EngineeringOffice/{project.Id}");
                        if (saved != null)
                        {
                            await _attachmentRepo.CreateAsync(new EngineeringProjectAttachment
                            {
                                EngineeringProjectId = project.Id,
                                FileName = saved.FileName,
                                FilePath = saved.RelativePath
                            }, userId);
                        }
                    }
                }

                return new ResponseDTO
                {
                    IsValid = true,
                    Data = project.Id,
                    Message = "Engineering project created"
                };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? string.Empty, userId);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error" };
            }
        }

        public async Task<ResponseDTO> UpdateAsync(UpdateEngineeringProjectDTO dto, int userId)
        {
            const string fn = nameof(UpdateAsync);
            try
            {
                var project = await _projectRepo.GetByIdWithAttachmentsAsync(dto.Id);
                if (project == null)
                    return new ResponseDTO { IsValid = false, Message = "Project not found" };

                if (string.IsNullOrWhiteSpace(dto.ProjectName))
                    return new ResponseDTO { IsValid = false, Message = "ProjectName is required" };

                if (string.IsNullOrWhiteSpace(dto.ClientName))
                    return new ResponseDTO { IsValid = false, Message = "ClientName is required" };

                project.ProjectName = dto.ProjectName.Trim();
                project.ClientName = dto.ClientName.Trim();
                project.ClientPhone = dto.ClientPhone?.Trim();
                project.ClientEmail = dto.ClientEmail?.Trim();
                project.ClientAddress = dto.ClientAddress?.Trim();
                project.Description = dto.Description?.Trim();

                // Remove attachments if requested
                if (dto.AttachmentIdsToRemove.Any())
                {
                    var attachmentsToRemove = project.Attachments
                        .Where(a => dto.AttachmentIdsToRemove.Contains(a.Id))
                        .ToList();

                    foreach (var attachment in attachmentsToRemove)
                    {
                        await _fileStorage.DeleteFileAsync(attachment.FilePath);
                        await _attachmentRepo.SoftDeleteAsync(attachment.Id, userId);
                    }
                }

                // Add new attachments
                if (dto.NewAttachments != null)
                {
                    foreach (var file in dto.NewAttachments)
                    {
                        var saved = await _fileStorage.SaveFileAsync(file, $"EngineeringOffice/{project.Id}");
                        if (saved != null)
                        {
                            await _attachmentRepo.CreateAsync(new EngineeringProjectAttachment
                            {
                                EngineeringProjectId = project.Id,
                                FileName = saved.FileName,
                                FilePath = saved.RelativePath
                            }, userId);
                        }
                    }
                }

                await _projectRepo.UpdateAsync(project, userId);

                return new ResponseDTO { IsValid = true, Message = "Engineering project updated" };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? string.Empty, userId);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error" };
            }
        }

        public async Task<ResponseDTO> DeleteAsync(int id, int userId)
        {
            const string fn = nameof(DeleteAsync);
            try
            {
                var project = await _projectRepo.GetByIdWithAttachmentsAsync(id);
                if (project == null)
                    return new ResponseDTO { IsValid = false, Message = "Project not found" };

                foreach (var attachment in project.Attachments)
                {
                    await _fileStorage.DeleteFileAsync(attachment.FilePath);
                    await _attachmentRepo.SoftDeleteAsync(attachment.Id, userId);
                }

                await _fileStorage.DeleteFolderAsync($"EngineeringOffice/{id}");
                await _projectRepo.SoftDeleteAsync(id, userId);

                return new ResponseDTO { IsValid = true, Message = "Engineering project deleted" };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? string.Empty, userId);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error" };
            }
        }

        public async Task<ResponseDTO> GetAsync(int id)
        {
            const string fn = nameof(GetAsync);
            try
            {
                var project = await _projectRepo.GetByIdWithAttachmentsAsync(id);
                if (project == null)
                    return new ResponseDTO { IsValid = false, Message = "Project not found" };

                var dto = Map(project);
                return new ResponseDTO { IsValid = true, Data = dto };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? string.Empty, 0);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error" };
            }
        }

        public async Task<ResponseDTO> GetAllAsync()
        {
            const string fn = nameof(GetAllAsync);
            try
            {
                var projects = await _projectRepo.GetAllWithAttachmentsAsync();
                var dtos = projects.Select(Map).ToList();
                return new ResponseDTO { IsValid = true, Data = dtos };
            }
            catch (Exception ex)
            {
                await _errors.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? string.Empty, 0);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error" };
            }
        }

        private static EngineeringProjectDTO Map(EngineeringProject project)
        {
            return new EngineeringProjectDTO
            {
                Id = project.Id,
                ProjectName = project.ProjectName,
                ClientName = project.ClientName,
                ClientPhone = project.ClientPhone,
                ClientEmail = project.ClientEmail,
                ClientAddress = project.ClientAddress,
                Description = project.Description,
                Attachments = project.Attachments
                    .Where(a => !a.IsDeleted)
                    .Select(a => new EngineeringProjectAttachmentDTO
                    {
                        Id = a.Id,
                        FileName = a.FileName,
                        FilePath = a.FilePath
                    }).ToList()
            };
        }
    }
}

using ERP.DTOs.Clients;
using ERP.Models.ClientsManagement;
using ERP.Repositories.Interfaces.Persons;
using ERP.Services.Interfaces.Persons;
using Microsoft.Extensions.Configuration;

public class ClientService : IClientService
{
    private readonly IClientRepository _clientRepository;
    private readonly IErrorRepository _errorRepository;
    private readonly string _storagePath;

    public ClientService(
        IClientRepository clientRepository,
        IErrorRepository errorRepository,
        IConfiguration configuration )
    {
        _clientRepository = clientRepository;
        _errorRepository = errorRepository;
        _storagePath = configuration["StoragePath"] ?? "wwwroot/uploads";

    }

    private async Task<(string relativePath, string originalName)> SaveFileAsync(IFormFile? file)
    {
        if (file == null || file.Length == 0) return (string.Empty, string.Empty);

        if (!Directory.Exists(_storagePath))
            Directory.CreateDirectory(_storagePath);

        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(_storagePath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return (Path.Combine("uploads", fileName).Replace("\\", "/"), file.FileName);
    }

    public async Task<ResponseDTO> CreateClient(CreateClientDTO dto, int createdBy)
    {
        const string fn = nameof(CreateClient);
        try
        {
            var (relativePath, originalName) = await SaveFileAsync(dto.ImageUrl);

            var client = new Client
            {
                Name = dto.Name,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                Address = dto.Address,
                ImageUrl = relativePath,
                OriginalImageName = originalName
            };

            var result = await _clientRepository.CreateAsync(client, createdBy);
            return new ResponseDTO { IsValid = true, Data = result, Message = "Client created successfully" };
        }
        catch (Exception ex)
        {
            await _errorRepository.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", createdBy);
            return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
        }
    }

    public async Task<ResponseDTO> EditClient(EditClientDTO dto, int updatedBy)
    {
        const string fn = nameof(EditClient);
        try
        {
            var client = await _clientRepository.GetByIdAsync(dto.Id);
            if (client == null)
                return new ResponseDTO { IsValid = false, Message = "Client not found" };

            if (dto.ImageUrl != null)
            {
                var (relativePath, originalName) = await SaveFileAsync(dto.ImageUrl);
                client.ImageUrl = relativePath;
                client.OriginalImageName = originalName;
            }

            client.Name = dto.Name;
            client.PhoneNumber = dto.PhoneNumber;
            client.Email = dto.Email;
            client.Address = dto.Address;

            var result = await _clientRepository.UpdateAsync(client, updatedBy);
            return new ResponseDTO { IsValid = result, Message = result ? "Client updated" : "Update failed" };
        }
        catch (Exception ex)
        {
            await _errorRepository.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", updatedBy);
            return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
        }
    }

    public async Task<ResponseDTO> GetClientAsync(int clientId)
    {
        const string fn = nameof(GetClientAsync);
        try
        {
            var client = await _clientRepository.GetByIdAsync(clientId);
            if (client == null)
                return new ResponseDTO { IsValid = false, Message = "Client not found" };

            var clientStatements = await _clientRepository.getClientStatements(clientId);

            var dto = new ClientDTO
            {
                Id = client.Id,
                Name = client.Name,
                PhoneNumber = client.PhoneNumber,
                Email = client.Email,
                Address = client.Address,
                ImageUrl = string.IsNullOrEmpty(client.ImageUrl) ? null : client.ImageUrl,
                OriginalImageName = client.OriginalImageName,
                ClientStatements = clientStatements.Select(c => new ClientStatementDTO
                {
                    ProjectId = c.ProjectId,
                    ProjectName = string.Empty,
                    Date = c.Date.ToString("yyyy-MM-dd"),
                    Description = c.Description,
                    Amount = c.Amount
                }).ToList()
            };

            return new ResponseDTO { IsValid = true, Data = dto };
        }
        catch (Exception ex)
        {
            await _errorRepository.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", 0);
            return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
        }
    }

    public async Task<ResponseDTO> GetAllClientsAsync()
    {
        const string fn = nameof(GetAllClientsAsync);
        try
        {
            var clients = await _clientRepository.GetAllAsync();

            var result = new List<ClientDTO>();
            foreach (var client in clients)
            {
                var clientStatements = await _clientRepository.getClientStatements(client.Id);

                result.Add(new ClientDTO
                {
                    Id = client.Id,
                    Name = client.Name,
                    PhoneNumber = client.PhoneNumber,
                    Email = client.Email,
                    Address = client.Address,
                    ImageUrl = string.IsNullOrEmpty(client.ImageUrl) ? null : client.ImageUrl,
                    OriginalImageName = client.OriginalImageName,
                    ClientStatements = clientStatements.Select(c => new ClientStatementDTO
                    {
                        ProjectId = c.ProjectId,
                        ProjectName = string.Empty,
                        Date = c.Date.ToString("yyyy-MM-dd"),
                        Description = c.Description,
                        Amount = c.Amount
                    }).ToList()
                });
            }

            return new ResponseDTO { IsValid = true, Data = result };
        }
        catch (Exception ex)
        {
            await _errorRepository.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? "", 0);
            return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
        }
    }
}

using ERP.DTOs;
using ERP.DTOs.PrivatePartnerships;
using ERP.Models.PrivatePartnerships;
using ERP.Repositories.Interfaces.PrivatePartnerships;
using ERP.Services.Interfaces.PrivatePartnerships;

namespace ERP.Services.PrivatePartnerships
{
    public class PrivatePartnershipService : IPrivatePartnershipService
    {
        private readonly IPrivatePartnershipProjectRepository _projectRepository;
        private readonly IPrivatePartnershipPartnerShareRepository _partnerShareRepository;
        private readonly IPrivatePartnershipTransactionRepository _transactionRepository;
        private readonly IErrorRepository _errorRepository;

        public PrivatePartnershipService(
            IPrivatePartnershipProjectRepository projectRepository,
            IPrivatePartnershipPartnerShareRepository partnerShareRepository,
            IPrivatePartnershipTransactionRepository transactionRepository,
            IErrorRepository errorRepository)
        {
            _projectRepository = projectRepository;
            _partnerShareRepository = partnerShareRepository;
            _transactionRepository = transactionRepository;
            _errorRepository = errorRepository;
        }

        public async Task<ResponseDTO> CreateProjectAsync(CreatePrivatePartnershipProjectDTO dto, int userId)
        {
            const string fn = nameof(CreateProjectAsync);
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Name))
                    return new ResponseDTO { IsValid = false, Message = "Project name is required." };

                if (dto.PartnerShares == null || !dto.PartnerShares.Any())
                    return new ResponseDTO { IsValid = false, Message = "At least one partner is required." };

                var percentageTotal = dto.PartnerShares.Sum(p => p.ContributionPercentage);
                if (Math.Abs(percentageTotal - 100) > 0.01)
                    return new ResponseDTO { IsValid = false, Message = "Partner percentages must add up to 100%." };

                if (dto.PartnerShares.Any(p => p.ContributionAmount < 0))
                    return new ResponseDTO { IsValid = false, Message = "Partner contributions cannot be negative." };

                var project = new PrivatePartnershipProject
                {
                    Name = dto.Name,
                    Description = dto.Description,
                };

                var shares = dto.PartnerShares.Select(p => new PrivatePartnershipPartnerShare
                {
                    PartnerName = p.PartnerName,
                    ContributionAmount = p.ContributionAmount,
                    ContributionPercentage = p.ContributionPercentage,
                }).ToList();

                var created = await _projectRepository.CreateProjectWithSharesAsync(project, shares, userId);

                return new ResponseDTO
                {
                    IsValid = true,
                    Data = created.Id,
                    Message = "Private partnership project created successfully."
                };
            }
            catch (Exception ex)
            {
                await _errorRepository.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? string.Empty, userId);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }

        public async Task<ResponseDTO> AddTransactionAsync(int projectId, CreatePrivatePartnershipTransactionDTO dto, int userId)
        {
            const string fn = nameof(AddTransactionAsync);
            try
            {
                var project = await _projectRepository.GetByIdAsync(projectId);
                if (project == null)
                    return new ResponseDTO { IsValid = false, Message = "Project not found." };

                if (dto.Amount <= 0)
                    return new ResponseDTO { IsValid = false, Message = "Amount must be greater than zero." };

                DateTime occurredAt = DateTime.UtcNow;
                if (!string.IsNullOrWhiteSpace(dto.OccurredAt))
                {
                    if (!DateTime.TryParse(dto.OccurredAt, out occurredAt))
                        return new ResponseDTO { IsValid = false, Message = "Invalid transaction date." };
                }

                if (dto.TransactionType == PrivatePartnershipTransactionType.PartnerWithdrawal && !dto.PartnerShareId.HasValue)
                    return new ResponseDTO { IsValid = false, Message = "Partner withdrawal requires a partner share identifier." };

                if (dto.TransactionType == PrivatePartnershipTransactionType.PartnerWithdrawal && dto.PartnerShareId.HasValue)
                {
                    var partnerShare = await _partnerShareRepository.GetByIdAsync(dto.PartnerShareId.Value);
                    if (partnerShare == null || partnerShare.ProjectId != projectId)
                        return new ResponseDTO { IsValid = false, Message = "Partner share not found for this project." };
                }

                var transaction = new PrivatePartnershipTransaction
                {
                    Amount = dto.Amount,
                    TransactionType = dto.TransactionType,
                    Note = dto.Note,
                    ProjectId = projectId,
                    PartnerShareId = dto.PartnerShareId,
                    OccurredAt = occurredAt,
                };

                await _transactionRepository.CreateAsync(transaction, userId);

                return new ResponseDTO { IsValid = true, Message = "Transaction recorded successfully." };
            }
            catch (Exception ex)
            {
                await _errorRepository.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? string.Empty, userId);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }

        public async Task<ResponseDTO> GetProjectSummaryAsync(int projectId)
        {
            const string fn = nameof(GetProjectSummaryAsync);
            try
            {
                var project = await _projectRepository.GetProjectWithDetailsAsync(projectId);
                if (project == null)
                    return new ResponseDTO { IsValid = false, Message = "Project not found." };

                var activeShares = project.PartnerShares.Where(p => !p.IsDeleted).ToList();
                var activeTransactions = project.Transactions.Where(t => !t.IsDeleted).ToList();

                var totalIncome = activeTransactions.Where(t => t.TransactionType == PrivatePartnershipTransactionType.Income).Sum(t => t.Amount);
                var totalExpenses = activeTransactions.Where(t => t.TransactionType == PrivatePartnershipTransactionType.Expense).Sum(t => t.Amount);
                var totalPartnerWithdrawals = activeTransactions.Where(t => t.TransactionType == PrivatePartnershipTransactionType.PartnerWithdrawal).Sum(t => t.Amount);

                var summary = new PrivatePartnershipProjectSummaryDTO
                {
                    Id = project.Id,
                    Name = project.Name,
                    Description = project.Description,
                    TotalContribution = activeShares.Sum(s => s.ContributionAmount),
                    TotalIncome = totalIncome,
                    TotalExpenses = totalExpenses,
                    TotalPartnerWithdrawals = totalPartnerWithdrawals,
                    NetProfitOrLoss = totalIncome - totalExpenses - totalPartnerWithdrawals,
                    Partners = activeShares.Select(s => new PrivatePartnerShareSummaryDTO
                    {
                        Id = s.Id,
                        PartnerName = s.PartnerName,
                        ContributionAmount = s.ContributionAmount,
                        ContributionPercentage = s.ContributionPercentage,
                        TotalWithdrawals = activeTransactions
                            .Where(t => t.TransactionType == PrivatePartnershipTransactionType.PartnerWithdrawal && t.PartnerShareId == s.Id)
                            .Sum(t => t.Amount)
                    }).ToList(),
                    Transactions = activeTransactions
                        .OrderByDescending(t => t.OccurredAt)
                        .Take(50)
                        .Select(t => new PrivatePartnershipTransactionViewDTO
                        {
                            Id = t.Id,
                            Amount = t.Amount,
                            TransactionType = t.TransactionType,
                            Note = t.Note,
                            OccurredAt = t.OccurredAt,
                            PartnerName = t.PartnerShare?.PartnerName ?? string.Empty,
                        }).ToList()
                };

                return new ResponseDTO { IsValid = true, Data = summary };
            }
            catch (Exception ex)
            {
                await _errorRepository.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? string.Empty, 0);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }

        public async Task<ResponseDTO> GetProjectsAsync()
        {
            const string fn = nameof(GetProjectsAsync);
            try
            {
                var projects = await _projectRepository.GetAllWithDetailsAsync();
                var data = projects.Select(project =>
                {
                    var activeShares = project.PartnerShares.Where(s => !s.IsDeleted).ToList();
                    var activeTransactions = project.Transactions.Where(t => !t.IsDeleted).ToList();

                    var totalIncome = activeTransactions.Where(t => t.TransactionType == PrivatePartnershipTransactionType.Income).Sum(t => t.Amount);
                    var totalExpenses = activeTransactions.Where(t => t.TransactionType == PrivatePartnershipTransactionType.Expense).Sum(t => t.Amount);
                    var totalWithdrawals = activeTransactions.Where(t => t.TransactionType == PrivatePartnershipTransactionType.PartnerWithdrawal).Sum(t => t.Amount);

                    return new PrivatePartnershipProjectSummaryDTO
                    {
                        Id = project.Id,
                        Name = project.Name,
                        Description = project.Description,
                        TotalContribution = activeShares.Sum(s => s.ContributionAmount),
                        TotalIncome = totalIncome,
                        TotalExpenses = totalExpenses,
                        TotalPartnerWithdrawals = totalWithdrawals,
                        NetProfitOrLoss = totalIncome - totalExpenses - totalWithdrawals,
                        Partners = activeShares.Select(s => new PrivatePartnerShareSummaryDTO
                        {
                            Id = s.Id,
                            PartnerName = s.PartnerName,
                            ContributionAmount = s.ContributionAmount,
                            ContributionPercentage = s.ContributionPercentage,
                            TotalWithdrawals = activeTransactions
                                .Where(t => t.TransactionType == PrivatePartnershipTransactionType.PartnerWithdrawal && t.PartnerShareId == s.Id)
                                .Sum(t => t.Amount)
                        }).ToList(),
                        Transactions = activeTransactions
                            .OrderByDescending(t => t.OccurredAt)
                            .Take(10)
                            .Select(t => new PrivatePartnershipTransactionViewDTO
                            {
                                Id = t.Id,
                                Amount = t.Amount,
                                TransactionType = t.TransactionType,
                                Note = t.Note,
                                OccurredAt = t.OccurredAt,
                                PartnerName = t.PartnerShare?.PartnerName ?? string.Empty,
                            }).ToList()
                    };
                }).ToList();

                return new ResponseDTO { IsValid = true, Data = data };
            }
            catch (Exception ex)
            {
                await _errorRepository.LogErrorAsync(ex.Message, fn, ex.StackTrace ?? string.Empty, 0);
                return new ResponseDTO { IsValid = false, Message = "Unexpected error happened" };
            }
        }
    }
}

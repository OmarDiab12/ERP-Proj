using ERP.Helpers;
using ERP.Helpers.JWT;
using ERP.Models;
using ERP.Repositories;
using ERP.Repositories.Interfaces.Persons;
using ERP.Repositories.Interfaces.ProjectsManagement;
using ERP.Repositories.Interfaces.QuotationManagement;
using ERP.Repositories.Interfaces.PrivatePartnerships;
using ERP.Repositories.Interfaces.EngineeringOffice;
using ERP.Repositories.Interfaces.Suppliers;
using ERP.Repositories.Interfaces.Inventory;
using ERP.Repositories.Interfaces.Invoices;
using ERP.Services.Interfaces.Notifications;
using ERP.Repositories.Persons;
using ERP.Repositories.ProjectManagement;
using ERP.Repositories.QuotationManagement;
using ERP.Repositories.PrivatePartnerships;
using ERP.Repositories.EngineeringOffice;
using ERP.Repositories.Suppliers;
using ERP.Repositories.Inventory;
using ERP.Repositories.Invoices;
using ERP.Services;
using ERP.Services.Interfaces.Persons;
using ERP.Services.Interfaces.ProjectManagement;
using ERP.Services.Interfaces.QuotationManagement;
using ERP.Services.Interfaces.PrivatePartnerships;
using ERP.Services.Interfaces;
using ERP.Services.EngineeringOffice;
using ERP.Services.Notifications;
using ERP.Services.Persons;
using ERP.Services.ProjectManagement;
using ERP.Services.QuotationManagement;
using ERP.Services.PrivatePartnerships;
using ERP.Services.Interfaces.Suppliers;
using ERP.Services.Interfaces.Inventory;
using ERP.Services.Interfaces.Invoices;
using ERP.Services.Suppliers;
using ERP.Services.Inventory;
using ERP.Services.Invoices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

#region Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ERP API",
        Version = "v1", // 🔧 THIS ensures OpenAPI version 3.x is generated
        Description = "ERP Backend API"
    });

    // ✅ JWT Bearer setup for Swagger UI "Authorize" button
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {token}'"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

#endregion

#region DB SQLServer
builder.Services.AddDbContext<ERPDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});
#endregion

#region JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSettings["Key"];
var jwtIssuer = jwtSettings["Issuer"];
var jwtAudience = jwtSettings["Audience"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

#endregion

#region CORS

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
#endregion

#region DI

//builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IJWTHelper, JWTHelper>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IErrorRepository, ErrorRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService,UserService>();
builder.Services.AddScoped<IPartnerRepository, PartnerRepository>();
builder.Services.AddScoped<IPartnerService, PartnerService>();
builder.Services.AddScoped<IBrokerRepository, BrokerRepository>();
builder.Services.AddScoped<IBrokerService, BrokerService>();
builder.Services.AddScoped<IContractorService, ContractorService>();
builder.Services.AddScoped<IContractorRepository, ContractorRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IOperationalExpensesService, OperationalExpensesService>();
builder.Services.AddScoped<IOperationalExpensesRepository, OperationalExpensesRepository>();
builder.Services.AddScoped<IPersonalLoanService, PersonalLoanService>();
builder.Services.AddScoped<IPersonalLoansRepository, PersonalLoansRepository>();
builder.Services.AddScoped<IQuotationItemRepository, QuotationItemRepository>();
builder.Services.AddScoped<IQuotationRepository, QuotationRepository>();
builder.Services.AddScoped<IQuotationService, QuotationService>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IProjectExpenseRepository, ProjectExpenseRepository>();
builder.Services.AddScoped<IProjectPaymentRepository, ProjectPaymentRepository>();
builder.Services.AddScoped<IProjectTaskRepository, ProjectTaskRepository>();
builder.Services.AddScoped<IProjectAttachmentRepository, ProjectAttachmentRepository>();
builder.Services.AddScoped<IProjectProfitShareRepository, ProjectProfitShareRepository>();
builder.Services.AddScoped<IQuotaionAttachementRepository , QuotaionAttachementRepository>();
builder.Services.AddScoped<IPrivatePartnershipProjectRepository, PrivatePartnershipProjectRepository>();
builder.Services.AddScoped<IPrivatePartnershipPartnerShareRepository, PrivatePartnershipPartnerShareRepository>();
builder.Services.AddScoped<IPrivatePartnershipTransactionRepository, PrivatePartnershipTransactionRepository>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();
builder.Services.AddScoped<IReportExportService, ReportExportService>();
builder.Services.AddScoped<ICreateService , CreateService>();
builder.Services.AddScoped<IGetService , GetService>();
builder.Services.AddScoped<IUpdateService , UpdateService>();
builder.Services.AddScoped<IPrivatePartnershipService, PrivatePartnershipService>();
builder.Services.AddScoped<IEngineeringProjectRepository, EngineeringProjectRepository>();
builder.Services.AddScoped<IEngineeringProjectAttachmentRepository, EngineeringProjectAttachmentRepository>();
builder.Services.AddScoped<IEngineeringOfficeService, EngineeringOfficeService>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAngular");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ERP.Hubs.NotificationHub>("/hubs/notifications");

app.Run();

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Application.Interfaces;
using Application.Services;
using DataAccess.Repositories;
using GraphqlAPI;
using GraphqlAPI.Middlewares;
using GraphqlAPI.Types;
using HotChocolate;
using HotChocolate.Authorization;

// Voeg hier usings toe voor HotChocolate

// Services
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => {
    options.AddPolicy("AllowFrontend", policy => {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var jwtKey = builder.Configuration["Jwt:Key"]!;
var jwtIssuer = builder.Configuration["Jwt:Issuer"]!;
var connectionString = builder.Configuration["ConnectionStrings:DefaultConnection"]!;

builder.Services.AddAuthentication(options => {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<IUserRepository>(provider => {
    var logger = provider.GetRequiredService<ILogger<UserRepository>>();
    return new UserRepository(connectionString, logger);
});

builder.Services.AddScoped<ITransactionRepository>(provider => {
    var logger = provider.GetRequiredService<ILogger<TransactionRepository>>();
    return new TransactionRepository(connectionString, logger);
});

builder.Services.AddScoped<ICategoryRepository>(provider => {
    var logger = provider.GetRequiredService<ILogger<CategoryRepository>>();
    return new CategoryRepository(connectionString, logger);
});

builder.Services.AddScoped<ISavingGoalRepository>(provider => {
    var logger = provider.GetRequiredService<ILogger<SavingGoalRepository>>();
    return new SavingGoalRepository(connectionString, logger);
});

builder.Services.AddScoped<IBudgetRepository>(provider => {
    var logger = provider.GetRequiredService<ILogger<BudgetRepository>>();
    return new BudgetRepository(connectionString, logger);
});

builder.Services.AddScoped<ITagRepository>(provider => {
    var logger = provider.GetRequiredService<ILogger<TagRepository>>();
    return new TagRepository(connectionString, logger);
});

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ISavingGoalService, SavingGoalService>();
builder.Services.AddScoped<IBudgetService, BudgetService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IUserService, UserService>();

// GraphQL Services
builder.Services
    .AddGraphQLServer()
    .AddAuthorization()
    .AddQueryType<Query>()
    // .AddMutationType<Mutation>()
    .AddType<TransactionType>();

var app = builder.Build();

if (app.Environment.IsDevelopment()) { }

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseMiddleware<ApiKeyMiddleware>();
app.UseAuthorization();

app.MapGraphQL();

app.Run();
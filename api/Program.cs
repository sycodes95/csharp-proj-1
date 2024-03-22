using api.Data;
using api.Interfaces;
using api.Models;
using api.Repository;
using api.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

// builder.Services.AddControllers(); It enables the framework to discover and activate controller classes as part of the application's request handling pipeline. Controllers are classes that handle incoming HTTP requests and return responses. Without this service registration, the application wouldn't know how to handle requests mapped to controllers.
builder.Services.AddControllers();

//In essence, AddEndpointsApiExplorer() sets the foundation for automatic API documentation generation and exploration capabilities in an ASP.NET Core application, making it a key part of developing, documenting, and testing APIs.
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

//Overall, this configuration is essential for applications that use complex object relationships and need to ensure their APIs can serialize data into JSON without errors caused by reference loops. It makes Newtonsoft.Json the default serializer for the application and configures it to gracefully handle one of the common pitfalls in JSON serialization.
builder.Services.AddControllers().AddNewtonsoftJson(options => {
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});

//Connects to MSSQL DB
builder.Services.AddDbContext<ApplicationDBContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


// The builder.Services.AddIdentity<AppUser, IdentityRole>(options => { ... }) line of code is configuring ASP.NET Core Identity in an application, specifying how the system should handle user authentication and authorization. ASP.NET Core Identity is a membership system that adds login functionality to ASP.NET Core apps, allowing for user registration, login, and management.AddEntityFrameworkStores<ApplicationDBContext>(): This method call tells ASP.NET Core Identity to use Entity Framework Core as its persistence mechanism, with ApplicationDBContext as the database context. ApplicationDBContext should extend IdentityDbContext and is responsible for connecting to the database and mapping Identity classes (such as users and roles) to database tables. By using Entity Framework Core, ASP.NET Core Identity can automatically create and manage the necessary database schema for storing user and role information.
builder.Services.AddIdentity<AppUser, IdentityRole>(options => {
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 12;
})
.AddEntityFrameworkStores<ApplicationDBContext>();

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = 
    options.DefaultChallengeScheme = 
    options.DefaultForbidScheme = 
    options.DefaultScheme = 
    options.DefaultSignInScheme = 
    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"])
        )
    };
});

builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPortfolioRepository, PortfolioRepository>();
builder.Services.AddScoped<IFMPService, FMPService>();
builder.Services.AddHttpClient<IFMPService, FMPService>();


var app = builder.Build(); 

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(x => x
.AllowAnyMethod()
.AllowAnyHeader()
.AllowCredentials()
//When deploying 
.WithOrigins("https://localhost:3000")
.SetIsOriginAllowed(origin => true)
);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();


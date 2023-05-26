global using System.Security.Claims;
global using TinderProject.Models.ModelEnums;
global using TinderProject.Models;
global using Microsoft.EntityFrameworkCore;
global using TinderProject.Data;
global using TinderProject.Repositories.Repositories_Interfaces;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using TinderProject.Repositories;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "Google";
})
.AddCookie(options =>
{
    // When a user logs in to Google for the first time, create a local account for that user in our database.
    options.Events.OnValidatePrincipal += async context =>
    {
        var serviceProvider = context.HttpContext.RequestServices;
        using var db = new AppDbContext(serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>());

        string subject = context.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
        string issuer = context.Principal.FindFirst(ClaimTypes.NameIdentifier).Issuer;
        //Utdaterad
        string name = context.Principal.FindFirst(ClaimTypes.Name).Value;

        //Detta är den nya
        var account = db.Users
            .FirstOrDefault(p => p.OpenIDIssuer == issuer && p.OpenIDSubject == subject);

        if (account == null)
        {
            account = new User
            {
                OpenIDIssuer = issuer,
                OpenIDSubject = subject,
                FirstName = name,
                LastName = " ",
                DateOfBirth = DateTime.UtcNow,
                Description = " ",
                Gender = GenderType.Other,
                ProfilePictureUrl = " ",
            };
            db.Users.Add(account);
        }

        await db.SaveChangesAsync();
    };
})

.AddOpenIdConnect("Google", options =>
{
    options.Authority = "https://accounts.google.com";

    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    options.ResponseType = OpenIdConnectResponseType.Code;
    options.CallbackPath = "/signin-oidc-google";
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");
    options.SaveTokens = true;
    options.GetClaimsFromUserInfoEndpoint = true;
});

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

//For the usage of Dependency Injection.
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IMatchRepository, MatchRepository>();
builder.Services.AddScoped<IAppDbContext, AppDbContext>();


//For the usage of Session variables.
//Setting the session variable to disappear after 30 minutes of idletime.
builder.Services.AddSession(options => options.IdleTimeout = TimeSpan.FromMinutes(30));

builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AccessControl>();
builder.Services.AddSingleton<FileRepository>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

Directory.CreateDirectory(builder.Configuration["Uploads:FolderPath"]);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, builder.Configuration["Uploads:FolderPath"])
    ),
    RequestPath = builder.Configuration["Uploads:URLPath"]
});
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.MapRazorPages();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    SampleData.CreateData(context);
}

app.Run();

global using System.Security.Claims;
global using TinderProject.Models.ModelEnums;
global using TinderProject.Models;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using TinderProject.Data;
using TinderProject.Repositories;
using TinderProject.Repositories.Repositories_Interfaces;

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
        string name = context.Principal.FindFirst(ClaimTypes.Name).Value;

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
                Gender = " ",
                ProfilePictureUrl = " ",
            };
            db.Users.Add(account);
        }
        else
        {
            // If the account already exists, just update the name in case it has changed.
            account.FirstName = name;
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

//For the usage of Session variables.
//Setting the session variable to disapear after 30mins of idletime.
builder.Services.AddSession(options => options.IdleTimeout = TimeSpan.FromMinutes(30));

builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AccessControl>();

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
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    SampleData.CreateData(context);
}

app.Run();

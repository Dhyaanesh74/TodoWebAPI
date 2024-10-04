using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using todowebapi.Configurations;
    using todowebapi.DataAnnotations;

    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddDbContext<TodoDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
    builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection(key:"JwtConfig"));

    builder.Services.AddAuthentication(Options =>{
         Options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
         Options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
         Options.DefaultChallengeScheme =  JwtBearerDefaults.AuthenticationScheme;

    })
    .AddJwtBearer(jwt =>{
        var key = Encoding.ASCII.GetBytes(builder.Configuration.GetSection(key:"JwtConfig:Secret").Value);
        jwt.SaveToken = true;
        jwt.TokenValidationParameters =  new TokenValidationParameters()
        {
             ValidateIssuerSigningKey = true,
             IssuerSigningKey =  new SymmetricSecurityKey(key),     
             ValidateIssuer = false,
             ValidateAudience = false,
             RequireExpirationTime = false,
             ValidateLifetime =  true
        };
    });

   builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<TodoDbContext>();
    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    
    app.AddGlobalErrorHandler();

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    
    app.MapControllers();
    app.Run();

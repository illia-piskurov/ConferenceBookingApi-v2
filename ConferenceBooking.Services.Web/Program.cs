using System.Reflection;
using ConferenceBooking.Bll;
using ConferenceBooking.Dal.SqlRepositories;
using ConferenceBooking.Services.Web.Mapping;
using ConferenceBooking.Services.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "ConferenceBookingApi",
        Version = "v1",
        Description = "API для управління бронюванням конференц-залів"
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<ServicesMappingProfile>());

builder.Services.AddBllServices();
builder.Services.AddSqlRepositories(builder.Configuration);

var app = builder.Build();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

var schema = builder.Configuration.GetValue<string>("DatabaseOptions:Schema") ?? "dbo";
SqlDatabaseMigrator.Migrate(connectionString, schema);

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "ConferenceBookingApi v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

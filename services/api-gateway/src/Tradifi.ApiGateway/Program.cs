using ApiGateway.Middlewares;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
string corsPolicy = builder.Configuration["CorsPolicy"];

// register the IhttpContextAccessor
builder.Services.AddHttpContextAccessor();
builder.AddCustomLogger();
builder.Services.InjectObjectLifeCycle();





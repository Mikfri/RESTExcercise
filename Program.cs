using BookLib;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
const string MyAllowedOrigin = "https://mf-swapper.azurewebsites.net/";

/// Denne kodeblok tillader ALLE at kunne lave �ndringer p� vores bogprojekt.
/// Vi benytter os pt af kodeblokken nedenunder..
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

/*Denne del s�rger for vi kun kan benytte os af CORS fra et bestemt site. 
Bem�rk vi beh�ver en `const string <variablenavn>` som peger p� det specifikke website
... og vi skal �ndre inholdet i `app.UseCors` argumentet til at vores current brugte policy name*/
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy(name: MyAllowedOrigin,
//        builder =>
//        {
//            builder.WithOrigins(MyAllowedOrigin)
//                    //.AllowAnyMethod()
//                    //.AllowAnyHeader();
//                    //ELLER
//                    .WithMethods("GET", "PUT")  // Tillad kun bestemte HTTP-metoder
//                    .WithHeaders("Content-Type");  // Tillad kun bestemte HTTP-overskrifter (f.eks. Content-Type)
//        });
//});

builder.Services.AddControllers();

//---: SWASHBUCKLE SETUP for SWAGGER :---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//END: SWASHBUCKLE SETUP for SWAGGER :---


builder.Services.AddSingleton(new BookRepository()); //Ny m�de! //builder.Services.AddSingleton<BookRepository>(new BookRepository()); //Gammel m�de

var app = builder.Build();
// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");
//app.UseCors(MyAllowedOrigin);  //Pr�v uden denne del.. Applier policy til hele applikationen?? S� n�r man applier en header er den allerede applied??

app.UseAuthorization();

app.MapControllers();

app.Run();

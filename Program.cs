using BookLib;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
const string MyAllowedOrigin = "https://mf-swapper.azurewebsites.net/";

/// Denne kodeblok tillader ALLE at kunne lave ændringer på vores bogprojekt.
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

/*Denne del sørger for vi kun kan benytte os af CORS fra et bestemt site. 
Bemærk vi behøver en `const string <variablenavn>` som peger på det specifikke website
... og vi skal ændre inholdet i `app.UseCors` argumentet til at vores current brugte policy name*/
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


builder.Services.AddSingleton(new BookRepository()); //Ny måde! //builder.Services.AddSingleton<BookRepository>(new BookRepository()); //Gammel måde

var app = builder.Build();
// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");
//app.UseCors(MyAllowedOrigin);  //Prøv uden denne del.. Applier policy til hele applikationen?? Så når man applier en header er den allerede applied??

app.UseAuthorization();

app.MapControllers();

app.Run();

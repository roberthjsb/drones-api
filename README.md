
**Proyecto Drones-API**
 1. Net core 3.1
 2. Test Xunit + Moq
 3. AutoMapper
 4. Entity Framework Core 

**Base de datos SQL server 2017**\
Cree la instancia y el esquema de la base de datos y edite el string de conexión en el appsetting.json.
Puede generar una base de datos usando docker-compose.
Para levantar la base de datos de la aplicación ejecutar 

`docker compose up -d`

**Compilar el proyecto**

`dotnet restore "drones-api/drones-api.csproj"` 

`dotnet build "drones-api.csproj" -c Release`
   
 - Instalar dotnet-ef para generar la base de datos por medio de las migraciones\
    `dotnet tool install --global dotnet-ef`

 - Generar la base datos\
    `dotnet-ef database update`

El proyecto contiene Swagger, así que podrá probarlo por medio de la interfaz de grafica de Swagger

`https://localhost:5001/swagger`




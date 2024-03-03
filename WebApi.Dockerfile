FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /App
COPY src/PedroTer7.Rinha2024Q1.Database ./PedroTer7.Rinha2024Q1.Database
COPY src/PedroTer7.Rinha2024Q1.WebApi ./PedroTer7.Rinha2024Q1.WebApi
ENV WEBAPP_PROJECT=./PedroTer7.Rinha2024Q1.WebApi
RUN dotnet restore ${WEBAPP_PROJECT}
RUN dotnet publish ${WEBAPP_PROJECT} -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /App
COPY --from=build /App/out .
ENV CUSTOMCONNSTR_MariaDbRead=""
ENV CUSTOMCONNSTR_MariaDbWrite=""
ENTRYPOINT ["dotnet", "PedroTer7.Rinha2024Q1.WebApi.dll"]
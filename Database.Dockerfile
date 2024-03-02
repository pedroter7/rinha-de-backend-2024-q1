FROM mariadb:11 as server
COPY ./src/PedroTer7.Rinha2024Q1.Database/Sql/* /docker-entrypoint-initdb.d/
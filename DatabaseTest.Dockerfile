FROM mariadb:11 as server
COPY ./src/PedroTer7.Rinha2024Q1.Database/Sql/1_database.sql /docker-entrypoint-initdb.d/
COPY ./src/PedroTer7.Rinha2024Q1.Database/Sql/2_structure.sql /docker-entrypoint-initdb.d/
COPY ./src/PedroTer7.Rinha2024Q1.Database/Sql/3_procedures.sql /docker-entrypoint-initdb.d/
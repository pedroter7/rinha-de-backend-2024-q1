FROM mariadb:11 as server
COPY mariadb.cnf /etc/mysql/conf.d/custom_mariadb.cnf
COPY ./src/PedroTer7.Rinha2024Q1.Database/Sql/* /docker-entrypoint-initdb.d/
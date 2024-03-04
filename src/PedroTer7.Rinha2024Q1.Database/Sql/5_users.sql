CREATE USER 'rinha_user'@'%' IDENTIFIED BY 'pwd';
GRANT ALL PRIVILEGES ON rinha_2024_q1.* TO 'rinha_user'@'%';

FLUSH PRIVILEGES;
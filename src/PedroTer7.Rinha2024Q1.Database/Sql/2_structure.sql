USE rinha_2024_q1;

CREATE TABLE account
(
    id INT PRIMARY KEY,
    `limit` INT UNSIGNED NOT NULL
);

CREATE TRIGGER create_account_cache
AFTER INSERT ON account
FOR EACH ROW
INSERT INTO account_balance_cache (account_id)
VALUES (NEW.Id);

CREATE TABLE account_transaction_log
(
    id INT AUTO_INCREMENT PRIMARY KEY,
    account_id INT NOT NULL,
    `type` TINYINT NOT NULL,
    amount INT NOT NULL,
    `description` VARCHAR(10) CHARACTER SET utf8 NOT NULL,
    timestamp_utc TIMESTAMP NOT NULL DEFAULT UTC_TIMESTAMP,
    FOREIGN KEY (account_id) REFERENCES account(id),
    INDEX (account_id)
);

CREATE TABLE account_balance_cache
(
    account_id INT NOT NULL UNIQUE,
    balance INT NOT NULL DEFAULT 0,
    FOREIGN KEY (account_Id) REFERENCES account(id),
    INDEX (account_id)
);
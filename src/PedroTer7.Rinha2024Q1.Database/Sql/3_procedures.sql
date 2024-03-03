USE rinha_2024_q1;

DELIMITER //

CREATE PROCEDURE transaction
(
    IN in_amount INT,
    IN in_account_id INT,
    IN in_type TINYINT,
    IN in_description VARCHAR(10),
    OUT out_code TINYINT,
    OUT out_balance INT,
    OUT out_limit INT
)
READS SQL DATA
MODIFIES SQL DATA
COMMENT '
    ABOUT:
    *  This procedure performs a transaction of any type.
    INPUTS:
    *  in_amount: transaction amount (signed)
    *  in_account_id: ID of the account performing the transaction
    *  in_type: ASCII decimal code for the transaction type
    *  in_description: transaction description
    OUTPUTS:
    *  out_code: int describing the operation result, see below
    *  out_balance (NULLABLE): account balance after this transaction happens
    *  out_limit (NULLABLE): account limit after this transaction happens
    OUTPUT CODES:
    *  0: everything went well
    *  1: account not found
    *  2: invalid transaction due to insufficient funds
	*  3: invalid arguments
'
transaction_proc:
BEGIN
	DECLARE current_balance INT;
	DECLARE account_limit INT;
	DECLARE new_balance INT;

	IF NOT((in_type = 99 AND in_amount > 0)
			OR (in_type = 100 AND in_amount < 0))
	THEN
		SET out_code = 3;
        LEAVE transaction_proc;
	END IF;

    START TRANSACTION;

    SELECT a.balance, a.`limit` 
	INTO current_balance, account_limit
    FROM account a
    WHERE a.id = in_account_id
    FOR UPDATE;

    IF current_balance IS NULL
	THEN
        ROLLBACK;
        SET out_code = 1;
        LEAVE transaction_proc;
    END IF;

	SET new_balance = current_balance + in_amount;

    IF new_balance < (-1) * account_limit
	THEN
		ROLLBACK;
		SET out_code = 2;
		LEAVE transaction_proc;
	END IF;

	INSERT INTO account_transaction_log
	(account_id, `type`, amount, `description`)
	VALUES
	(in_account_id, in_type, in_amount, in_description);

	UPDATE account
	SET balance = new_balance
	WHERE id = in_account_id;

	SELECT 0, new_balance, account_limit INTO out_code, out_balance, out_limit;
    COMMIT;
END//

CREATE PROCEDURE get_account_statement
(
	IN in_account_id INT,
	OUT out_code TINYINT,
	OUT out_current_balance INT,
	OUT out_current_limit INT,
	OUT out_statement_timestamp TIMESTAMP
)
READS SQL DATA
COMMENT '
	ABOUT:
	*	Builds a statement for a given account ID
	INPUTS:
	*	in_account_id: account ID to build the statement
	OUTPUTS:
	*	out_code: int describing the operation result, see below
	*	out_current_balance (NULLABLE): current account balance
	*	out_current_limit (NULLABLE): current account limit
	*	out_statement_timestamp: time when the resulting statement was generated in UTC time
	CALL OUTCOME:
		Up to the last 10 transactions for the given account. 
		See account_transaction_log table for results structure.
	OUTPUT CODES:
	*	0: everything went well 
	*	1: account not found
'
statement_proc:
BEGIN
	DECLARE account_balance INT;
	DECLARE account_limit INT;
	SET out_statement_timestamp = UTC_TIMESTAMP;

	SELECT a.balance, a.`limit` 
	INTO account_balance, account_limit
    FROM account a
    WHERE a.id = in_account_id;
   	
   	IF account_balance IS NULL
	THEN
        SET out_code = 1;
        LEAVE statement_proc;
    END IF;
   
   	SELECT atl.*
   	FROM account_transaction_log atl
   	WHERE atl.account_id = in_account_id
   	ORDER BY atl.id DESC
   	LIMIT 10;
   	
   	SELECT 0, account_balance, account_limit
   	INTO out_code, out_current_balance, out_current_limit;
END//


DELIMITER ;
CREATE TABLE user (
	id INT AUTO_INCREMENT PRIMARY KEY,
    email VARCHAR(255) NOT NULL,
    password VARCHAR(255) NOT NULL,
    first_name VARCHAR(255),
    last_name VARCHAR(255)
);

CREATE TABLE account_balance (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT,
    usd DECIMAL(18, 8) DEFAULT 0.0,
    btc DECIMAL(18, 8) DEFAULT 0.0,
    eth DECIMAL(18, 8) DEFAULT 0.0,
    doge DECIMAL(18, 8) DEFAULT 0.0,
    xrp DECIMAL(18, 8) DEFAULT 0.0,
    bnb DECIMAL(18, 8) DEFAULT 0.0,
    FOREIGN KEY (user_id) REFERENCES user(id)
);


CREATE TABLE crypto_price (
    symbol VARCHAR(10) NOT NULL,
    price DECIMAL(18, 8) NOT NULL
);



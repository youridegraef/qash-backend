CREATE TABLE user
(
    id            INTEGER PRIMARY KEY AUTOINCREMENT,
    name          TEXT        NOT NULL,
    email         TEXT UNIQUE NOT NULL,
    password_hash TEXT        NOT NULL,
    date_of_birth date        NOT NULL
);

CREATE TABLE transactions
(
    id      integer primary key autoincrement,
    amount  NUMERIC(10, 2) NOT NULL,
    date    DATE           NOT NULL,
    user_id INTEGER        NOT NULL,
    FOREIGN KEY (user_id) REFERENCES user (id)
);

CREATE TABLE tag
(
    id             INTEGER PRIMARY KEY AUTOINCREMENT,
    name           TEXT UNIQUE NOT NULL,
    color_hex_code TEXT        NOT NULL,
    user_id        INTEGER     NOT NULL,
    FOREIGN KEY (user_id) REFERENCES user (id)
);

CREATE TABLE saving_goal
(
    id       INTEGER PRIMARY KEY AUTOINCREMENT,
    name     TEXT           NOT NULL,
    TARGET   NUMERIC(10, 2) NOT NULL,
    deadline DATE           NOT NULL,
    user_id  INTEGER        NOT NULL,
    FOREIGN KEY (user_id) REFERENCES user (id)
);

CREATE TABLE category
(
    id      INTEGER PRIMARY KEY AUTOINCREMENT,
    name    TEXT    NOT NUlL UNIQUE,
    user_id INTEGER NOT NULL,
    FOREIGN KEY (user_id) REFERENCES user (id)
);

CREATE TABLE budget
(
    id          INTEGER PRIMARY KEY AUTOINCREMENT,
    start_date  date           NOT NULL,
    end_date    date           NOT NULL,
    budget      NUMERIC(10, 2) NOT NULL,
    category_id INTEGER        NOT NULL,
    FOREIGN KEY (category_id) REFERENCES category (id)
);

CREATE TABLE transaction_tag
(
    transaction_id INTEGER NOT NULL,
    tag_id INTEGER NOT NULL,
    FOREIGN KEY (transaction_id) REFERENCES transactions (id),
    FOREIGN KEY (tag_id) REFERENCES tag (id)
);
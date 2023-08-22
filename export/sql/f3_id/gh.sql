PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: godshelpinfo
CREATE TABLE godshelpinfo (
    GodsHelpInfoID INTEGER      PRIMARY KEY AUTOINCREMENT
                                UNIQUE
                                NOT NULL,
    GodsHelpAbilID VARCHAR (4)  NOT NULL,
    GodsHelpName   VARCHAR (30) NOT NULL
);

INSERT INTO godshelpinfo (GodsHelpInfoID, GodsHelpAbilID, GodsHelpName) VALUES (1, 'A04B', 'Gold');
INSERT INTO godshelpinfo (GodsHelpInfoID, GodsHelpAbilID, GodsHelpName) VALUES (2, 'A04F', 'Level Up');
INSERT INTO godshelpinfo (GodsHelpInfoID, GodsHelpAbilID, GodsHelpName) VALUES (3, 'A0DD', 'Stats');
INSERT INTO godshelpinfo (GodsHelpInfoID, GodsHelpAbilID, GodsHelpName) VALUES (4, 'A04C', 'Anti-Magic Potion');
INSERT INTO godshelpinfo (GodsHelpInfoID, GodsHelpAbilID, GodsHelpName) VALUES (5, 'A04D', 'Full Heal Potion');
INSERT INTO godshelpinfo (GodsHelpInfoID, GodsHelpAbilID, GodsHelpName) VALUES (6, 'A00U', 'Invulnerable Familiar');

COMMIT TRANSACTION;
PRAGMA foreign_keys = on;

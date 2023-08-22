PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: herostatinfo
CREATE TABLE herostatinfo (
    HeroStatInfoID INTEGER      NOT NULL
                                PRIMARY KEY AUTOINCREMENT
                                UNIQUE,
    HeroStatAbilID VARCHAR (4)  NOT NULL,
    HeroStatName   VARCHAR (45) NOT NULL
);

INSERT INTO herostatinfo (HeroStatInfoID, HeroStatAbilID, HeroStatName) VALUES (1, 'A02W', 'Strength +1');
INSERT INTO herostatinfo (HeroStatInfoID, HeroStatAbilID, HeroStatName) VALUES (2, 'A03W', 'Attack +4');
INSERT INTO herostatinfo (HeroStatInfoID, HeroStatAbilID, HeroStatName) VALUES (3, 'A0CJ', 'Prelati Spellbook Mana +2');
INSERT INTO herostatinfo (HeroStatInfoID, HeroStatAbilID, HeroStatName) VALUES (4, 'A03E', 'Intelligence +1');
INSERT INTO herostatinfo (HeroStatInfoID, HeroStatAbilID, HeroStatName) VALUES (5, 'A03Z', 'Mana Regen +1.5');
INSERT INTO herostatinfo (HeroStatInfoID, HeroStatAbilID, HeroStatName) VALUES (6, 'A03D', 'Agility +1');
INSERT INTO herostatinfo (HeroStatInfoID, HeroStatAbilID, HeroStatName) VALUES (7, 'A03X', 'Armor +2');
INSERT INTO herostatinfo (HeroStatInfoID, HeroStatAbilID, HeroStatName) VALUES (8, 'A04Y', 'Movement Speed +7');
INSERT INTO herostatinfo (HeroStatInfoID, HeroStatAbilID, HeroStatName) VALUES (9, 'A03Y', 'Health Regen +2');
INSERT INTO herostatinfo (HeroStatInfoID, HeroStatAbilID, HeroStatName) VALUES (10, 'A0A9', 'Gold Regen +2');

COMMIT TRANSACTION;
PRAGMA foreign_keys = on;

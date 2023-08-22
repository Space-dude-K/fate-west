PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: herotypename
CREATE TABLE herotypename (
    HeroTypeNameID INTEGER      NOT NULL
                                UNIQUE,
    FK_HeroTypeID  INT          NOT NULL
                                CONSTRAINT fk_HeroTypeName_HeroType REFERENCES herotype (HeroTypeID) ON DELETE NO ACTION
                                                                                                     ON UPDATE NO ACTION,
    Language       TEXT (65532) NOT NULL,
    HeroName       VARCHAR (20) NOT NULL,
    HeroTitle      VARCHAR (45) NOT NULL,
    PRIMARY KEY (
        HeroTypeNameID,
        FK_HeroTypeID
    )
);

INSERT INTO herotypename (HeroTypeNameID, FK_HeroTypeID, Language, HeroName, HeroTitle) VALUES (1, 1, 'EN', 'Saber (5th)', 'King of Knights');
INSERT INTO herotypename (HeroTypeNameID, FK_HeroTypeID, Language, HeroName, HeroTitle) VALUES (2, 4, 'EN', 'Caster (Extra)', 'A Tale for Somebody''s Sake');
INSERT INTO herotypename (HeroTypeNameID, FK_HeroTypeID, Language, HeroName, HeroTitle) VALUES (3, 5, 'EN', 'Avenger', 'All The World''s Evils');
INSERT INTO herotypename (HeroTypeNameID, FK_HeroTypeID, Language, HeroName, HeroTitle) VALUES (4, 16, 'EN', 'Lancer (5th)', 'Prince of Light');
INSERT INTO herotypename (HeroTypeNameID, FK_HeroTypeID, Language, HeroName, HeroTitle) VALUES (5, 18, 'EN', 'Caster (5th)', 'Witch of the Ancient Ages');
INSERT INTO herotypename (HeroTypeNameID, FK_HeroTypeID, Language, HeroName, HeroTitle) VALUES (6, 7, 'EN', 'Archer (5th)', 'Counter Guardian');
INSERT INTO herotypename (HeroTypeNameID, FK_HeroTypeID, Language, HeroName, HeroTitle) VALUES (7, 23, 'EN', 'Rider (5th)', 'Monsterous Queen');
INSERT INTO herotypename (HeroTypeNameID, FK_HeroTypeID, Language, HeroName, HeroTitle) VALUES (8, 24, 'EN', 'Assassin (5th)', 'Mysterious Swordsman');
INSERT INTO herotypename (HeroTypeNameID, FK_HeroTypeID, Language, HeroName, HeroTitle) VALUES (9, 11, 'EN', 'Berserker (5th)', 'Greatest Warrior of Greece');
INSERT INTO herotypename (HeroTypeNameID, FK_HeroTypeID, Language, HeroName, HeroTitle) VALUES (10, 26, 'EN', 'Saber Alter', 'Tainted King of Knights');
INSERT INTO herotypename (HeroTypeNameID, FK_HeroTypeID, Language, HeroName, HeroTitle) VALUES (11, 10, 'EN', 'True Assassin', 'Genesis of the Middle East');
INSERT INTO herotypename (HeroTypeNameID, FK_HeroTypeID, Language, HeroName, HeroTitle) VALUES (12, 13, 'EN', 'Archer (4th)', 'King of Heroes');
INSERT INTO herotypename (HeroTypeNameID, FK_HeroTypeID, Language, HeroName, HeroTitle) VALUES (13, 6, 'EN', 'Rider (4th)', 'Conqueror of Macedonia');
INSERT INTO herotypename (HeroTypeNameID, FK_HeroTypeID, Language, HeroName, HeroTitle) VALUES (14, 25, 'EN', 'Caster (Extra)', 'Nine-Tailed Fox');
INSERT INTO herotypename (HeroTypeNameID, FK_HeroTypeID, Language, HeroName, HeroTitle) VALUES (15, 21, 'EN', 'Saber (Extra)', 'Knight of the Sun');
INSERT INTO herotypename (HeroTypeNameID, FK_HeroTypeID, Language, HeroName, HeroTitle) VALUES (16, 29, 'EN', 'Rider (Extra)', 'Voyager of the Storm');
INSERT INTO herotypename (HeroTypeNameID, FK_HeroTypeID, Language, HeroName, HeroTitle) VALUES (17, 22, 'EN', 'Assassin (Extra)', 'Master of Bajiquan');
INSERT INTO herotypename (HeroTypeNameID, FK_HeroTypeID, Language, HeroName, HeroTitle) VALUES (18, 12, 'EN', 'Lancer (Extra)', 'Innocent Monster');
INSERT INTO herotypename (HeroTypeNameID, FK_HeroTypeID, Language, HeroName, HeroTitle) VALUES (19, 28, 'EN', 'Berserker (Extra)', 'Traitorous General');
INSERT INTO herotypename (HeroTypeNameID, FK_HeroTypeID, Language, HeroName, HeroTitle) VALUES (20, 8, 'EN', 'Archer (Extra)', 'Faceless King');
INSERT INTO herotypename (HeroTypeNameID, FK_HeroTypeID, Language, HeroName, HeroTitle) VALUES (21, 27, 'EN', 'Saber (Extra)', 'Origin of the Flames');
INSERT INTO herotypename (HeroTypeNameID, FK_HeroTypeID, Language, HeroName, HeroTitle) VALUES (22, 19, 'EN', 'Lancer (Extra)', 'Merciful Saint');
INSERT INTO herotypename (HeroTypeNameID, FK_HeroTypeID, Language, HeroName, HeroTitle) VALUES (23, 20, 'EN', 'Berserkser (4th)', 'Knight of the Lake');
INSERT INTO herotypename (HeroTypeNameID, FK_HeroTypeID, Language, HeroName, HeroTitle) VALUES (24, 9, 'EN', 'Lancer (4th)', 'First Spear of the Fianna Knights');
INSERT INTO herotypename (HeroTypeNameID, FK_HeroTypeID, Language, HeroName, HeroTitle) VALUES (25, 30, 'EN', 'Caster (4th)', 'BlueBeard');
INSERT INTO herotypename (HeroTypeNameID, FK_HeroTypeID, Language, HeroName, HeroTitle) VALUES (26, 31, 'EN', 'Lancer (Extra)', 'Dragon''s Daughter');

COMMIT TRANSACTION;
PRAGMA foreign_keys = on;

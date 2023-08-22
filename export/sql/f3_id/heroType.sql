PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: herotype
CREATE TABLE herotype (
    HeroTypeID     INTEGER     PRIMARY KEY AUTOINCREMENT
                               UNIQUE
                               NOT NULL,
    HeroUnitTypeID VARCHAR (4) NOT NULL
);

INSERT INTO herotype (HeroTypeID, HeroUnitTypeID) VALUES (1, 'H000');
INSERT INTO herotype (HeroTypeID, HeroUnitTypeID) VALUES (4, 'H024');
INSERT INTO herotype (HeroTypeID, HeroUnitTypeID) VALUES (5, 'H028');
INSERT INTO herotype (HeroTypeID, HeroUnitTypeID) VALUES (6, 'H00A');
INSERT INTO herotype (HeroTypeID, HeroUnitTypeID) VALUES (7, 'H001');
INSERT INTO herotype (HeroTypeID, HeroUnitTypeID) VALUES (8, 'H01T');
INSERT INTO herotype (HeroTypeID, HeroUnitTypeID) VALUES (9, 'H04D');
INSERT INTO herotype (HeroTypeID, HeroUnitTypeID) VALUES (10, 'H008');
INSERT INTO herotype (HeroTypeID, HeroUnitTypeID) VALUES (11, 'H006');
INSERT INTO herotype (HeroTypeID, HeroUnitTypeID) VALUES (12, 'H01F');
INSERT INTO herotype (HeroTypeID, HeroUnitTypeID) VALUES (13, 'H009');
INSERT INTO herotype (HeroTypeID, HeroUnitTypeID) VALUES (16, 'H002');
INSERT INTO herotype (HeroTypeID, HeroUnitTypeID) VALUES (18, 'H004');
INSERT INTO herotype (HeroTypeID, HeroUnitTypeID) VALUES (19, 'H021');
INSERT INTO herotype (HeroTypeID, HeroUnitTypeID) VALUES (20, 'H03M');
INSERT INTO herotype (HeroTypeID, HeroUnitTypeID) VALUES (21, 'H00I');
INSERT INTO herotype (HeroTypeID, HeroUnitTypeID) VALUES (22, 'H01A');
INSERT INTO herotype (HeroTypeID, HeroUnitTypeID) VALUES (23, 'H003');
INSERT INTO herotype (HeroTypeID, HeroUnitTypeID) VALUES (24, 'H005');
INSERT INTO herotype (HeroTypeID, HeroUnitTypeID) VALUES (25, 'H00E');
INSERT INTO herotype (HeroTypeID, HeroUnitTypeID) VALUES (26, 'H007');
INSERT INTO herotype (HeroTypeID, HeroUnitTypeID) VALUES (27, 'H01X');
INSERT INTO herotype (HeroTypeID, HeroUnitTypeID) VALUES (28, 'H01Q');
INSERT INTO herotype (HeroTypeID, HeroUnitTypeID) VALUES (29, 'H00Y');
INSERT INTO herotype (HeroTypeID, HeroUnitTypeID) VALUES (30, 'E002');
INSERT INTO herotype (HeroTypeID, HeroUnitTypeID) VALUES (31, 'H02W');

COMMIT TRANSACTION;
PRAGMA foreign_keys = on;

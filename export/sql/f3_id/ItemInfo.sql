PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: iteminfo
CREATE TABLE iteminfo (
    ItemID     INT          NOT NULL
                            PRIMARY KEY,
    ItemTypeID VARCHAR (4)  NOT NULL,
    ItemName   VARCHAR (45) NOT NULL,
    ItemCost   INT          NOT NULL
);

INSERT INTO iteminfo (ItemID, ItemTypeID, ItemName, ItemCost) VALUES (1, 'I00H', 'Blink Scroll', 100);
INSERT INTO iteminfo (ItemID, ItemTypeID, ItemName, ItemCost) VALUES (2, 'I00E', 'Blink Scroll 1.5x', 150);
INSERT INTO iteminfo (ItemID, ItemTypeID, ItemName, ItemCost) VALUES (3, 'I01A', 'Combination Scroll', 300);
INSERT INTO iteminfo (ItemID, ItemTypeID, ItemName, ItemCost) VALUES (4, 'I019', 'Combination Scroll 1.5x', 450);
INSERT INTO iteminfo (ItemID, ItemTypeID, ItemName, ItemCost) VALUES (5, 'I011', 'Dust of Navigation', 1500);
INSERT INTO iteminfo (ItemID, ItemTypeID, ItemName, ItemCost) VALUES (6, 'I012', 'Dust of Navigation 1.5x', 2250);
INSERT INTO iteminfo (ItemID, ItemTypeID, ItemName, ItemCost) VALUES (7, 'I002', 'Familiar', 300);
INSERT INTO iteminfo (ItemID, ItemTypeID, ItemName, ItemCost) VALUES (8, 'I005', 'Familiar 1.5x', 450);
INSERT INTO iteminfo (ItemID, ItemTypeID, ItemName, ItemCost) VALUES (9, 'I00A', 'Gem of Speed', 300);
INSERT INTO iteminfo (ItemID, ItemTypeID, ItemName, ItemCost) VALUES (10, 'I00D', 'Gem of Speed 1.5x', 450);
INSERT INTO iteminfo (ItemID, ItemTypeID, ItemName, ItemCost) VALUES (11, 'I00M', 'Mass Teleportation Scroll', 1500);
INSERT INTO iteminfo (ItemID, ItemTypeID, ItemName, ItemCost) VALUES (12, 'I00X', 'Mass Teleportation Scroll 1.5x', 2250);
INSERT INTO iteminfo (ItemID, ItemTypeID, ItemName, ItemCost) VALUES (13, 'I000', 'Rank C Magic Scroll', 150);
INSERT INTO iteminfo (ItemID, ItemTypeID, ItemName, ItemCost) VALUES (14, 'I00T', 'Rank C Magic Scroll 1.5x', 225);
INSERT INTO iteminfo (ItemID, ItemTypeID, ItemName, ItemCost) VALUES (15, 'I003', 'Sentry Ward', 900);
INSERT INTO iteminfo (ItemID, ItemTypeID, ItemName, ItemCost) VALUES (16, 'I00N', 'Sentry Ward 1.5x', 1350);
INSERT INTO iteminfo (ItemID, ItemTypeID, ItemName, ItemCost) VALUES (17, 'I00R', 'Spirit Link Scroll', 1500);
INSERT INTO iteminfo (ItemID, ItemTypeID, ItemName, ItemCost) VALUES (18, 'I00G', 'Spirit Link Scroll 1.5x', 2250);
INSERT INTO iteminfo (ItemID, ItemTypeID, ItemName, ItemCost) VALUES (19, 'I00B', 'Teleportation Scroll', 500);
INSERT INTO iteminfo (ItemID, ItemTypeID, ItemName, ItemCost) VALUES (20, 'I00U', 'Teleportation Scroll 1.5x', 750);
INSERT INTO iteminfo (ItemID, ItemTypeID, ItemName, ItemCost) VALUES (21, 'I00I', 'Red Potion', 800);
INSERT INTO iteminfo (ItemID, ItemTypeID, ItemName, ItemCost) VALUES (22, 'I00F', 'Red Potion', 1200);
INSERT INTO iteminfo (ItemID, ItemTypeID, ItemName, ItemCost) VALUES (23, 'vamp', 'Berserk Potion', 800);
INSERT INTO iteminfo (ItemID, ItemTypeID, ItemName, ItemCost) VALUES (24, 'I00W', 'Berserk Potion 1.5x', 1200);
INSERT INTO iteminfo (ItemID, ItemTypeID, ItemName, ItemCost) VALUES (25, 'SWAR', 'Sentry Ward (Familiar)', 800);

COMMIT TRANSACTION;
PRAGMA foreign_keys = on;

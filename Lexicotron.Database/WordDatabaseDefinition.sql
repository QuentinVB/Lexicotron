PRAGMA foreign_keys = ON;

DROP TABLE IF EXISTS `word`;

CREATE TABLE IF NOT EXISTS `word` 
(
    `wordid` INTEGER PRIMARY KEY NOT NULL,
    `word` TEXT UNIQUE NOT NULL,
    `synsetId` TEXT NULL,
    `creationDate` TEXT NULL
);

INSERT INTO `word` (`word`,`synsetId`, `creationDate` ) VALUES( 'test',	'b0854164fe',date('now'));

DROP TABLE IF EXISTS `relation`;

CREATE TABLE IF NOT EXISTS `relation` 
(
    `relationid` INTEGER PRIMARY KEY NOT NULL,
    `wordSource` INT UNIQUE NOT NULL,
    `relationGroup` TEXT NOT NULL,
    `synsetId` TEXT NOT NULL,
    `wordTarget` INT UNIQUE NULL,
    `creationDate` TEXT NULL,
    FOREIGN KEY(wordSource) REFERENCES word(wordid) ON DELETE SET NULL ON UPDATE CASCADE,
    FOREIGN KEY(wordTarget) REFERENCES word(wordid) ON DELETE SET NULL ON UPDATE CASCADE
);



INSERT INTO `relation` (`wordSource`,`relationGroup`,`synsetId`, `wordTarget`,`creationDate` ) VALUES( 1,	'synonym','b:zfiuzofiu5',1,date('now'));

/*

.mode column;
.headers on;

*/
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

.mode column
.headers on 
;

*/

DROP TABLE IF EXISTS `babellog`
CREATE TABLE IF NOT EXISTS `babellog` 
(
    `id` INTEGER PRIMARY KEY NOT NULL,
    `requestDateTime` TEXT NOT NULL,
    `synsetRequested` TEXT NOT NULL,
    `jsonReturned` TEXT NOT NULL 
);


INSERT INTO `babellog` (`requestDateTime`,`synsetRequested`,`jsonReturned` ) VALUES(datetime('now'), "n:fezf45475","{'test':'valuetest'}")

SELECT date(`requestDateTime`) as date, count(`requestDateTime`) as count FROM `babellog` WHERE date(`requestDateTime`) = date('now') GROUP BY date(`requestDateTime`);

SELECT date(`requestDateTime`) FROM `babellog` WHERE date(`requestDateTime`) = date('now');
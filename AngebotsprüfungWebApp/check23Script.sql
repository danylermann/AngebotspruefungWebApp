SELECT * FROM quote_evaluation;

SELECT * FROM client;

SELECT *FROM requirement;

SELECT * FROM solution;

SELECT * FROM activity_log;

SELECT * FROM estimation;

SELECT * FROM log_entry_requirement; 

SELECT * FROM folder;

SELECT * FROM file;

SELECT * FROM user;

SELECT * FROM access_group;

SELECT * FROM area_of_responsibility;

SELECT * FROM user_has_area_of_responsibility;

SELECT EXISTS(SELECT name FROM check23.user WHERE name = "dlermann");

SELECT area_of_responsibility.id FROM area_of_responsibility
JOIN user_has_area_of_responsibility ON area_of_responsibility.id = user_has_area_of_responsibility.area_of_responsibility_id
JOIN user ON user_has_area_of_responsibility.user_id = user.id
WHERE user.id = 2;

SELECT email FROM check23.user
JOIN check23.user_has_area_of_responsibility ON user.id = user_has_area_of_responsibility.user_id
JOIN check23.area_of_responsibility ON user_has_area_of_responsibility.area_of_responsibility_id = area_of_responsibility.id
WHERE area_of_responsibility.id = 1;

SELECT * FROM check23.area_of_responsibility WHERE id > 1;

SELECT * FROM check23.quote_evaluation
JOIN requirement ON requirement.quote_evaluation_id = quote_evaluation.id
WHERE requirement.name LIKE "%test%";

ALTER TABLE quote_evaluation ADD fulltext index (name,creator,legal_guidelines,external_contact);
SHOW INDEXES FROM quote_evaluation;
SELECT * FROM quote_evaluation WHERE MATCH(name,creator,legal_guidelines,external_contact) AGAINST ("an*" IN BOOLEAN MODE);

ALTER TABLE client ADD fulltext index client_index (name,location);
SHOW INDEXES FROM client;
SELECT * FROM check23.client WHERE MATCH(name,location) AGAINST ("herbert* helmut*
+" IN BOOLEAN MODE);
ALTER TABLE client DROP INDEX client_index;

SELECT quote_evaluation.id FROM check23.quote_evaluation
JOIN requirement ON quote_evaluation.id = requirement.quote_evaluation_id
WHERE EHW_construction OR EHW_HV_Tester OR EHW_internel_tools OR EHW_LV_Tester OR EHW_other OR EHW_TPMs;

SELECT activity_log.id FROM check23.activity_log 
JOIN quote_evaluation ON activity_log.quote_evaluation_id = quote_evaluation.id 
JOIN requirement ON quote_evaluation.id = requirement.quote_evaluation_id 
WHERE requirement.id = 1;

SELECT person, date, activity
FROM log_entry_quote_evaluation
Where activity_log_id = 1
UNION
SELECT person, date, activity
FROM log_entry_requirement
Where activity_log_id = 1
UNION
SELECT person, date, activity
FROM log_entry_solution
Where activity_log_id = 1
UNION
SELECT person, date, activity
FROM log_entry_estimation
Where activity_log_id = 1
ORDER BY date;

SELECT * FROM quote_evaluation WHERE id IN (7,2,3,6,9) ORDER BY date ASC;

SELECT folder.id FROM check23.folder WHERE folder.quote_evaluation_id = 14 AND folder.name = "quick test14";
SELECT * FROM folder;

SELECT * FROM quote_evaluation
WHERE name LIKE ("%test%")
OR name LIKE ("%mit%");


SELECT * FROM quote_evaluation
WHERE name LIKE ("%test%mit%");

SELECT * FROM quote_evaluation
WHERE name LIKE "%mit%"
OR creator LIKE "%mit%"
OR legal_guidelines LIKE "%mit%"
OR external_contact LIKE "%mit%"
OR name LIKE "%kein%"
OR creator LIKE "%kein%"
OR legal_guidelines LIKE "%kein%"
OR external_contact LIKE "%kein%";

SELECT id FROM check23.quote_evaluation WHERE name LIKE @searchTerm0 OR creator LIKE @searchTerm0 OR legal_guidelines LIKE @searchTerm0 OR external_contact LIKE @searchTerm0 OR name LIKE @searchTerm1 OR creator LIKE @searchTerm1 OR legal_guidelines LIKE @searchTerm1 OR external_contact LIKE @searchTerm1;

SELECT EXISTS(SELECT name FROM check23.quote_evaluation WHERE name = "test");

ALTER TABLE account RENAME user;

ALTER TABLE estimation
ADD Service_time varchar(1000);

ALTER TABLE estimation
ADD Service_cost varchar(1000);

INSERT INTO check23.log_entry_quote_evaluation (person, date, activity, quote_evaluation_id, activity_log_id)VALUES ("dlermann", '2022-04-22 10:34:23', "Created new quote evaluation", 5, 2) ;
INSERT INTO check23.log_entry_requirement (person, date, activity, requirement_id, activity_log_id) VALUES ( 'dlermann', '1000-01-01 00:00:00', "test insert", 12, 13);
INSERT INTO check23.access_group (name, create_user, create_access_group, create_client, create_quote_evaluation, create_requirement, create_solution, create_estimation) VALUES ("admin", true, true, true, true, true, true, true);
INSERT INTO check23.user (name, email, access_group_id) VALUES ("dlermann", "dlermann@weetech.com", 1);
INSERT INTO check23.user (name, email, access_group_id) VALUES ("mhartrich", "mhartrich@weetech.com", 1);
INSERT INTO check23.access_group (id, name, create_user, create_access_group, create_client, create_quote_evaluation, create_requirement, create_solution, create_estimation) VALUES (0, "default_user", false, false, false, false, false, false, false);
INSERT INTO check23.user_has_area_of_responsibility (user_id, area_of_responsibility_id) VALUES (1, 1);
INSERT INTO check23.user_has_area_of_responsibility (user_id, area_of_responsibility_id) VALUES (2, 1);
INSERT INTO check23.area_of_responsibility (name) VALUES ("Admin");
INSERT INTO check23.area_of_responsibility (name) VALUES ("ESW CEETIS");
INSERT INTO check23.area_of_responsibility (name) VALUES ("ESW IVISionStudio");
INSERT INTO check23.area_of_responsibility (name) VALUES ("ESW Netstar");
INSERT INTO check23.area_of_responsibility (name) VALUES ("ESW Interne Tools");
INSERT INTO check23.area_of_responsibility (name) VALUES ("ESW Other");
INSERT INTO check23.area_of_responsibility (name) VALUES ("EHW HV Tester");
INSERT INTO check23.area_of_responsibility (name) VALUES ("EHW Konstruktion");
INSERT INTO check23.area_of_responsibility (name) VALUES ("EHW TPMs");
INSERT INTO check23.area_of_responsibility (name) VALUES ("EHW LV Tester");
INSERT INTO check23.area_of_responsibility (name) VALUES ("EHW Interne Tools");
INSERT INTO check23.area_of_responsibility (name) VALUES ("EHW Other");
INSERT INTO check23.area_of_responsibility (name) VALUES ("Dokumentation");
INSERT INTO check23.area_of_responsibility (name) VALUES ("Service");
INSERT INTO check23.user_has_area_of_responsibility VALUES (2, 2);
INSERT INTO check23.area_of_responsibility (name) VALUES ("Vertrieb");

UPDATE check23.user SET accessgroup_id = 1 WHERE id = 1;
UPDATE check23.access_group SET id = 0 WHERE name = "default_user";
UPDATE check23.user SET access_group_id = 1 WHERE name = "dlermann";
UPDATE check23.user SET email = "dlermann@weetech.com" WHERE name = "dlermann";
UPDATE check23.user SET email = "mhartrich@weetech.com" WHERE id = 2;
UPDATE check23.area_of_responsibility SET name = "Admin" WHERE id = 1;

DELETE FROM check23.user_has_area_of_responsibility WHERE user_id = 1 and area_of_responsibility_id = 1;
DELETE FROM check23.area_of_responsibility WHERE id = 3;
DELETE FROM check23.user_has_area_of_responsibility WHERE user_id = 3;
DELETE FROM check23.access_group WHERE id = 6;
DELETE FROM check23.user WHERE id = 3;

DROP TABLE check23.folder;
DROP TABLE check23.file;
ALTER TABLE check23.user_has_area_of_responsibility DROP COLUMN checkcol;
ALTER TABLE check23.access_group AUTO_INCREMENT = 2;
ALTER TABLE check23.user AUTO_INCREMENT = 3;

CREATE TABLE IF NOT EXISTS `check23`.`comment_solution` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `creator` VARCHAR(1000) NOT NULL,
  `date` DATETIME NOT NULL,
  `message` VARCHAR(1000) NOT NULL,
  `solution_id` INT NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_KommentarLösung_Lösung1_idx` (`solution_id` ASC) VISIBLE,
  CONSTRAINT `fk_KommentarLösung_Lösung1`
    FOREIGN KEY (`solution_id`)
    REFERENCES `check23`.`solution` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;

CREATE TABLE IF NOT EXISTS `check23`.`comment_requirement` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `creator` VARCHAR(1000) NOT NULL,
  `date` DATETIME NOT NULL,
  `message` VARCHAR(1000) NOT NULL,
  `requirement_id` INT NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_KommentarAufgabe_Aufgabe1_idx` (`requirement_id` ASC) VISIBLE,
  CONSTRAINT `fk_KommentarAnforderung_Anforderung1`
    FOREIGN KEY (`requirement_id`)
    REFERENCES `check23`.`requirement` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;

CREATE TABLE IF NOT EXISTS `check23`.`folder` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(100) NOT NULL,
  `folder_path` VARCHAR(1000) NOT NULL,
  `quote_evaluation_id` INT NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_folder_quote_evaluation1_idx` (`quote_evaluation_id` ASC) VISIBLE,
  CONSTRAINT `fk_folder_quote_evaluation1`
    FOREIGN KEY (`quote_evaluation_id`)
    REFERENCES `check23`.`quote_evaluation` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;

CREATE TABLE IF NOT EXISTS `check23`.`file` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(100) NOT NULL,
  `file_path` VARCHAR(1000) NOT NULL,
  `folder_id` INT NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_file_folder1_idx` (`folder_id` ASC) VISIBLE,
  CONSTRAINT `fk_file_folder1`
    FOREIGN KEY (`folder_id`)
    REFERENCES `check23`.`folder` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;

CREATE TABLE IF NOT EXISTS `check23`.`estimation` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `ESW_time` VARCHAR(1000) NULL,
  `ESW_cost` VARCHAR(1000) NULL,
  `EHW_time` VARCHAR(1000) NULL,
  `EHW_cost` VARCHAR(1000) NULL,
  `CDE_time` VARCHAR(1000) NULL,
  `CDE_cost` VARCHAR(1000) NULL,
  `Documentation_time` VARCHAR(1000) NULL,
  `Documentation_cost` VARCHAR(1000) NULL,
  `Service_time` VARCHAR(1000) NULL,
  `Service_cost` VARCHAR(1000) NULL,
  `solution_id` INT NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_Abschätzung_Lösung1_idx` (`solution_id` ASC) VISIBLE,
  CONSTRAINT `fk_Abschätzung_Lösung1`
    FOREIGN KEY (`solution_id`)
    REFERENCES `check23`.`solution` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;

CREATE TABLE IF NOT EXISTS `check23`.`ticket` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `number` VARCHAR(1000) NOT NULL,
  `type` VARCHAR(45) NOT NULL,
  `quote_evaluation_id` INT NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_ticket_quote_evaluation1_idx` (`quote_evaluation_id` ASC) VISIBLE,
  CONSTRAINT `fk_ticket_quote_evaluation1`
    FOREIGN KEY (`quote_evaluation_id`)
    REFERENCES `check23`.`quote_evaluation` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;

CREATE TABLE IF NOT EXISTS `check23`.`accessgroup` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(100) NOT NULL,
  `create_user` TINYINT NOT NULL DEFAULT 0,
  `create_accessgroup` TINYINT NOT NULL DEFAULT 0,
  `create_client` TINYINT NOT NULL DEFAULT 0,
  `create_quote_evaluation` TINYINT NOT NULL DEFAULT 0,
  `create_requirement` TINYINT NOT NULL DEFAULT 0,
  `create_solution` TINYINT NOT NULL DEFAULT 0,
  `create_estimation` TINYINT NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `id_UNIQUE` (`id` ASC) VISIBLE,
  UNIQUE INDEX `name_UNIQUE` (`name` ASC) VISIBLE)
ENGINE = InnoDB;

CREATE TABLE IF NOT EXISTS `check23`.`area_of_responsibility` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(100) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `name_UNIQUE` (`name` ASC) VISIBLE)
ENGINE = InnoDB;

CREATE TABLE IF NOT EXISTS `check23`.`user_has_area_of_responsibility` (
  `user_id` INT NOT NULL,
  `area_of_responsibility_id` INT NOT NULL,
  `checkcol` VARCHAR(100) NULL,
  PRIMARY KEY (`user_id`, `area_of_responsibility_id`),
  INDEX `fk_user_has_area_of_responsibility_area_of_responsibility1_idx` (`area_of_responsibility_id` ASC) VISIBLE,
  INDEX `fk_user_has_area_of_responsibility_user1_idx` (`user_id` ASC) VISIBLE,
  UNIQUE INDEX `checkcol_UNIQUE` (`checkcol` ASC) VISIBLE,
  CONSTRAINT `fk_user_has_area_of_responsibility_user1`
    FOREIGN KEY (`user_id`)
    REFERENCES `check23`.`user` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_user_has_area_of_responsibility_area_of_responsibility1`
    FOREIGN KEY (`area_of_responsibility_id`)
    REFERENCES `check23`.`area_of_responsibility` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;

CREATE TRIGGER add_checkcol BEFORE INSERT ON check23.user_has_area_of_responsibility
FOR EACH ROW SET NEW.checkcol = CONCAT(NEW.user_id , 'u' , NEW.area_of_responsibility_id , 'a');

DROP TRIGGER add_checkcol;
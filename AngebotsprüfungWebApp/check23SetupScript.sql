INSERT INTO check23.access_group (name, create_user, create_access_group, create_client, create_quote_evaluation, create_requirement, create_solution, create_estimation) VALUES ("admin", true, true, true, true, true, true, true);
INSERT INTO check23.access_group (name, create_user, create_access_group, create_client, create_quote_evaluation, create_requirement, create_solution, create_estimation) VALUES ("default_user", false, false, false, false, false, false, false);
UPDATE check23.access_group SET id = 0 WHERE name = "default_user";
ALTER TABLE check23.access_group AUTO_INCREMENT = 2;
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
INSERT INTO check23.area_of_responsibility (name) VALUES ("Vertrieb");

/*Use the following statement to add an admin user*/
/*INSERT INTO check23.user (name, email, access_group_id) VALUES ("mmustermann", "mmustermann@weetech.com", 1);*/
/*Replace name and email as needed. The 1 indicates admin as priviliges if the setup script was run beforehand.*/
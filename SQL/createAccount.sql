USE `widmann_dev`;
DROP procedure IF EXISTS `createAccount`;

DELIMITER $$
USE `widmann_dev`$$
CREATE DEFINER=`widmann`@`%` PROCEDURE `createAccount`(username varchar(45), pass varChar(200))
BEGIN
    declare success int default 0;
    declare uid int default 0;
    DECLARE `_rollback` BOOL DEFAULT 0;
    DECLARE CONTINUE HANDLER FOR SQLEXCEPTION SET `_rollback` = 1;
    start transaction;
        
        insert into Account (uname,password) values (username,pass);
        select row_count() into success;
        select id into uid from Account where uname=username;
        if success and uid then
            insert into Balance values (uid,0);
        end if;
        if _rollback then
            rollback;
        else
            commit;
        end if;
END$$

DELIMITER ;


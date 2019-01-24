USE `widmann_dev`;
DROP procedure IF EXISTS `deposit`;

DELIMITER $$
USE `widmann_dev`$$
CREATE DEFINER=`widmann`@`%` PROCEDURE `deposit`(ident int, amt double(15,2))
BEGIN
    declare success int default 0;
    DECLARE `_rollback` BOOL DEFAULT 0;
    DECLARE CONTINUE HANDLER FOR SQLEXCEPTION SET `_rollback` = 1;
    start transaction;
        update Balance set Balance.amount=(Balance.amount+amt) where Balance.uid=ident;        
        select ROW_COUNT() into success;
        if success then
            set time_zone = 'America/Los_Angeles';
            insert into History (id,action,delta,display) values (ident,"deposit",amt,1);
        end if;
        if _rollback then
            rollback;
        else
            commit;
        end if;

END$$

DELIMITER ;


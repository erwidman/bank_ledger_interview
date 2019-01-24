USE `widmann_dev`;
DROP procedure IF EXISTS `withdrawal`;

DELIMITER $$
USE `widmann_dev`$$
CREATE DEFINER=`widmann`@`%` PROCEDURE `withdrawal`(ident int,amt float)
BEGIN
    declare success int default 0;
    declare currentAmt int default 0;
    DECLARE `_rollback` BOOL DEFAULT 0;
    DECLARE CONTINUE HANDLER FOR SQLEXCEPTION SET `_rollback` = 1;
    start transaction;
        
        select amount into currentAmt from Balance where Balance.uid = ident;
        if currentAmt - amt > -.009999  then
            update Balance set Balance.amount=(Balance.amount-amt) where Balance.uid=ident;    
            select ROW_COUNT() into success;
        end if;
        if success then
            set time_zone = 'America/Los_Angeles';
            insert into History (id,action,delta,display) values (ident,"withdraw",amt,1);
        end if;
        if _rollback then
            rollback;
        else
            commit;
        end if;
END$$

DELIMITER ;


create database BMBank;
use BMBank;

-----------BASE STRUCTURE-----------

/*
Table representing the bank account.

The table allows for viewing history (is_active).
The account number is not defined as unique in the table itself since
that will cause errors when attempting to recycle account numbers of already closed accounts.
The active_account_number_index ensures that the account_number for the active accounts is unique.
*/
create table account (
id int identity(1,1) primary key,
account_number int not null check (account_number between 10000 and 99999),
balance bigint not null default 0 check (balance >= 0),
is_active bit not null default 1,
created_at_date datetime2 not null default sysutcdatetime(),
closed_at_date datetime2 null
);

-- Sequence for issuing new account numbers (10000–99999)
create sequence account_number_sequence
as int
start with 10000 increment by 1
minvalue 10000 maxvalue 99999
no cycle;

-- Index ensuring that the active account numbers are unique
create unique index active_account_number_index
on account(account_number)
where is_active = 1;

-----------COMMAND HANDLING-----------

-- Procedure for creating a new account (AC command)
create procedure create_account
as
begin
    set xact_abort on;

    declare @account_number int;

    begin transaction;

    begin try
        set @account_number = next value for account_number_sequence;

        insert into account (account_number)
        values (@account_number);
    end try
    begin catch
        select top 1 @account_number = account_number
        from account with (updlock, readpast)
        where is_active = 0
        order by closed_at_date;

        if @account_number is null
        begin
            raiserror('No available account numbers.', 16, 1);
            rollback;
            return;
        end

        update account
        set is_active = 1, balance = 0, closed_at_date = null
        where account_number = @account_number and is_active = 0;
    end catch;

    commit;

    select @account_number as account_number;
end;

-- Procedure for account deposit (AD command)
create procedure deposit_account @account_number int, @amount bigint
as
begin
    set xact_abort on;

    if @amount <= 0
    begin
        raiserror('Invalid amount.', 16, 1);
        return;
    end

    update account set balance = balance + @amount
    where account_number = @account_number and is_active = 1;

    if @@rowcount = 0
	begin
        raiserror('Account not found.', 16, 1);
	end
end;

-- Procedure for account withdrawal (AW command)
create procedure withdraw_account @account_number int, @amount bigint
as
begin
    set xact_abort on;

    if @amount <= 0
    begin
        raiserror('Invalid amount.', 16, 1);
        return;
    end

    update account set balance = balance - @amount
    where account_number = @account_number and is_active = 1 and balance >= @amount;

    if @@rowcount = 0
	begin
        raiserror('Insufficient funds or account not found.', 16, 1);
	end
end;

-- Procedure for account balance (AB command)
create procedure get_account_balance @account_number int
as
begin
    select balance
    from account
    where account_number = @account_number and is_active = 1;

    if @@rowcount = 0
	begin
        raiserror('Account not found.', 16, 1);
	end
end;

-- Procedure for account removal (AR command)
create procedure remove_account @account_number int
as
begin
    set xact_abort on;

    update account set is_active = 0, closed_at_date = sysutcdatetime()
    where account_number = @account_number and is_active = 1 and balance = 0;

    if @@rowcount = 0
	begin
        raiserror('Account cannot be removed.', 16, 1);
	end
end;

-- View for obtaining the bank total amount (BA command)
create view vieww_bank_total_amount
as
select sum(balance) as total_amount
from account
where is_active = 1;

-- View for obtaining the bank number of clients (BN command)
create view view_bank_client_count
as
select count(*) as client_count
from account
where is_active = 1;

-----------VIEWS TO BE USED IN THE UI-----------

-- View for active accounts
create view view_active_accounts
as
select account_number, balance, created_at_date
from account
where is_active = 1;

-- View for closed accounts
create view view_closed_accounts
as
select account_number, balance, created_at_date, closed_at_date
from account
where is_active = 0;

-- View for available account numbers (available for recyclation)
create view view_available_account_numbers
as
select account_number, closed_at_date
from account
where is_active = 0;

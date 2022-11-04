### Version 3.3.16

- Fixed: Generating code for CRUD operations sometimes emitted the full namespace of the type being mapped `return account.MapToApplication.Accounts.Accounts.AccountDTO(_mapper);`. This will no longer happen but instead generate `return account.MapToAccountDTO(_mapper);`.

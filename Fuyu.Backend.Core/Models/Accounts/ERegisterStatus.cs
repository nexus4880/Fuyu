namespace Fuyu.Backend.Core.Models.Accounts;

public enum ERegisterStatus
{
    UsernameEmpty,
    UsernameTooShort,
    UsernameTooLong,
    UsernameInvalidCharacter,
    PasswordEmpty,
    PasswordTooShort,
    PasswordTooLong,
    PasswordMissingLowerCase,
    PasswordMissingUpperCase,
    PasswordMissingDigit,
    PasswordMissingSpecial,
    PasswordInvalidCharacter,
    AlreadyExists,
    Success
}
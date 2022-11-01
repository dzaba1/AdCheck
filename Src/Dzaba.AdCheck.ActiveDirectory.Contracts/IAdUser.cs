using System;

namespace Dzaba.AdCheck.ActiveDirectory.Contracts
{
    public interface IAdUser
    {
        string Sid { get; }
        string AccountName { get; }
        string Description { get; }
        string Dn { get; }
        string FirstName { get; }
        string MiddleName { get; }
        string DisplayName { get; }
        string Name { get; }
        string Surname { get; }
        DateTime? AccountExpirationDate { get; }
        DateTime? AccountLockoutTime { get; }
        bool Disabled { get; }
        string Domain { get; }
        string Manager { get; }
        bool PasswordNeverExpires { get; }
        bool PasswordNotRequired { get; }
        Guid? Guid { get; }
        DateTime? LastLogon { get; }
        bool DelegationPermitted { get; }
        DateTime? LastPasswordSet { get; }
        DateTime? LastBadPasswordAttempt { get; }
        bool UserCannotChangePassword { get; }
    }
}

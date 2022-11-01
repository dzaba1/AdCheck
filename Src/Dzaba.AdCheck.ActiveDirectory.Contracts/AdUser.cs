using System.Diagnostics;
using System;

namespace Dzaba.AdCheck.ActiveDirectory.Contracts
{
    [DebuggerDisplay("{" + nameof(Dn) + "}")]
    public sealed class AdUser : IAdUser
    {
        [AdMap("objectSid", AutoSet = false, UserPrincipalPropertyName = "Sid")]
        public string Sid { get; set; }

        [AdMap("sAMAccountName", UserPrincipalPropertyName = "SamAccountName")]
        public string AccountName { get; set; }

        [AdMap("description", UserPrincipalPropertyName = "Description")]
        public string Description { get; set; }

        [AdMap("distinguishedName", UserPrincipalPropertyName = "DistinguishedName")]
        public string Dn { get; set; }

        [AdMap("name", UserPrincipalPropertyName = "Name")]
        public string Name { get; set; }

        [AdMap("sn", UserPrincipalPropertyName = "Surname")]
        public string Surname { get; set; }

        [AdMap("accountExpires", UserPrincipalPropertyName = "AccountExpirationDate")]
        public DateTime? AccountExpirationDate { get; set; }

        [AdMap("lockouttime", UserPrincipalPropertyName = "AccountLockoutTime")]
        public DateTime? AccountLockoutTime { get; set; }

        [AdMap("userAccountControl", AutoSet = false)]
        public bool Disabled { get; set; }
        public string Domain { get; set; }
        public string Manager { get; set; }

        [AdMap("manager")]
        public string ManagerDn { get; set; }

        [AdMap("userAccountControl", AutoSet = false, UserPrincipalPropertyName = "PasswordNeverExpires")]
        public bool PasswordNeverExpires { get; set; }

        [AdMap("userAccountControl", AutoSet = false, UserPrincipalPropertyName = "PasswordNotRequired")]
        public bool PasswordNotRequired { get; set; }

        [AdMap("objectGUID", AutoSet = false, UserPrincipalPropertyName = "Guid")]
        public Guid? Guid { get; set; }

        [AdMap("lastlogon", UserPrincipalPropertyName = "LastLogon")]
        public DateTime? LastLogon { get; set; }

        [AdMap("userAccountControl", AutoSet = false, UserPrincipalPropertyName = "DelegationPermitted")]
        public bool DelegationPermitted { get; set; }

        [AdMap("pwdlastset", UserPrincipalPropertyName = "LastPasswordSet")]
        public DateTime? LastPasswordSet { get; set; }

        [AdMap("badpasswordtime", UserPrincipalPropertyName = "LastBadPasswordAttempt")]
        public DateTime? LastBadPasswordAttempt { get; set; }

        [AdMap("userAccountControl", AutoSet = false, UserPrincipalPropertyName = "UserCannotChangePassword")]
        public bool UserCannotChangePassword { get; set; }

        [AdMap("givenName", UserPrincipalPropertyName = "GivenName")]
        public string FirstName { get; set; }

        [AdMap("middleName", UserPrincipalPropertyName = "MiddleName")]
        public string MiddleName { get; set; }

        [AdMap("displayName", UserPrincipalPropertyName = "DisplayName")]
        public string DisplayName { get; set; }

        [AdMap("objectClass")]
        public string ObjectClass { get; set; }

        [AdMap("objectCategory")]
        public string ObjectCategory { get; set; }
    }
}
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Dzaba.AdCheck.ActiveDirectory.Contracts;

namespace Dzaba.AdCheck.DataAccess.Contracts.Model
{
    [Table("AdUsers")]
    public class AdUser : IAdUser
    {
        [MaxLength(64)]
        [Required(AllowEmptyStrings = false)]
        public string Sid { get; set; }

        [MaxLength(128)]
        [Required(AllowEmptyStrings = false)]
        public string AccountName { get; set; }

        public string Description { get; set; }

        [MaxLength(256)]
        [Required(AllowEmptyStrings = false)]
        public string Dn { get; set; }

        [MaxLength(64)]
        public string Name { get; set; }

        [MaxLength(64)]
        public string Surname { get; set; }

        [MaxLength(64)]
        public string FirstName { get; set; }

        [MaxLength(64)]
        public string MiddleName { get; set; }

        [MaxLength(128)]
        public string DisplayName { get; set; }
        public DateTime? AccountExpirationDate { get; set; }
        public DateTime? AccountLockoutTime { get; set; }
        public bool Disabled { get; set; }

        [MaxLength(32)]
        [Required(AllowEmptyStrings = false)]
        [Index(Name = "IX_Domain", Unique = false)]
        public string Domain { get; set; }

        [MaxLength(128)]
        public string Manager { get; set; }

        public bool PasswordNeverExpires { get; set; }
        public bool PasswordNotRequired { get; set; }
        public Guid? Guid { get; set; }
        public DateTime? LastLogon { get; set; }
        public bool DelegationPermitted { get; set; }
        public DateTime? LastPasswordSet { get; set; }
        public DateTime? LastBadPasswordAttempt { get; set; }
        public bool UserCannotChangePassword { get; set; }

        public int PollingId { get; set; }

        [JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public virtual Polling Polling { get; set; }
    }
}

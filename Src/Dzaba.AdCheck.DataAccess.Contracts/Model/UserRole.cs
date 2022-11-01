using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Dzaba.AdCheck.DataAccess.Contracts.Model
{
    [Table("UserRoles")]
    public class UserRole
    {
        [Key]
        [MaxLength(128)]
        [Required(AllowEmptyStrings = false)]
        public string UserName { get; set; }

        public int RoleId { get; set; }

        [JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public virtual Role Role { get; set; }
    }
}

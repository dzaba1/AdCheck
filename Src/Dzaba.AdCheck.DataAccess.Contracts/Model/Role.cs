using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Dzaba.AdCheck.DataAccess.Contracts.Model
{
    [Table("Roles")]
    public class Role
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(64)]
        [Required(AllowEmptyStrings = false)]
        [Index(Name = "IX_Name", Unique = false)]
        public string Name { get; set; }

        [JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public virtual ICollection<UserRole> Users { get; set; }
    }
}

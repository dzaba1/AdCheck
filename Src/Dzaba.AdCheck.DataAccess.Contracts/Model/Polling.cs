using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Dzaba.AdCheck.DataAccess.Contracts.Model
{
    [Table("Pollings")]
    public class Polling : IPolling
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime TimeStamp { get; set; }

        [JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public virtual ICollection<AdUser> Users { get; set; }
    }
}

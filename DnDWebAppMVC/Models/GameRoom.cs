using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DnDWebAppMVC.Models
{
    public class GameRoom
    {
        [Key]
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }
        [JsonProperty(PropertyName = "ownerId")]
        public Guid OwnerId { get; set; }
        [JsonProperty(PropertyName = "createdOn")]
        public DateTime? CreatedOn { get; set; }
        [JsonProperty(PropertyName = "closedOn")]
        public DateTime? ClosedOn { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
        [JsonProperty(PropertyName = "isActive")]
        public bool IsActive { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DnDWebAppMVC.Models
{
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }
        [StringLength(1000, ErrorMessage = "The message {0} cannot exceed {1} characters. ")]
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }
        [JsonProperty(PropertyName = "isPrivate")]
        public bool IsPrivate { get; set; }

        [JsonProperty(PropertyName = "senderId")]
        public Guid SenderId { get; set; }
        [JsonProperty(PropertyName = "senderName")]
        public string SenderName { get; set; }
        [JsonProperty(PropertyName = "senderProfile")]
        public string SenderProfile { get; set; }

        [JsonProperty(PropertyName = "receiverId")]
        public Guid ReceiverId { get; set; }
        [JsonProperty(PropertyName = "receiverName")]
        public string ReceiverName { get; set; }
        [JsonProperty(PropertyName = "receiverProfile")]
        public string ReceiverProfile { get; set; }

        [JsonProperty(PropertyName = "roomId")]
        public Guid RoomId { get; set; }
        [ScaffoldColumn(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [JsonProperty(PropertyName = "sentOn")]
        public DateTime? SentOn { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}

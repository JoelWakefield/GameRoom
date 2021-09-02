using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DnDWebAppMVC.Models
{
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [StringLength(1000,ErrorMessage = "The message {0} cannot exceed {1} characters. ")]
        public string Text { get; set; }
        public bool IsPrivate { get; set; }

        public Guid SenderId { get; set; }
        public string SenderName { get; set; }
        public string SenderProfile { get; set; }

        public Guid ReceiverId { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverProfile { get; set; }

        public Guid RoomId { get; set; }
        [ScaffoldColumn(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? SentOn { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DnDWebAppMVC.Models
{
    public class UserProfile
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }    //  This is for internal use, not used for auth
        public string NickName { get; set; }
        public string AccountId { get; set; }   //  This is the ID of the auth user
        public bool IsHost { get; set; }

        [ScaffoldColumn(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? CreatedOn { get; set; }
        [ScaffoldColumn(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? ModifiedOn { get; set; }
    }
}

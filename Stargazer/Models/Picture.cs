using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Stargazer.Models
{
    public class Picture
    {
        [Key]
        public int Id { get; set; }
        public string BodyName { get; set; }
        public string ImgurHash { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
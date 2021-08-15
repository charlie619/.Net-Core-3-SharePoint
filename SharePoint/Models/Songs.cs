using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SharePoint.Models
{
    public class Songs
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Author { get; set; }
        [DisplayName("Release Date (mm.yyyy)")]
        [Required]
        
        [RegularExpression(@"^(\d{2}|\d{1})*(\.)?([0-9]{4})?$", ErrorMessage = "Numbers only & should be in (MM.YYYY) Format")]
        public double ReleaseDate { get; set; }
    }
}

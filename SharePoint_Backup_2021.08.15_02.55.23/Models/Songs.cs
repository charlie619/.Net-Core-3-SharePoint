using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace SharePoint.Models
{
    public class Songs
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        [DisplayName("Release Date (mm.yyyy)")]
        public double ReleaseDate { get; set; }
    }
}

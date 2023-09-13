using System.ComponentModel.DataAnnotations;

namespace BazaGljivara.Client.Models.Locations
{
    public class AddLocation
    {
    

        [Required]
        public string Lat { get; set; }

        [Required]
        public string Lng { get; set; }

    
    }
}
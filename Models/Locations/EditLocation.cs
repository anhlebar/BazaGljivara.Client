using System.ComponentModel.DataAnnotations;

namespace BazaGljivara.Client.Models.Locations
{
    public class EditLocation
    {

        [Required]
        public string Lat { get; set; }

        [Required]
        public string Lng { get; set; }


        //[RegularExpression("([1-9][0-9]*)", ErrorMessage = "Count must be a natural number")]


        public EditLocation() { }

        public EditLocation(Location location)
        {
            Lat = location.Lat;
            Lng = location.Lng;
          
        }
    }
}

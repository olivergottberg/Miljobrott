namespace Miljöboven.Models;

// POCO-klass för bilder som kopplas till ett ärende

public class Picture
{
    public int PictureID { get; set; }
    public string PictureName { get; set; }
    public int ErrandID { get; set; }
}

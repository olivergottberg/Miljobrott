using System.ComponentModel.DataAnnotations;

// POCO-klass för statusar ett ärende kan ha

namespace Miljöboven.Models;

public class ErrandStatus
{
    [Key]
    public string StatusID { get; set; }
    public string StatusName { get; set; }
}
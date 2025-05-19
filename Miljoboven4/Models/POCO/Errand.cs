using System.ComponentModel.DataAnnotations;

// POCO-klass för ärenden, med validering för formulär

namespace Miljöboven.Models;

public class Errand
{
    public int ErrandId { get; set; }
    public string RefNumber { get; set; }

    [Display(Name = "Var har brottet skett någonstans?")]
    [Required(ErrorMessage = "Du måste fylla i plats för brottet")]
    public string Place { get; set; }

	[Display(Name = "Vilken typ av brott?")]
	[Required(ErrorMessage = "Du måste fylla i typ av brott")]
    public string TypeOfCrime { get; set; }

	[Display(Name = "När skedde brottet?")]
	[Required(ErrorMessage = "Du måste fylla i datum för observation")]
    [DataType(DataType.Date)]
    public DateTime DateOfObservation { get; set; }

	[Display(Name = "Ditt namn (för- och efternamn):")]
	[Required(ErrorMessage = "Du måste fylla i ditt namn")]
    public string InformerName { get; set; }

	[Display(Name = "Din telefon:")]
	[Required(ErrorMessage = "Du måste fylla i ditt telefonnummer")]
	[RegularExpression(@"^[0]{1}[0-9]{1,3}-[0-9]{5,9}$", ErrorMessage = "Formatet är 0XX-XXXXXXX")]
    public string InformerPhone { get; set; }

	[Display(Name = "Beskriv din observation (ex. namn på misstänkt person):")]
	public string Observation { get; set; }
    public string InvestigatorInfo { get; set; }
    public string InvestigatorAction { get; set; }
    public string StatusId { get; set; }
    public string DepartmentId { get; set; }
    public string EmployeeId { get; set; }
    public ICollection<Sample> Samples { get; set; }
    public ICollection<Picture> Pictures { get; set; }
}
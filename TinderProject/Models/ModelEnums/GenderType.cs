using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace TinderProject.Models.ModelEnums
{
	public enum GenderType
	{
		[Display(Name = "Male")]
		Male,
		[Display(Name = "Female")]
		Female,
		[Display(Name = "Other")]
		Other
	}
}

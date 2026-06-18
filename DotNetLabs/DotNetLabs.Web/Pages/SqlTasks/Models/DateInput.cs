using System.ComponentModel.DataAnnotations;

namespace DotNetLabs.Web.Pages.SqlTasks;

public sealed class DateInput
{
    [DataType(DataType.Date)]
    public DateTime VoteDate { get; set; } = DateTime.Today;
}

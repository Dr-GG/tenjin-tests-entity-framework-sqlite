namespace Tenjin.Tests.EntityFramework.Sqlite.Tests.Models;

public class PersonModel
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
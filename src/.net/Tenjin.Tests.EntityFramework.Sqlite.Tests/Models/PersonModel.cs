using System.ComponentModel.DataAnnotations;

namespace Tenjin.Tests.EntityFramework.Sqlite.Tests.Models;

public class PersonModel
{
    public int Id { get; set; }

    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;
}
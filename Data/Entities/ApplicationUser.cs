using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Biography { get; set; }
        public string? ProfileImg { get; set; } /*= "https://storageaccountthana.blob.core.windows.net/files/523b1e39-f4fb-4707-9125-bcaeff0fd399.profile-24.png";*/
        public int? AddressId { get; set; }
        public AddressEntity Address { get; set; }
        public bool IsSubscribed { get; set; }
        public bool IsDarkTheme { get; set; }
        public string? NotificationEmail { get; set; }


    }

    public class AddressEntity
    {
        [Key]
        public int Id { get; set; }
        public string AddressLine_1 { get; set; } = null!;
        public string? AddressLine_2 { get; set; }
        public string PostalCode { get; set; } = null!;
        public string City { get; set; } = null!;
    }

    public class FileEntity
    {
        [Key]
        public string FileName { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public string? ContentType { get; set; }
        public string? ContainerName { get; set; }

    }
}

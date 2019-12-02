using System;
#if BACKEND
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#elif MOBILE
using SQLite;
#endif

namespace XamList.Shared
{
#if BACKEND
    [Table("ContactModels")]
#endif
    public class ContactModel : IBaseModel
    {
        public ContactModel() => Id = Guid.NewGuid().ToString();

#if BACKEND
#else
        public string FullName => $"{FirstName} {LastName}";
#endif

#if MOBILE
        [PrimaryKey, Unique]
#elif BACKEND
        [Key]
#endif
        public string Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }

#if BACKEND
        [Column("deleted")]
#endif
        public bool IsDeleted { get; set; }
    }
}

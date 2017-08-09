using System;
#if Backend
using System.Data.Linq.Mapping;
#elif MOBILE
using SQLite;
using Newtonsoft.Json;
#endif

namespace XamList.Shared
{
#if Backend
    [Table(Name = "ContactModels")]
#endif
    public class ContactModel : IBaseModel
    {
        #region Constructors
        public ContactModel() => Id = Guid.NewGuid().ToString();
        #endregion

        #region Properties
#if Backend
#else
        public string FullName => $"{FirstName} {LastName}";
#endif

#if Backend
        [Column(Name = nameof(Id), IsPrimaryKey = true, CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
#elif MOBILE
        [PrimaryKey, Unique]
#endif
        public string Id { get; set; }
#if Backend
        [Column(Name = nameof(FirstName), IsPrimaryKey = false, CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
#endif
        public string FirstName { get; set; }

#if Backend
        [Column(Name = nameof(LastName), IsPrimaryKey = false, CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
#endif
        public string LastName { get; set; }

#if Backend
        [Column(Name = nameof(PhoneNumber), IsPrimaryKey = false, CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
#endif
        public string PhoneNumber { get; set; }

#if Backend
        [Column(Name = nameof(CreatedAt), IsPrimaryKey = false, CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
#endif
        public DateTimeOffset CreatedAt { get; set; }

#if Backend
        [Column(Name = nameof(UpdatedAt), IsPrimaryKey = false, CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
#endif
        public DateTimeOffset UpdatedAt { get; set; }

#if Backend
        [Column(Name = "deleted", IsPrimaryKey = false, CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
#endif
        public bool IsDeleted { get; set; }
        #endregion
    }
}

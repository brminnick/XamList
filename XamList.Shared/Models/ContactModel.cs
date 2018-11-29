using System;
#if BACKEND
using NPoco;
#elif MOBILE
using SQLite;
using Newtonsoft.Json;
#endif

namespace XamList.Shared
{
#if BACKEND
    [TableName("ContactModels")]
#endif
    public class ContactModel : IBaseModel
    {
        #region Constructors
        public ContactModel() => Id = Guid.NewGuid().ToString();
        #endregion

        #region Properties
#if BACKEND
#else
        public string FullName => $"{FirstName} {LastName}";
#endif

#if MOBILE
        [PrimaryKey, Unique]
#endif
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }

#if BACKEND
        [Column("deleted")]
#endif
        public bool IsDeleted { get; set; }
        #endregion
    }
}

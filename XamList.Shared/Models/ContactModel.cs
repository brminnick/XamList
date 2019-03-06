using System;
using Newtonsoft.Json;
#if BACKEND
using NPoco;
#elif MOBILE
using SQLite;
#endif

namespace XamList.Shared
{
#if BACKEND
    [TableName("ContactModels")]
    [PrimaryKey(nameof(Id), AutoIncrement = false)]
#endif
    public class ContactModel : IBaseModel
    {
        #region Constructors
        public ContactModel() : this(Guid.NewGuid().ToString())
        {

        }

        [JsonConstructor]
        public ContactModel(string id) => Id = id;
        #endregion

        #region Properties
#if BACKEND
#else
        public string FullName => $"{FirstName} {LastName}";
#endif

#if MOBILE
        [PrimaryKey, Unique]
#endif
        public string Id { get; }
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

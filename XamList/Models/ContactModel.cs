using System;

using SQLite;

namespace XamList
{
    public class ContactModel
    {
        #region Constructors
        public ContactModel() => Id = Guid.NewGuid().ToString();
        #endregion

        #region Properties
        public string FullName => $"{FirstName} {LastName}";

        [PrimaryKey, Unique]
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        #endregion
    }
}

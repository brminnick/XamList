using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using XamList.Shared;

namespace XamList.Backend.Shared
{
    class XamListDatabaseService
    {
        readonly XamListDbContext _xamListDbContext;

        public XamListDatabaseService(XamListDbContext dbContext) => _xamListDbContext = dbContext;

        public IReadOnlyList<ContactModel> GetAllContactModels(Func<ContactModel, bool> wherePredicate) =>
            _xamListDbContext.Contacts.Where(wherePredicate).ToList();

        public async Task<IReadOnlyList<ContactModel>> GetAllContactModels() =>
            await _xamListDbContext.Contacts.ToListAsync().ConfigureAwait(false);

        public Task<ContactModel> GetContactModel(string id) =>
            _xamListDbContext.Contacts.SingleAsync(x => x.Id.Equals(id));

        public async Task<ContactModel> InsertContactModel(ContactModel contact)
        {
            if (string.IsNullOrWhiteSpace(contact.Id))
                contact.Id = Guid.NewGuid().ToString();

            contact.CreatedAt = DateTimeOffset.UtcNow;
            contact.UpdatedAt = DateTimeOffset.UtcNow;

            await _xamListDbContext.AddAsync(contact).ConfigureAwait(false);
            await _xamListDbContext.SaveChangesAsync().ConfigureAwait(false);

            return contact;
        }

        public async Task<ContactModel> PatchContactModel(ContactModel contactModel)
        {
            var contactFromDatabase = await _xamListDbContext.Contacts.SingleAsync(y => y.Id.Equals(contactModel.Id)).ConfigureAwait(false);

            contactFromDatabase.FirstName = contactModel.FirstName;
            contactFromDatabase.LastName = contactModel.LastName;
            contactFromDatabase.PhoneNumber = contactModel.PhoneNumber;
            contactFromDatabase.IsDeleted = contactModel.IsDeleted;
            contactFromDatabase.UpdatedAt = DateTimeOffset.UtcNow;

            _xamListDbContext.Update(contactFromDatabase);
            await _xamListDbContext.SaveChangesAsync().ConfigureAwait(false);

            return contactFromDatabase;
        }

        public async Task<ContactModel> DeleteContactModel(string id)
        {
            var contactFromDatabase = await _xamListDbContext.Contacts.SingleAsync(y => y.Id.Equals(id)).ConfigureAwait(false);

            contactFromDatabase.IsDeleted = true;

            return await PatchContactModel(contactFromDatabase).ConfigureAwait(false);
        }

        public async Task<ContactModel> RemoveContactModel(string id)
        {
            var answerModelFromDatabase = await _xamListDbContext.Contacts.SingleAsync(x => x.Id.Equals(id)).ConfigureAwait(false);

            _xamListDbContext.Remove(answerModelFromDatabase);
            await _xamListDbContext.SaveChangesAsync().ConfigureAwait(false);

            return answerModelFromDatabase;
        }
    }
}
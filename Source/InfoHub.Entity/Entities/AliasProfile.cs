using System;
using InfoHub.Entity.Models;
using InfoHub.Infrastructure.Security.Helpers;
using InfoHub.ORM.Attributes;

namespace InfoHub.Entity.Entities
{
    public partial class AliasProfile : BaseEntity
    {
        [Sortable, Filter, Encrypted, Obsolete]
        public virtual string FullNameEncrypted { get; set; }
        [Sortable, Filter, Encrypted, Obsolete]
        public virtual string LastNameEncrypted { get; set; }

        
    }
}

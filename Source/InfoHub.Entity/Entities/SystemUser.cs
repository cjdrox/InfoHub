using System.Collections.Generic;
using InfoHub.Entity.Models;
using InfoHub.ORM.Attributes;

namespace InfoHub.Entity.Entities
{
    public class SystemUser : BaseEntity
    {
        [Sortable, Filter, Encrypted]
        public virtual string Username { get; set; }
        public virtual string Passhash{ get; set; }
        public List<AliasProfile> AliasProfiles { get; set; }
    }
}

using System.Collections.Generic;
using InfoHub.Entity.Attributes;
using InfoHub.Entity.Models;

namespace InfoHub.Entity.Entities
{
    public class SystemUser : BaseEntity
    {
        [Sortable, Filter]
        public virtual string Username { get; set; }
        public virtual string Passhash{ get; set; }
        public List<AliasProfile> AliasProfiles { get; set; }
    }
}

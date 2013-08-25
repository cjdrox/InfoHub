using System;
using System.Collections.Generic;
using System.Net;
using InfoHub.Entity.Models;
using InfoHub.ORM.Attributes;

namespace InfoHub.Entity.Entities
{
    public partial class SystemUser : BaseEntity
    {
        [Sortable, Filter, Encrypted, Obsolete]
        public virtual string UsernameEncrypted { get; set; }

        [MapAs("Password")]
        public virtual string Passhash{ get; set; }

        [OnDelete(Do.Restrict)]
        public List<AliasProfile> AliasProfiles { get; set; }
    }

    public partial class LoginAttempt : BaseEntity
    {
        [OnDelete(Do.Proceed)]
        public SystemUser SystemUser { get; set; }

        [Sortable, Filter]
        public DateTime TimeStamp { get; set; }

        [Sortable, Filter]
        public IPAddress IPAddress { get; set; }
    }
}

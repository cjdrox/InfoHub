using InfoHub.Entity.Models;
using InfoHub.ORM.Attributes;

namespace InfoHub.Entity.Entities
{
    public class AliasProfile : BaseEntity
    {
        [Sortable, Filter, Encrypted]
        public virtual string FullName { get; set; }
        [Sortable, Filter, Encrypted]
        public virtual string LastName{ get; set; }
    }
}

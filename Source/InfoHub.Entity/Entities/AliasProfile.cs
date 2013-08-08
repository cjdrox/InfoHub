using InfoHub.Entity.Attributes;
using InfoHub.Entity.Models;

namespace InfoHub.Entity.Entities
{
    public class AliasProfile : BaseEntity
    {
        [Sortable, Filter]
        public virtual string FullName { get; set; }
        public virtual string LastName{ get; set; }
    }
}

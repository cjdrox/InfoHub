using InfoHub.Entity.Models;
using InfoHub.ORM.Attributes;

namespace InfoHub.Entity.Entities
{
    public class Country : BaseEntity
    {
        [Sortable, Filter]
        public string Name { get; set; }
        [Sortable, Filter]
        public string FullName { get; set; }
    }
}

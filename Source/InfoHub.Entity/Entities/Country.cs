using InfoHub.Entity.Attributes;
using InfoHub.Entity.Models;

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

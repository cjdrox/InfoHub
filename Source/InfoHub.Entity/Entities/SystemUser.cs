using InfoHub.Entity.Models;

namespace InfoHub.Entity.Entities
{
    public class SystemUser : BaseEntity
    {
        public virtual string Username { get; set; }
        public virtual string Passhash{ get; set; }
    }
}

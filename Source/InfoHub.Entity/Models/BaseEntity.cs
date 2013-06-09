using System;
using InfoHub.Entity.Attributes;
using InfoHub.ORM.Models;

namespace InfoHub.Entity.Models
{
    [Table]
    public class BaseEntity : Table
    {
        public virtual Guid Id { get; protected set; }
        public virtual DateTime CreatedAt { get; set; }
        public virtual DateTime ModifiedAt { get; set; }
        public virtual bool IsDeleted { get; set; }

        public BaseEntity()
        {
            Name = GetType().Name;
        }
    }
}

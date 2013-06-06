using System;
using InfoHub.Entity.Attributes;
using InfoHub.ORM.Models;

namespace InfoHub.Entity.Models
{
    [Table]
    public class BaseEntity : Table
    {
        public virtual Guid Id { get; protected set; }
        public virtual Guid CreatedAt { get; set; }
        public virtual Guid ModifiedAt { get; set; }
        public virtual Guid IsDeleted { get; set; }
    }
}

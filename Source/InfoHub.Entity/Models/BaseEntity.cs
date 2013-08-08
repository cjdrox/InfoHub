using System;
using InfoHub.Entity.Attributes;
using InfoHub.Entity.Interfaces;
using InfoHub.ORM.Models;

namespace InfoHub.Entity.Models
{
    [Table]
    public abstract class BaseEntity : Table, IEntity
    {
        [PrimaryKey]
        public virtual Guid Id { get; private set; }
        public virtual DateTime? CreatedAt { get; set; }
        public virtual DateTime? ModifiedAt { get; set; }
        public virtual DateTime? DeletedAt { get; set; }

        [Unmapped]
        public bool IsNew
        {
            get { return !CreatedAt.HasValue; }
        }

        [Unmapped]
        public bool IsDeleted
        {
            get { return DeletedAt.HasValue; }
        }

        protected BaseEntity()
        {
            Id = Guid.NewGuid();
            TableName = GetType().Name;
        }
    }
}

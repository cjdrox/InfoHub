using System;
using System.Runtime.Serialization;
using InfoHub.ORM.Attributes;

namespace InfoHub.Entity.Interfaces
{
    public interface IEntity : ISerializable, ICloneable
    {
        [PrimaryKey]
        Guid Id { get; }

        DateTime? CreatedAt { get; set; }
        DateTime? ModifiedAt { get; set; }
        DateTime? DeletedAt { get; set; }

        [Unmapped]
        bool IsNew { get; }

        [Unmapped]
        bool IsDeleted { get; }
    }
}
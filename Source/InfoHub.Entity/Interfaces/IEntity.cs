using System;
using InfoHub.Entity.Attributes;

namespace InfoHub.Entity.Interfaces
{
    public interface IEntity
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
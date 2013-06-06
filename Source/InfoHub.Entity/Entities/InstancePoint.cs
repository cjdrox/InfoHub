using System;
using InfoHub.Entity.Models;
using InfoHub.Entity.Types;

namespace InfoHub.Entity.Entities
{
    public class InstancePoint : BaseEntity
    {
        public string Name { get; set; }
        public virtual DateTime DateAndTime { get; set; }
        public virtual Coordinate Point { get; set; }
        public virtual Ambience Ambience { get; set; }
        public virtual double JulianDay { get { return 0; } }
    }
}

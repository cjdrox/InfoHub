using System;
using System.Collections.Generic;
using InfoHub.Entity.Models;

namespace InfoHub.Entity.Entities
{
    public class BusinessEvent : BaseEntity
    {
        public virtual InstancePoint Source { get; set; }
        //public virtual List<InstancePoint> Destination { get; set; }
        public virtual DateTime TimeStamp { get; set; }
        public virtual string Description { get; set; }
        public virtual Dictionary<string, string> Properties { get; set; }
    }
}

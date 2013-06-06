using System;

namespace InfoHub.ORM.Types
{
    public struct ColumnData
    {
        public Type Type { get; set; }
        public long Length { get; set; }
        public bool IsPrimary { get; set; }
        public bool NotNull { get; set; }
        public string DefaultValue { get; set; }
    }
}

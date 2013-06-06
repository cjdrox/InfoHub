using System;
using InfoHub.Deployer.Interfaces;

namespace InfoHub.Deployer.Models
{
    public class ScriptBase : IScript
    {
        private readonly Guid _id;
        public const string Anonymous = "anonymous";

        protected Guid Id
        {
            get { return _id; }
        }

        public string Author { get; set; }
        public DateTime CreatedAt { get; set; }

        public ScriptBase()
            : this(Anonymous, DateTime.Now)
        {
            _id = Guid.NewGuid();
        }

        private ScriptBase(string author, DateTime createdAt)
        {
            Author = author;
            CreatedAt = createdAt;
        }

        public virtual bool Execute()
        {
            return false;
        }

        public virtual bool RollBack()
        {
            return false;
        }

        public virtual void Dispose()
        {
        }

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}
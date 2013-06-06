using System;
using InfoHub.Business.Attributes;
using InfoHub.Business.Interfaces;
using InfoHub.Entity.Domain;

namespace InfoHub.Business.Requests
{
    public sealed class LoginRequest : ILoginRequest
    {
        public bool Sumbitted { get; set; }

        [RequiredField, OperativeField(Discrete = true, Sortable = true)]
        public string Username { get; set; }

        [RequiredField]
        public string Password { get; set; }
        public bool Remember { get; set; }
        public bool ForgottenPassword { get; set; }
        public DateTime RequestedAt { get; set; }
        public IBusinessEvent EventData { get; set; }

        public AccountProfile Data { get; set; }
    }
}

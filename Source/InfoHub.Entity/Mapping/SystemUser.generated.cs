using InfoHub.Infrastructure.Security.Helpers;
using InfoHub.ORM.Attributes;

namespace InfoHub.Entity.Entities
{	
	public partial class SystemUser 
	{
		[Unmapped]
		public System.String Username
		{ 
			get { return UsernameEncrypted.Decrypt(); }
			set { UsernameEncrypted = value.Decrypt(); }
		}

	}
}

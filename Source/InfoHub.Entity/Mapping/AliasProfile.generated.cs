using InfoHub.Infrastructure.Security.Helpers;
using InfoHub.ORM.Attributes;

namespace InfoHub.Entity.Entities
{	
	public partial class AliasProfile 
	{
		[Unmapped]
		public System.String FullName
		{ 
			get { return FullNameEncrypted.Decrypt(); }
			set { FullNameEncrypted = value.Decrypt(); }
		}

		[Unmapped]
		public System.String LastName
		{ 
			get { return LastNameEncrypted.Decrypt(); }
			set { LastNameEncrypted = value.Decrypt(); }
		}

	}
}

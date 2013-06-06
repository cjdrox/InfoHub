namespace InfoHub.ORM.Models
{
    public class Configuration : ConfigurationBase
    {
        public Configuration(string host, string database, string port, string username, string password) 
            : base(host, database, port, username, password)
        {
        }
    }
}

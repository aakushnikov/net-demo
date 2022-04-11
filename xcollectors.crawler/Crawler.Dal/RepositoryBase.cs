using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.Dal
{
	public abstract class RepositoryBase
	{
		private string DbName { get; set; }
        private string _connectionString;

		protected RepositoryBase(string dbDbName)
		{
			DbName = dbDbName;
		}
        private string ConnectionString
        {
            get
            {
                if (_connectionString == null)
                {
                    var builder = new ConfigurationBuilder()
                       .SetBasePath(Directory.GetCurrentDirectory())
                       .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                    var configuration = builder.Build();
                    var setting = configuration.GetConnectionString(DbName);
                    if (string.IsNullOrEmpty(setting))
                    {
                        throw new InvalidOperationException($"Connection string \"{DbName}\" not found in config file");
                    }

                    _connectionString = setting;
                }

                return _connectionString;
            }
        }
        protected IDbConnection GetConnection()
		{
			return new SqlConnection(ConnectionString);
		}
		
	}
}

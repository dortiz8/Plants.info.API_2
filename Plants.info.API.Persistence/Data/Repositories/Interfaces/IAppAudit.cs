using System;
using Plants.info.API.Common.Data.Models;
using Plants.info.API.Data.Repository;

namespace Plants.info.API.Persistence.Data.Repositories.Interfaces
{
	public interface IAppAuditRepository : IDbActions
	{
		Task UpdateAppAudit(AppAudit appAudit); 
	}
}


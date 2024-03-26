using System;
using Plants.info.API.Common.Data.Models;

namespace Plants.info.API.Business.Data.Services.AppAuditService.Interfaces
{
	public interface IAppAuditService
	{
		Task AddToAppAudit(int toolId, string action, string summary); 
	}
}


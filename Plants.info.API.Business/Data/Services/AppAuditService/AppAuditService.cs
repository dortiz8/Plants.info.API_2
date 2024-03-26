using System;
using Plants.info.API.Business.Data.Services.AppAuditService.Interfaces;
using Plants.info.API.Common.Data.Models;
using Plants.info.API.Persistence.Data.Repositories.Interfaces;

namespace Plants.info.API.Business.Data.Services.AppAuditService
{
	public class AppAuditService : IAppAuditService
	{
        private readonly IAppAuditRepository _appAuditRepository;

        public AppAuditService(IAppAuditRepository appAuditRepository)
        {
            _appAuditRepository = appAuditRepository;
        }

        public async Task AddToAppAudit(int toolId, string action, string summary)
        {
            var appAudit = new AppAudit
            {
                ToolId = toolId,
                Action = action,
                Summary = summary,
                DateAdded = DateTime.Now
            }; 
            await _appAuditRepository.UpdateAppAudit(appAudit);
            await _appAuditRepository.SaveAllChangesAsync(); 
        }
    }
}


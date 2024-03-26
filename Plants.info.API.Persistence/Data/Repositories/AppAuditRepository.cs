using System;
using Plants.info.API.Common.Data.Models;
using Plants.info.API.Data.Contexts;
using Plants.info.API.Persistence.Data.Repositories.Interfaces;

namespace Plants.info.API.Persistence.Data.Repositories
{
	public class AppAuditRepository : IAppAuditRepository
	{
        private readonly UserContext _ctx;
        public AppAuditRepository(UserContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<bool> SaveAllChangesAsync()
        {
            return (await _ctx.SaveChangesAsync() >= 0);
        }

        public async Task UpdateAppAudit(AppAudit appAudit)
        {
            await _ctx.AppAudit.AddAsync(appAudit); 
        }
    }
}


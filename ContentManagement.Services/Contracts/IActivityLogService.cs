using ContentManagement.Entities;
using ContentManagement.ViewModels.Areas.Manage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContentManagement.Services.Contracts
{
    public interface IActivityLogService
    {
        Task<IEnumerable<ActivityLogViewModel>> GetActivityLogsAsync(string keyword, int startIndex = 0, int pageSize = 50);
        Task<ActivityLog> CreateActivityLogAsync(ActivityLogViewModel activityLog);
        //void DeleteActivityLog(DateTime? to);
        Task<long> TotalFilteredLogsCountAsync(string keyword);
        Task<long> TotalLogsCountAsync();
    }
}

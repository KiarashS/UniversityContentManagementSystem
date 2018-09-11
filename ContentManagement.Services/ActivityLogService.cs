using ContentManagement.Common.GuardToolkit;
using ContentManagement.DataLayer.Context;
using ContentManagement.Entities;
using ContentManagement.Services.Contracts;
using ContentManagement.ViewModels.Areas.Manage;
using EFSecondLevelCache.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContentManagement.Services
{
    public class ActivityLogService: IActivityLogService
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<ActivityLog> _logs;

        public ActivityLogService(IUnitOfWork uow)
        {
            _uow = uow;
            _uow.CheckArgumentIsNull(nameof(uow));

            _logs = _uow.Set<ActivityLog>();
        }

        public async Task<IEnumerable<ActivityLogViewModel>> GetActivityLogsAsync(string keyword, int startIndex = 0, int pageSize = 50)
        {
            var logs = new List<ActivityLogViewModel>();
            var logsQuery = _logs.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                logsQuery = logsQuery.Where(l => l.ActionBy.Contains(keyword) || l.Message.Contains(keyword) || l.Portal.Contains(keyword));
            }

            var list = await logsQuery
                .OrderByDescending(l => l.Id)
                .Skip(startIndex)
                .Take(pageSize)
                .Cacheable()
                .ToListAsync().ConfigureAwait(false);

            foreach (var log in list)
            {
                logs.Add(new ActivityLogViewModel
                {
                    Id = log.Id,
                    ActionBy = log.ActionBy,
                    ActionDate = log.ActionDate,
                    ActionLevel = log.ActionLevel,
                    ActionType = log.ActionType,
                    Message = log.Message,
                    Portal = log.Portal,
                    Language = log.Language,
                    SourceAddress = log.SourceAddress,
                    Url = log.Url
                });
            }

            return logs;
        }

        public async Task<ActivityLog> CreateActivityLogAsync(ActivityLogViewModel activityLog)
        {
            var newLog = new ActivityLog
            {
                ActionBy = activityLog.ActionBy,
                Url = activityLog.Url,
                SourceAddress = activityLog.SourceAddress,
                Message = activityLog.Message,
                Portal = activityLog.Portal,
                Language = activityLog.Language,
                ActionType = activityLog.ActionType,
                ActionLevel = activityLog.ActionLevel
            };

            _logs.Add(newLog);
            await _uow.SaveChangesAsync().ConfigureAwait(false);

            return newLog;
        }

        //public void DeleteActivityLog(DateTime? to)
        //{
        //    var deleteTo = DateTime.UtcNow.AddDays(-31);
        //    if(to.HasValue)
        //    {
        //        deleteTo = (DateTime)to;
        //    }

        //    var affectedRows = _uow.Database.ExecuteSqlCommand(
        //        "DELETE From [dbo].[ActivityLogs] WHERE ([ActionDate] = @p0)", deleteTo);
        //}

        public async Task<long> TotalFilteredLogsCountAsync(string keyword)
        {
            var logsCount = _logs.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                logsCount = logsCount.Where(l => l.ActionBy.Contains(keyword) || l.Message.Contains(keyword) || l.Portal.Contains(keyword));
            }

            return await logsCount.Select(l => l.Id).Cacheable().LongCountAsync().ConfigureAwait(false);
        }

        public async Task<long> TotalLogsCountAsync()
        {
            return await _logs.Select(l => l.Id).Cacheable().LongCountAsync().ConfigureAwait(false);
        }
    }
}

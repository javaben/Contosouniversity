using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MvcWithEf.Models
{
    public partial class ContosouniversityContext : DbContext
    {

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            FilterBeforeModified();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess,cancellationToken);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            FilterBeforeModified();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            FilterBeforeModified();
            return base.SaveChanges();
        }

        private void FilterBeforeModified()
        {
            var entities = this.ChangeTracker.Entries();
            foreach (var entry in entities)
            {
                if (entry.State == EntityState.Modified)
                {
                    //請修改 Course, Department, Person 表格，新增 DateModified 欄位(datetime)，並且這三個表格的資料透過 Web API 更新時，都要自動更新該欄位為更新當下的時間
                    entry.CurrentValues.SetValues(
                            new { DateModified = DateTime.Now }
                            );

                }
                else if (entry.State == EntityState.Deleted)
                {
                    //請修改 Course, Department, Person 表格欄位，新增 IsDeleted 欄位 (bit)，且讓所有刪除這三個表格資料的 API 都不能真的刪除資料，而是標記刪除即可，標記刪除後，在 GET 資料的時候不能輸出該筆資料。
                    switch (entry.Entity.GetType().Name)
                    {
                        case nameof(Course):
                        case nameof(Department):
                        case nameof(Person):
                            entry.State = EntityState.Modified;
                            entry.CurrentValues.SetValues(
                                new { IsDeleted = true }
                            );
                            break;
                    }
                }
            }
        }
    }
}

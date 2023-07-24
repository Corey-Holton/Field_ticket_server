using CA.Ticketing.Business.Services.Employees.Dto;
using CA.Ticketing.Business.Services.Scheduling.Dto;
using CA.Ticketing.Common.Enums;
using CA.Ticketing.Persistance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Business.Services.Scheduling
{
    public interface ISchedulingService
    {
        Task<IEnumerable<SchedulingDto>> GetAll();

        Task<SchedulingDto> GetById(int id);

        Task<int> Create(SchedulingDto entity);

        Task Update(SchedulingDto entity);

        Task Delete(int id);

    }
}

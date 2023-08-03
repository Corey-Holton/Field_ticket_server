using CA.Ticketing.Business.Services.Charges.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Business.Services.Charges
{
    public interface IChargesService
    {
        Task<IEnumerable<ChargeDto>> GetAll();

        Task<int> Create(ChargeDto entity);

        Task Update(ChargeDto entity);

        Task Delete(int id);
    }
}

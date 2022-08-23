using CNF.Business.Repositories.Repository;
using CNF.Business.Resources.ExceptionLogging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNF.Business.Repositories.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get;}
        ILoginRepository loginRepository { get; }
        IAdminRepository adminRepository { get; }
        ILogger GetLoggerInstance { get; }
        IMastersRepository MastersRepository { get; }
        ILoggerRepository GetLoggerRepositoryInstance { get; }
        IOrderDispatchRepository OrderDispatchRepository { get; }
        IChequeAccountingRepository chequeAccountingRepository { get; }
        IConfigurationRepository configurationRepository { get; }
        IOrderReturnRepository OrderReturnRepository { get; }
        IInventoryInwardRepository inventoryInwardRepository { get; }
    }
}

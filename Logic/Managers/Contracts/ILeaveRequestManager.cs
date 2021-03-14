using System.Threading.Tasks;
using leave_management.Models;

namespace leave_management.Logic.Managers.Contracts
{
    public interface ILeaveRequestManager
    {
         Task<AdminLeaveRequestViewVM> GetAllAdminLeaveRequestViewVM();
    }
}
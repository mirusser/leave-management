using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using leave_management.Contracts;
using leave_management.Data;
using leave_management.Logic.Managers.Contracts;
using leave_management.Models;

namespace leave_management.Logic.Managers
{
    public class LeaveRequestManager : BaseLeaveManager, ILeaveRequestManager
    {
        public LeaveRequestManager(
            IMapper mapper, 
            IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
        {

        }

        public async Task<AdminLeaveRequestViewVM> GetAllAdminLeaveRequestViewVM()
        {
            var leaveRequests = await unitOfWork.LeaveRequests.FindAll(includes: new List<string>() { nameof(LeaveRequest.RequestingEmployee), nameof(LeaveRequest.ApprovedBy), nameof(LeaveRequest.LeaveType) });
            var leaveRequestsVM = mapper.Map<List<LeaveRequestVM>>(leaveRequests);

            var adminLeaveRequestVM = new AdminLeaveRequestViewVM()
            {
                LeaveRequestVMs = leaveRequestsVM,
                TotalNumberOfRequests = leaveRequestsVM.Count,
                TotalNumberOfApprovedRequests = leaveRequestsVM.Count(x => x.Approved == true),
                TotalNumberOfPendingRequests = leaveRequestsVM.Count(x => x.Approved == null),
                TotalNumberOfRejectedRequests = leaveRequestsVM.Count(x => x.Approved == false)
            };

            return adminLeaveRequestVM;
        }
    }
}
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace leave_management.Models
{
    public class LeaveRequestVM
    {
        public int Id { get; set; }

        public EmployeeVM RequestingEmployee { get; set; }

        [Display(Name = "Employee name")]
        public string RequestingEmployeeId { get; set; }

        [Required]
        [Display(Name = "Start date")]
        public DateTime StarDate { get; set; }

        [Required]
        [Display(Name = "End date")]
        public DateTime EndDate { get; set; }

        public LeaveTypeVM LeaveType { get; set; }
        public int LeaveTypeId { get; set; }

        [Display(Name = "Date requested")]
        public DateTime DateRequested { get; set; }

        [Display(Name = "Date actioned")]
        public DateTime DateActioned { get; set; }

        [Display(Name = "Approval state")]
        public bool? Approved { get; set; }

        public EmployeeVM ApprovedBy { get; set; }

        [Display(Name = "Approver name")]
        public string ApprovedById { get; set; }
    }

    public class AdminLeaveRequestViewVM 
    {
        [Display(Name = "No. of all requests")]
        public int TotalNumberOfRequests { get; set; }

        [Display(Name = "No. of approved requests")]
        public int TotalNumberOfApprovedRequests { get; set; }

        [Display(Name = "No. of pending requests")]
        public int TotalNumberOfPendingRequests { get; set; }

        [Display(Name = "No. of rejected requests")]
        public int TotalNumberOfRejectedRequests { get; set; }

        public List<LeaveRequestVM> LeaveRequestVMs { get; set; }
    }

    public class CreateLeaveRequestVM
    {
        public CreateLeaveRequestVM()
        {
            StartDate = DateTime.Now.Date.ToString("dd/MM/yyyy").Replace(".", "/");
            EndDate = DateTime.Now.Date.ToString("dd/MM/yyyy").Replace(".", "/");
        }

        [Required]
        [Display(Name = "Start date")]
        public string StartDate { get; set; }

        [Required]
        [Display(Name = "End date")]
        public string EndDate { get; set; }

        public IEnumerable<SelectListItem> LeaveTypes { get; set; }

        [Required]
        [Display(Name = "Leave type")]
        public int LeaveTypeId { get; set; }
    }

    public class EmployeeLeaveRequestViewVM
    {
        public List<LeaveAllocationVM> LeaveAllocationVMs { get; set; }
        public List<LeaveRequestVM> LeaveRequestVMs { get; set; }
    }
}

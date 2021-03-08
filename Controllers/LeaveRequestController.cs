using AutoMapper;
using leave_management.Contracts;
using leave_management.Data;
using leave_management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace leave_management.Controllers
{
    [Authorize]
    public class LeaveRequestController : Controller
    {
        private readonly IMapper mapper;
        private readonly UserManager<Employee> userManager;
        private readonly IUnitOfWork unitOfWork;

        public LeaveRequestController(
            IMapper mapper,
            UserManager<Employee> userManager,
            IUnitOfWork unitOfWork)
        {
            this.mapper = mapper;
            this.userManager = userManager;
            this.unitOfWork = unitOfWork;
        }

        [Authorize(Roles = "admin")]
        // GET: LeaveRequestController
        public async Task<IActionResult> Index()
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

            return View(adminLeaveRequestVM);
        }

        public async Task<IActionResult> MyLeave()
        {
            var employeeId = userManager.GetUserId(HttpContext.User);
            var employeeLeaveAllocations = await unitOfWork.LeaveAllocations.FindAll(x => x.EmployeeId == employeeId && x.Period == DateTime.Now.Year, includes: new List<string>() { nameof(LeaveType), nameof(Employee) });
            var employeeLeaveRequests = await unitOfWork.LeaveRequests.FindAll(
                x => x.RequestingEmployeeId == employeeId,
                includes: new List<string>() { nameof(LeaveRequest.RequestingEmployee), nameof(LeaveRequest.ApprovedBy), nameof(LeaveRequest.LeaveType) });

            var employeeAllocationVMs = mapper.Map<List<LeaveAllocationVM>>(employeeLeaveAllocations);
            var employeeRequestsVMs = mapper.Map<List<LeaveRequestVM>>(employeeLeaveRequests);

            var employeeLeaveRequestsViewVM = new EmployeeLeaveRequestViewVM
            {
                LeaveAllocationVMs = employeeAllocationVMs,
                LeaveRequestVMs = employeeRequestsVMs
            };

            return View(employeeLeaveRequestsViewVM);
        }

        // GET: LeaveRequestController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var leaveRequest = await unitOfWork.LeaveRequests.Find(
                x => x.Id == id,
                includes: new List<string>() { nameof(LeaveRequest.RequestingEmployee), nameof(LeaveRequest.ApprovedBy), nameof(LeaveRequest.LeaveType) });
            var leaveRequestVM = mapper.Map<LeaveRequestVM>(leaveRequest);

            return View(leaveRequestVM);
        }

        public async Task<IActionResult> ApproveLeaveRequest(int leaveRequestId, bool approveRequest)
        {
            try
            {
                var leaveRequest = await unitOfWork.LeaveRequests.Find(
                    x => x.Id == leaveRequestId,
                    includes: new List<string>() { nameof(LeaveRequest.RequestingEmployee), nameof(LeaveRequest.ApprovedBy), nameof(LeaveRequest.LeaveType) });
                leaveRequest.Approved = approveRequest;
                leaveRequest.DateActioned = DateTime.Now;
                leaveRequest.ApprovedById = userManager.GetUserId(HttpContext.User);

                var isSuccees = unitOfWork.LeaveRequests.Update(leaveRequest);

                if (isSuccees)
                {
                    var allocation = await unitOfWork.LeaveAllocations.Find(
                        x => x.EmployeeId == leaveRequest.RequestingEmployeeId && x.Period == DateTime.Now.Year && x.LeaveTypeId == leaveRequest.LeaveTypeId,
                        includes: new List<string>() { nameof(Employee), nameof(LeaveType) });
                    allocation.NumberOfDays -= (int)leaveRequest.EndDate.Subtract(leaveRequest.StarDate).TotalDays;
                    
                    unitOfWork.LeaveAllocations.Update(allocation);
                    await unitOfWork.Save();
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        public ActionResult CancelRequest(int leaveRequestId)
        {
            //TODO
            return View();
        }

        // GET: LeaveRequestController/Create
        public async Task<IActionResult> Create()
        {
            var leaveTypes = await unitOfWork.LeaveTypes.FindAll();
            var createLeaveRequestVM = new CreateLeaveRequestVM
            {
                LeaveTypes = leaveTypes.Select(l => new SelectListItem()
                {
                    Value = l.Id.ToString(),
                    Text = l.Name
                })
            };

            return View(createLeaveRequestVM);
        }

        // POST: LeaveRequestController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateLeaveRequestVM createLeaveRequestVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var startDate = DateTime.ParseExact(createLeaveRequestVM.StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    var endDate = DateTime.ParseExact(createLeaveRequestVM.EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                    if (DateTime.Compare(startDate, endDate) < 0)
                    {
                        var employee = await userManager.GetUserAsync(HttpContext.User);
                        var allocation = await unitOfWork.LeaveAllocations.Find(
                            x => x.EmployeeId == employee.Id && x.Period == DateTime.Now.Year && x.LeaveTypeId == createLeaveRequestVM.LeaveTypeId,
                            includes: new List<string>() { nameof(LeaveType), nameof(employee) });

                        if (allocation != null)
                        {
                            var requestedDays = (int)endDate.Subtract(startDate).TotalDays;

                            if (requestedDays > 0 && allocation.NumberOfDays >= requestedDays)
                            {
                                var leaveRequest = new LeaveRequest
                                {
                                    RequestingEmployeeId = employee.Id,
                                    StarDate = startDate,
                                    EndDate = endDate,
                                    Approved = null,
                                    DateRequested = DateTime.Now,
                                    LeaveTypeId = createLeaveRequestVM.LeaveTypeId,
                                    DateActioned = default
                                };

                                var isSuccess = await unitOfWork.LeaveRequests.Create(leaveRequest);

                                if (isSuccess)
                                {
                                    await unitOfWork.Save();
                                    return RedirectToAction(nameof(MyLeave));
                                }
                                else
                                {
                                    ModelState.AddModelError("", "Something went wrong with submitting your request.");
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", $@"Requested number of days exceed current available number of days ({allocation.NumberOfDays}) for this leave type");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Couldn't find leave allocation for this employee for given leave type");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "End date must be greater than start date");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Something went wrong.");
            }
            finally
            {
                var leaveTypes = await unitOfWork.LeaveTypes.FindAll();

                createLeaveRequestVM.LeaveTypes = leaveTypes.Select(l => new SelectListItem()
                {
                    Value = l.Id.ToString(),
                    Text = l.Name
                });

            }

            return View(createLeaveRequestVM);
        }

        // GET: LeaveRequestController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: LeaveRequestController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LeaveRequestController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LeaveRequestController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}

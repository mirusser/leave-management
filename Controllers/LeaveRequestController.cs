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
        private readonly ILeaveRequestRepository leaveRequestRepository;
        private readonly ILeaveTypeRepository leaveTypeRepository;
        private readonly ILeaveAllocationRepository leaveAllocationRepository;

        public LeaveRequestController(
            IMapper mapper,
            UserManager<Employee> userManager,
            ILeaveRequestRepository leaveRequestRepository,
            ILeaveTypeRepository leaveTypeRepository,
            ILeaveAllocationRepository leaveAllocationRepository)
        {
            this.mapper = mapper;
            this.userManager = userManager;
            this.leaveRequestRepository = leaveRequestRepository;
            this.leaveTypeRepository = leaveTypeRepository;
            this.leaveAllocationRepository = leaveAllocationRepository;
        }

        [Authorize(Roles = "admin")]
        // GET: LeaveRequestController
        public ActionResult Index()
        {
            var leaveRequests = leaveRequestRepository.FindAll();
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
            var employeeLeaveAllocations = leaveAllocationRepository.GetLeaveAllocationsByEmployeeId(employeeId);
            var employeeLeaveRequests = leaveRequestRepository.GetLeaveRequestsByEmployeeId(employeeId);

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
        public ActionResult Details(int id)
        {
            var leaveRequest = leaveRequestRepository.FindById(id);
            var leaveRequestVM = mapper.Map<LeaveRequestVM>(leaveRequest);

            return View(leaveRequestVM);
        }

        public ActionResult ApproveLeaveRequest(int leaveRequestId, bool approveRequest)
        {
            try
            {
                var leaveRequest = leaveRequestRepository.FindById(leaveRequestId);
                leaveRequest.Approved = approveRequest;
                leaveRequest.DateActioned = DateTime.Now;
                leaveRequest.ApprovedById = userManager.GetUserId(HttpContext.User);

                var isSuccees = leaveRequestRepository.Update(leaveRequest);

                if (isSuccees)
                {
                    var allocation = leaveAllocationRepository.GetLeaveAllocationByEmployeeIdAndLeaveTypeId(leaveRequest.RequestingEmployeeId, leaveRequest.LeaveTypeId);
                    allocation.NumberOfDays -= (int)leaveRequest.EndDate.Subtract(leaveRequest.StarDate).TotalDays;
                    leaveAllocationRepository.Update(allocation);
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
        public ActionResult Create()
        {
            var leaveTypes = leaveTypeRepository.FindAll();
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
                        var allocation = leaveAllocationRepository.GetLeaveAllocationByEmployeeIdAndLeaveTypeId(employee.Id, createLeaveRequestVM.LeaveTypeId);

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

                                var isSuccess = leaveRequestRepository.Create(leaveRequest);

                                if (isSuccess)
                                {
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
                var leaveTypes = leaveTypeRepository.FindAll();
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

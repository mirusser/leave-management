using AutoMapper;
using leave_management.Contracts;
using leave_management.Data;
using leave_management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_management.Controllers
{
    [Authorize(Roles = "admin")]
    public class LeaveAllocationController : Controller
    {
        private readonly IMapper mapper;
        private readonly UserManager<Employee> userManager;
        private readonly ILeaveTypeRepository leaveTypeRepository;
        private readonly ILeaveAllocationRepository leaveAllocationRepository;

        public LeaveAllocationController(
            IMapper mapper,
            UserManager<Employee> userManager,
            ILeaveTypeRepository leaveTypeRepository,
            ILeaveAllocationRepository leaveAllocationRepository)
        {
            this.mapper = mapper;
            this.userManager = userManager;
            this.leaveTypeRepository = leaveTypeRepository;
            this.leaveAllocationRepository = leaveAllocationRepository;
        }

        // GET: LeaveAllocationController
        public async Task<IActionResult> Index(int numberUpdated = default)
        {
            var leaveTypes = await leaveTypeRepository.FindAll()?.ToListAsync();
            var listOfLeaveTypeVM = mapper.Map<List<LeaveType>, List<LeaveTypeVM>>(leaveTypes);
            var createLeaveAllocationVM = new CreateLeaveAllocationVM
            {
                LeaveTypes = listOfLeaveTypeVM,
                NumberUpdated = numberUpdated
            };

            return View(createLeaveAllocationVM);
        }

        public async Task<IActionResult> SetLeave(int leaveTypeId)
        {
            var leaveType = leaveTypeRepository.FindById(leaveTypeId);
            var employees = userManager.GetUsersInRoleAsync("employee").Result;
            var numberUpdated = default(int);

            foreach (var employee in employees)
            {
                if (!leaveAllocationRepository.CheackIfAllocationExistsForEmployee(leaveTypeId, employee.Id))
                {
                    var allocation = new LeaveAllocationVM
                    {
                        DateCreated = DateTime.Now,
                        EmployeeId = employee.Id,
                        LeaveTypeId = leaveTypeId,
                        NumberOfDays = leaveType.DeafaultNumberOfDays,
                        Period = DateTime.Now.Year
                    };

                    var leaveAllocation = mapper.Map<LeaveAllocation>(allocation);
                    leaveAllocationRepository.Create(leaveAllocation);
                    numberUpdated++;
                }
            }

            return RedirectToAction(nameof(Index), new { numberUpdated = numberUpdated });
        }

        public async Task<IActionResult> ListOfEmployees()
        {
            var employees = userManager.GetUsersInRoleAsync("employee").Result;
            var listOfEmployeesVM = mapper.Map<List<EmployeeVM>>(employees);

            return View(listOfEmployeesVM);
        }

        // GET: LeaveAllocationController/Details/5
        public async Task<IActionResult> Details(string employeeId)
        {
            var employeeVM = mapper.Map<EmployeeVM>(await userManager.FindByIdAsync(employeeId));
            var leaveAllocations = mapper.Map<List<LeaveAllocationVM>>(leaveAllocationRepository.GetLeaveAllocationsByEmployeeId(employeeId).ToList());
            var viewAllocationsVM = new ViewAllocationsVM
            {
                EmployeeVM = employeeVM,
                EmployeeId = employeeId,
                LeaveAllocationVMs = leaveAllocations
            };

            return View(viewAllocationsVM);
        }

        // GET: LeaveAllocationController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LeaveAllocationController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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

        // GET: LeaveAllocationController/Edit/5
        public ActionResult Edit(int leaveAllocationId)
        {
            var leaveAllocation = leaveAllocationRepository.FindById(leaveAllocationId);
            var editLeaveAllocationVM = mapper.Map<EditLeaveAllocationVM>(leaveAllocation);

            return View(editLeaveAllocationVM);
        }

        // POST: LeaveAllocationController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditLeaveAllocationVM editLeaveAllocationVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var leaveAllocation = mapper.Map<LeaveAllocation>(editLeaveAllocationVM);
                    var isSuccess = leaveAllocationRepository.Update(leaveAllocation);

                    if (!isSuccess)
                    {
                        ModelState.AddModelError("", "Error while saving");
                    }
                    else
                    {
                        return RedirectToAction(nameof(Details), new { employeeId  = editLeaveAllocationVM.EmployeeId});
                    }
                }

                return View(editLeaveAllocationVM);
            }
            catch
            {
                return View(editLeaveAllocationVM);
            }
        }

        // GET: LeaveAllocationController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LeaveAllocationController/Delete/5
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

using AutoMapper;
using leave_management.Contracts;
using leave_management.Data;
using leave_management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_management.Controllers
{
    [Authorize(Roles = "admin")]
    public class LeaveTypesController : Controller
    {
        private readonly IMapper mapper;
        private readonly ILeaveTypeRepository leaveTypeRepository;

        public LeaveTypesController(IMapper mapper, ILeaveTypeRepository leaveTypeRepository)
        {
            this.mapper = mapper;
            this.leaveTypeRepository = leaveTypeRepository;
        }

        // GET: LeaveTypesController
        public async Task<IActionResult> Index()
        {
            var leaveTypes = await (await leaveTypeRepository.FindAll())?.ToListAsync();
            var listOfDetailsLeaveTypeVM = mapper.Map<List<LeaveType>, List<LeaveTypeVM>>(leaveTypes);

            return View(listOfDetailsLeaveTypeVM);
        }

        // GET: LeaveTypesController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (id == default || ! await leaveTypeRepository.CheckIfExistsById(id))
            {
                return NotFound();
            }

            var leaveType = await leaveTypeRepository .FindById(id);
            var leaveTypeVM = mapper.Map<LeaveTypeVM>(leaveType);

            return View(leaveTypeVM);
        }

        // GET: LeaveTypesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LeaveTypesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LeaveTypeVM leaveTypeVM)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(leaveTypeVM);
                }

                var leaveType = mapper.Map<LeaveType>(leaveTypeVM);
                leaveType.DateCreated = DateTime.Now;
                var creationResultIsSuccess = await leaveTypeRepository .Create(leaveType);

                if (!creationResultIsSuccess)
                {
                    ModelState.AddModelError(string.Empty, "Something went wrong...");
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Something went wrong...");
                return View(leaveTypeVM);
            }
        }

        // GET: LeaveTypesController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == default || ! await leaveTypeRepository.CheckIfExistsById(id))
            {
                return NotFound();
            }

            var leaveType = await leaveTypeRepository .FindById(id);
            var leaveTypeVM = mapper.Map<LeaveTypeVM>(leaveType);

            return View(leaveTypeVM);
        }

        // POST: LeaveTypesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(LeaveTypeVM leaveTypeVM)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(leaveTypeVM);
                }

                var leaveType = mapper.Map<LeaveType>(leaveTypeVM);
                var editResultIsSuccess = await leaveTypeRepository.Update(leaveType);

                if (!editResultIsSuccess)
                {
                    ModelState.AddModelError(string.Empty, "Something went wrong...");
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Something went wrong...");
                return View(leaveTypeVM);
            }
        }

        // GET: LeaveTypesController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id == default || !await leaveTypeRepository .CheckIfExistsById(id))
            {
                return NotFound();
            }

            var leaveType = await leaveTypeRepository .FindById(id);

            var isDeletionSuccess = await leaveTypeRepository .Delete(leaveType);

            if (!isDeletionSuccess)
            {
                return BadRequest();
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: LeaveTypesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, LeaveTypeVM leaveTypeVM)
        {
            try
            {
                if (id == default || ! await leaveTypeRepository.CheckIfExistsById(id))
                {
                    return NotFound();
                }

                var leaveType = await leaveTypeRepository .FindById(id);
                var isDeletionSuccess = await leaveTypeRepository .Delete(leaveType);

                if (!isDeletionSuccess)
                {
                    return RedirectToAction(nameof(Delete), new { id = id });
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction(nameof(Delete), new { id = id });
            }
        }
    }
}

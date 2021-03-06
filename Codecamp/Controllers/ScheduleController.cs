﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Codecamp.Data;
using Codecamp.Models;
using Codecamp.BusinessLogic;

namespace Codecamp.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly CodecampDbContext _context;
        private readonly IScheduleBusinessLogic _scheduleBL;

        public ScheduleController(CodecampDbContext context,
            IScheduleBusinessLogic scheduleBL)
        {
            _context = context;
            _scheduleBL = scheduleBL;
        }

        // GET: Schedule
        public async Task<IActionResult> Index()
        {
            var schedule = await _scheduleBL.GetScheduleViewModelToBuildScheduleForActiveEvent();
            return View(schedule);
        }

        // GET: Schedule/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.CodecampSchedule
                .Include(s => s.Session)
                .Include(s => s.Timeslot)
                .Include(s => s.Track)
                .FirstOrDefaultAsync(m => m.SessionId == id);
            if (schedule == null)
            {
                return NotFound();
            }

            return View(schedule);
        }

        // GET: Schedule/Create
        public IActionResult Create()
        {
            ViewData["SessionId"] = new SelectList(_context.Sessions, "SessionId", "SessionId");
            ViewData["TimeslotId"] = new SelectList(_context.Timeslots, "TimeslotId", "TimeslotId");
            ViewData["TrackId"] = new SelectList(_context.Tracks, "TrackId", "Name");
            return View();
        }

        // POST: Schedule/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SessionId,TrackId,TimeslotId")] Schedule schedule)
        {
            if (ModelState.IsValid)
            {
                _context.Add(schedule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SessionId"] = new SelectList(_context.Sessions, "SessionId", "SessionId", schedule.SessionId);
            ViewData["TimeslotId"] = new SelectList(_context.Timeslots, "TimeslotId", "TimeslotId", schedule.TimeslotId);
            ViewData["TrackId"] = new SelectList(_context.Tracks, "TrackId", "Name", schedule.TrackId);
            return View(schedule);
        }

        // GET: Schedule/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.CodecampSchedule.FindAsync(id);
            if (schedule == null)
            {
                return NotFound();
            }
            ViewData["SessionId"] = new SelectList(_context.Sessions, "SessionId", "SessionId", schedule.SessionId);
            ViewData["TimeslotId"] = new SelectList(_context.Timeslots, "TimeslotId", "TimeslotId", schedule.TimeslotId);
            ViewData["TrackId"] = new SelectList(_context.Tracks, "TrackId", "Name", schedule.TrackId);
            return View(schedule);
        }

        // POST: Schedule/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SessionId,TrackId,TimeslotId")] Schedule schedule)
        {
            if (id != schedule.SessionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(schedule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScheduleExists(schedule.SessionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["SessionId"] = new SelectList(_context.Sessions, "SessionId", "SessionId", schedule.SessionId);
            ViewData["TimeslotId"] = new SelectList(_context.Timeslots, "TimeslotId", "TimeslotId", schedule.TimeslotId);
            ViewData["TrackId"] = new SelectList(_context.Tracks, "TrackId", "Name", schedule.TrackId);
            return View(schedule);
        }

        // GET: Schedule/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.CodecampSchedule
                .Include(s => s.Session)
                .Include(s => s.Timeslot)
                .Include(s => s.Track)
                .FirstOrDefaultAsync(m => m.SessionId == id);
            if (schedule == null)
            {
                return NotFound();
            }

            return View(schedule);
        }

        // POST: Schedule/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var schedule = await _context.CodecampSchedule.FindAsync(id);
            _context.CodecampSchedule.Remove(schedule);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ScheduleExists(int id)
        {
            return _context.CodecampSchedule.Any(e => e.SessionId == id);
        }
    }
}

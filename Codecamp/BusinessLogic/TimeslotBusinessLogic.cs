﻿using Codecamp.Data;
using Codecamp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Codecamp.BusinessLogic
{
    public interface ITimeslotBusinessLogic
    {
        Task<List<Timeslot>> GetAllTimeslots();
        Task<List<Timeslot>> GetAllTimeslots(int eventId);
        Task<List<Timeslot>> GetAllTimeslotsForActiveEvent();
        Task<Timeslot> GetTimeslot(int timeslotId);
        Task<bool> TimeslotExists(int timeslotId);
        Task<bool> CreateTimeslot(Timeslot timeslot);
        Task<bool> UpdateTimeslot(Timeslot timeslot);
        Task<bool> DeleteTimeslot(int timeslotId);
    }

    public class TimeslotBusinessLogic : ITimeslotBusinessLogic
    {
        private CodecampDbContext _context { get; set; }

        public TimeslotBusinessLogic(CodecampDbContext context)
        {
            _context = context;
        }

        public async Task<List<Timeslot>> GetAllTimeslots()
        {
            return await _context.Timeslots
                .OrderBy(t => t.StartTime)
                .ToListAsync();
        }

        public async Task<List<Timeslot>> GetAllTimeslots(int eventId)
        {
            return await _context.Timeslots
                .Where(t => t.EventId == eventId)
                .OrderBy(t => t.StartTime)
                .ToListAsync();
        }

        public async Task<List<Timeslot>> GetAllTimeslotsForActiveEvent()
        {
            var activeEvent
                = await _context.Events
                .FirstOrDefaultAsync(e => e.IsActive == true);

            if (activeEvent == null)
                return await _context.Timeslots
                    .OrderBy(t => t.StartTime)
                    .ToListAsync();
            else
                return await _context.Timeslots
                    .Where(t => t.EventId == activeEvent.EventId)
                    .OrderBy(t => t.StartTime)
                    .ToListAsync();
        }

        public async Task<Timeslot> GetTimeslot(int timeslotId)
        {
            return await _context.Timeslots
                .FirstOrDefaultAsync(t => t.TimeslotId == timeslotId);
        }

        public async Task<bool> TimeslotExists(int timeslotId)
        {
            return await _context.Timeslots
                .AnyAsync(t => t.TimeslotId == timeslotId);
        }

        public async Task<bool> CreateTimeslot(Timeslot timeslot)
        {
            try
            {
                _context.Timeslots.Add(timeslot);

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateTimeslot(Timeslot timeslot)
        {
            try
            {
                _context.Timeslots.Update(timeslot);

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteTimeslot(int timeslotId)
        {
            try
            {
                var timeslot = await _context.Timeslots.FindAsync(timeslotId);
                if (timeslot != null)
                {
                    _context.Timeslots.Remove(timeslot);

                    await _context.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

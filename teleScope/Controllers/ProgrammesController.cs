using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using teleScope.Models;
using X.PagedList;
using X.PagedList.Mvc.Core;
using X.PagedList.EntityFramework;
using System.Drawing.Printing;

namespace teleScope.Controllers
{
    public class ProgrammesController : Controller
    {
        private readonly DBContext _context;

        public ProgrammesController(DBContext context)
        {
            _context = context;
        }

        // GET: Programmes
        public async Task<IActionResult> Index(int? page)
        {
            // Pagination
            if (page == null || page < 1)
            {
                page = 1;
            }

            int pageSize = 3;

            var programmes = _context.Programmes;
            
            var totalCount = await programmes.CountAsync(); //total num of results

            var programmesData = await programmes
                .Skip((page.Value - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var pagedList = new StaticPagedList<Programme>(
                programmesData, page.Value, pageSize, totalCount);

            return View(pagedList);
        }

        //GET: Programmes for Sellers
        // GET: Programmes
        public async Task<IActionResult> IndexS(int? page)
        {
            // Pagination
            if (page == null || page < 1)
            {
                page = 1;
            }

            int pageSize = 3;

            var programmes = _context.Programmes;

            var totalCount = await programmes.CountAsync(); //total num of results

            var programmesData = await programmes
                .Skip((page.Value - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var pagedList = new StaticPagedList<Programme>(
                programmesData, page.Value, pageSize, totalCount);

            return View(pagedList);
        }

        // GET: Programmes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var programme = await _context.Programmes
                .FirstOrDefaultAsync(m => m.ProgramId == id);
            if (programme == null)
            {
                return NotFound();
            }

            return View(programme);
        }

        // GET: Programmes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Programmes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProgramId,ProgrName,LandlineMinutes,MobileMinutes,FixedCost,LandlineFee,MobileFee,FiveDigitFee")] Programme programme)
        {
            if (ModelState.IsValid)
            {
                //check if the program name is unique
                var programNameExists = await _context.Programmes
                    .AnyAsync(s => s.ProgrName == programme.ProgrName);

                if (programNameExists)
                {
                    ModelState.AddModelError("ProgrName", "This program name already exists. Please enter a unique one.");
                    return View(programme);
                }

                if (programme.LandlineMinutes == 0 || programme.LandlineMinutes < 50)
                {
                    ModelState.AddModelError("LandlineMinutes", "The Landline Minutes must at least 50");
                    return View(programme);
                }

                if (programme.MobileMinutes == 0 || programme.MobileMinutes < 50)
                {
                    ModelState.AddModelError("MobileMinutes", "The Mobile Minutes must at least 50");
                    return View(programme);
                }

                if (programme.LandlineFee == 0 || programme.LandlineFee > 2)
                {
                    ModelState.AddModelError("LandlineFee", "The Landline Fee must be less than 2 euros");
                    return View(programme);
                }

                if (programme.MobileFee == 0 || programme.MobileFee > 2)
                {
                    ModelState.AddModelError("MobileFee", "The Mobile Fee must be less than 2 euros");
                    return View(programme);
                }

                if (programme.FiveDigitFee == 0 || programme.FiveDigitFee > 2)
                {
                    ModelState.AddModelError("FiveDigitFee", "The Five Digit Fee must be less than 2 euros");
                    return View(programme);
                }

                if (programme.FixedCost == 0 || programme.FixedCost < 5)
                {
                    ModelState.AddModelError("FixedCost", "The fixed cost must be 5 euros or more");
                    return View(programme);
                }

                _context.Add(programme);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(programme);
        }

        // GET: Programmes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var programme = await _context.Programmes.FindAsync(id);

            

            if (programme == null)
            {
                return NotFound();
            }
            return View(programme);
        }

        // POST: Programmes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProgramId,ProgrName,LandlineMinutes,MobileMinutes,FixedCost,LandlineFee,MobileFee,FiveDigitFee")] Programme programme)
        {
            if (id != programme.ProgramId)
            {
                return NotFound();
            }

            //check if the program name is unique
            var programNameExists = await _context.Programmes
                .AnyAsync(s => s.ProgrName == programme.ProgrName && s.ProgramId != id);

            if (programNameExists)
            {
                ModelState.AddModelError("programme.ProgrName", "This program name already exists. Please enter a unique one.");
                return View(programme);
            }
            if (programme.LandlineMinutes == 0 || programme.LandlineMinutes < 50)
            {
                ModelState.AddModelError("LandlineMinutes", "The Landline Minutes must at least 50");
                return View(programme);
            }

            if (programme.MobileMinutes == 0 || programme.MobileMinutes < 50)
            {
                ModelState.AddModelError("MobileMinutes", "The Mobile Minutes must at least 50");
                return View(programme);
            }

            if (programme.LandlineFee == 0 || programme.LandlineFee > 2)
            {
                ModelState.AddModelError("LandlineFee", "The Landline Fee must be less than 2 euros");
                return View(programme);
            }

            if (programme.MobileFee == 0 || programme.MobileFee > 2)
            {
                ModelState.AddModelError("MobileFee", "The Mobile Fee must be less than 2 euros");
                return View(programme);
            }

            if (programme.FiveDigitFee == 0 || programme.FiveDigitFee > 2)
            {
                ModelState.AddModelError("FiveDigitFee", "The Five Digit Fee must be less than 2 euros");
                return View(programme);
            }

            if (programme.FixedCost == 0 || programme.FixedCost < 5)
            {
                ModelState.AddModelError("FixedCost", "The fixed cost must be 5 euros or more");
                return View(programme);
            }

            if (ModelState.IsValid)
            {
                try
                { 

                    _context.Update(programme);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProgrammeExists(programme.ProgramId))
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
            return View(programme);
        }

        // GET: Programmes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var programme = await _context.Programmes
                .FirstOrDefaultAsync(m => m.ProgramId == id);
            if (programme == null)
            {
                return NotFound();
            }

            return View(programme);
        }

        // POST: Programmes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var programme = await _context.Programmes.FindAsync(id);
            if (programme != null)
            {
                _context.Programmes.Remove(programme);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProgrammeExists(int id)
        {
            return _context.Programmes.Any(e => e.ProgramId == id);
        }
    }
}

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

namespace teleScope.Controllers
{
    public class CallsController : Controller
    {
        private readonly DBContext _context;

        public CallsController(DBContext context)
        {
            _context = context;
        }

        // GET: Calls
        public async Task<IActionResult> Index(int? page)
        {
            // Pagination
            if (page == null || page < 1)
            {
                page = 1;
            }

            int PageSize = 8;

            //get admins data
            var callsQuery = _context.Calls
              .Include(c => c.Phone)
                 .ThenInclude(p => p.Customer)
                   .ThenInclude(c => c.User)
              .Include(c => c.Phone)
                 .ThenInclude(pn => pn.Program)
              .Select(c => new CustomerCallsModel
              {
                  user = new UserCustomerModel{
                      user = c.Phone.Customer.User,
                      customer = c.Phone.Customer,
                      phoneNumber = c.Phone

                  },
                  program = c.Phone.Program,
                  call = c
              });

            var totalCount = await callsQuery.CountAsync(); //total num of results

            var callsData = await callsQuery
                .Skip((page.Value - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            var pagedList = new StaticPagedList<CustomerCallsModel>(
                callsData, page.Value, PageSize, totalCount);

            return View(pagedList);
            
        }

        public async Task<IActionResult> CallHistory(int? page)
        {

            // Pagination
            if (page == null || page < 1)
            {
                page = 1;
            }

            int PageSize = 8;

            //get admins data
            var callsQuery = _context.Calls
              .Include(c => c.Phone)
                 .ThenInclude(p => p.Customer)
                   .ThenInclude(c => c.User)
              .Include(c => c.Phone)
                 .ThenInclude(pn => pn.Program)
               .Where(c => c.Phone.PhoneId == 10)
              .Select(c => new CustomerCallsModel
              {
                  user = new UserCustomerModel
                  {
                      user = c.Phone.Customer.User,
                      customer = c.Phone.Customer,
                      phoneNumber = c.Phone

                  },
                  program = c.Phone.Program,
                  call = c
              });

            var totalCount = await callsQuery.CountAsync(); //total num of results

            var callsData = await callsQuery
                .Skip((page.Value - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            var pagedList = new StaticPagedList<CustomerCallsModel>(
                callsData, page.Value, PageSize, totalCount);

            return View(pagedList);
        }

        // GET: Calls/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var call = await _context.Calls
                .Include(c => c.Phone)
                .FirstOrDefaultAsync(m => m.CallId == id);
            if (call == null)
            {
                return NotFound();
            }

            return View(call);
        }

        // GET: Calls/Create
        public IActionResult Create()
        {
            ViewData["PhoneId"] = new SelectList(_context.PhoneNumbers, "PhoneId", "PhoneId");
            return View();
        }

        // POST: Calls/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CallId,PhoneId,CallDate,Duration,DestinationNumber,CallType,IsIncoming")] Call call)
        {
            if (ModelState.IsValid)
            {
                _context.Add(call);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PhoneId"] = new SelectList(_context.PhoneNumbers, "PhoneId", "PhoneId", call.PhoneId);
            return View(call);
        }

        // GET: Calls/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var call = await _context.Calls.FindAsync(id);
            if (call == null)
            {
                return NotFound();
            }
            ViewData["PhoneId"] = new SelectList(_context.PhoneNumbers, "PhoneId", "PhoneId", call.PhoneId);
            return View(call);
        }

        // POST: Calls/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CallId,PhoneId,CallDate,Duration,DestinationNumber,CallType,IsIncoming")] Call call)
        {
            if (id != call.CallId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(call);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CallExists(call.CallId))
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
            ViewData["PhoneId"] = new SelectList(_context.PhoneNumbers, "PhoneId", "PhoneId", call.PhoneId);
            return View(call);
        }

        // GET: Calls/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var call = await _context.Calls
                .Include(c => c.Phone)
                .FirstOrDefaultAsync(m => m.CallId == id);
            if (call == null)
            {
                return NotFound();
            }

            return View(call);
        }

        // POST: Calls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var call = await _context.Calls.FindAsync(id);
            if (call != null)
            {
                _context.Calls.Remove(call);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CallExists(int id)
        {
            return _context.Calls.Any(e => e.CallId == id);
        }
    }
}

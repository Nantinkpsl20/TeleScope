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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace teleScope.Controllers
{
    public class AdminsController : Controller
    {
        private readonly DBContext _context;

        public AdminsController(DBContext context)
        {
            _context = context;
        }

        // GET: Admins
        public async Task<IActionResult> Index(int? page)
        {
            // Pagination
            if (page == null || page < 1)
            {
                page = 1;
            }

            int PageSize = 8;

            //get admins data
            var adminsQuery = _context.Admins
              .Include(a => a.User)
              .Select(a => new UserAdminModel
              {
                  user = a.User,
                  admin = a
              });

            var totalCount = await adminsQuery.CountAsync(); //total num of results

            var adminsData = await adminsQuery
                .Skip((page.Value - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            var pagedList = new StaticPagedList<UserAdminModel>(
                adminsData, page.Value, PageSize, totalCount);

            return View(pagedList);
           
        }

        // GET: Admins/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.AdminId == id);

            if (admin == null)
            {
                return NotFound();
            }

            var model = new UserAdminModel
            {
                user = admin.User,
                admin = admin    
            };

            return View(model);
        }

        // GET: Admins/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            return View();
        }

        // POST: Admins/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AdminId,UserId")] Admin admin)
        {
            if (ModelState.IsValid)
            {
                _context.Add(admin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", admin.UserId);
            return View(admin);
        }

        // GET: Admins/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins
               .Include(a => a.User)
               .FirstOrDefaultAsync(m => m.AdminId == id);

            if (admin == null)
            {
                return NotFound();
            }

            var model = new UserAdminModel
            {
                user = admin.User,
                admin = admin
            };

            //ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", admin.UserId);
            return View(model);
        }

        // POST: Admins/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserAdminModel model)
        {
            if (id != model.admin.AdminId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var admin = await _context.Admins
                      .Include(a => a.User)
                      .FirstOrDefaultAsync(m => m.AdminId == id);

                    if(admin == null) { return NotFound(); }

                    //check if the username exists
                    var usernameExists = await _context.Users
                        .FirstOrDefaultAsync(u => u.Username == model.user.Username
                        && u.UserId != admin.User.UserId);

                    if (usernameExists != null)
                    {
                        ModelState.AddModelError("user.Username", "This username is already in use. Please enter a unique one.");
                        return View(model);
                    }

                    if (admin.User.Username != model.user.Username)
                    {
                        admin.User.Username = model.user.Username;
                    }

                    // update data of user
                    admin.User.FirstName = model.user.FirstName;
                    admin.User.LastName = model.user.LastName;
                    admin.User.Email = model.user.Email;
                    admin.User.Password = model.user.Password;

                    _context.Update(admin.User);
                    _context.Update(admin);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminExists(model.admin.AdminId))
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
            //ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", admin.UserId);
            return View(model);
        }

        // GET: Admins/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
         
            var admin = await _context.Admins
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.AdminId == id);

            if (admin == null)
            {
                return NotFound();
            }

            var model = new UserAdminModel
            {
                user = admin.User,
                admin = admin
            };

            return View(model);
        }

        // POST: Admins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var admin = await _context.Admins
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.AdminId == id);

            if (admin != null)
            {
                _context.Users.Remove(admin.User);
                _context.Admins.Remove(admin);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdminExists(int id)
        {
            return _context.Admins.Any(e => e.AdminId == id);
        }
    }
}

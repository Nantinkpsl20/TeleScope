using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using teleScope.Models;
using X.PagedList.Mvc.Core;
using X.PagedList.EntityFramework;
using X.PagedList;

namespace teleScope.Controllers
{
    public class SellersController : Controller
    {
        private readonly DBContext _context;

        public SellersController(DBContext context)
        {
            _context = context;
        }

        // GET: Sellers
        public async Task<IActionResult> Index(int? page)
        {
            // Pagination
            if (page == null || page < 1)
            {
                page = 1;
            }

            int PageSize = 10;

            //get sellers data
            var sellersQuery = _context.Sellers
              .Include(s => s.User)
              .Select(s => new UserSellerModel
              {
                  user = s.User,
                  seller = s
              });

            var totalCount = await sellersQuery.CountAsync(); //total num of results
            
            var sellersData = await sellersQuery
                .Skip((page.Value - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            var pagedList = new StaticPagedList<UserSellerModel>(
                sellersData, page.Value, PageSize, totalCount);

            return View(pagedList);

        }

        // GET: Sellers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seller = await _context.Sellers
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.SellerId == id);
            
            if (seller == null)
            {
                return NotFound();
            }

            var model = new UserSellerModel
            {
                user = seller.User,
                seller = seller
            };

            return View(model);
        }

        // GET: Sellers/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            return View();
        }

        // POST: Sellers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SellerId,UserId,TaxNumber")] Seller seller)
        {
            if (ModelState.IsValid)
            {
                _context.Add(seller);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", seller.UserId);
            return View(seller);
        }

        // GET: Sellers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seller = await _context.Sellers
                .Include(s => s.User) //load the user
                .FirstOrDefaultAsync(s => s.SellerId == id);

            if (seller == null)
            {
                return NotFound();
            }

            var model = new UserSellerModel
            {
                user = seller.User,
                seller = seller
            };

            //ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", seller.UserId);
            return View(model);
        }

        // POST: Sellers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserSellerModel model)
        {
            if (id != model.seller.SellerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var seller = await _context.Sellers
                       .Include(s => s.User)
                       .FirstOrDefaultAsync(s => s.SellerId == id);

                    if (seller == null)
                    {
                        return NotFound();
                    }

                    //check if the username exists
                    var usernameExists = await _context.Users
                        .FirstOrDefaultAsync(u => u.Username == model.user.Username
                        && u.UserId != seller.User.UserId);

                    if (usernameExists != null)
                    {
                        ModelState.AddModelError("user.Username", "This username is already in use. Please enter a unique one.");
                        return View(model);
                    }

                    if (seller.User.Username != model.user.Username)
                    {
                        seller.User.Username = model.user.Username;
                    }

                    //check if the taxNumber is unique
                    var taxNumberExists = await _context.Sellers
                        .AnyAsync(s => s.TaxNumber == model.seller.TaxNumber && 
                        s.UserId != seller.User.UserId);

                    if (taxNumberExists)
                    {
                        ModelState.AddModelError("seller.TaxNumber", "This tax number already exists. Please enter a unique one.");
                        return View(model);
                    }

                    if(model.seller.TaxNumber.Value.ToString().Length < 9)
                    {
                        ModelState.AddModelError("seller.TaxNumber", "The tax number must be exactly 9 digits");
                        return View(model);
                    }

                    if (seller.TaxNumber != model.seller.TaxNumber)
                    {
                        //update data of seller 
                        seller.TaxNumber = model.seller.TaxNumber;
                    }


                    //update data of user
                    seller.User.FirstName = model.user.FirstName;
                    seller.User.LastName = model.user.LastName;
                    seller.User.Email = model.user.Email;
                    seller.User.Password = model.user.Password;

                    _context.Update(seller.User);
                    _context.Update(seller);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SellerExists(model.seller.SellerId))
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
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", model.seller.UserId);
            return View(model);
        }

        // GET: Sellers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seller = await _context.Sellers
               .Include(s => s.User)
               .FirstOrDefaultAsync(m => m.SellerId == id);

            if (seller == null)
            {
                return NotFound();
            }

            var model = new UserSellerModel
            {
                user = seller.User,
                seller = seller
            };

            return View(model);
        }

        // POST: Sellers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var seller = await _context.Sellers
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.SellerId == id);
            //var seller = await _context.Sellers.FindAsync(id);
            if (seller != null)
            {
                _context.Sellers.Remove(seller);
                _context.Users.Remove(seller.User);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SellerExists(int id)
        {
            return _context.Sellers.Any(e => e.SellerId == id);
        }
    }
}

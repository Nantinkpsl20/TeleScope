using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using NuGet.Protocol.Plugins;
using teleScope.Models;
using X.PagedList;

namespace teleScope.Controllers
{
    public class CustomersController : Controller
    {
        private readonly DBContext _context;

        public CustomersController(DBContext context)
        {
            _context = context;
        }

        // GET: Customers
        public async Task<IActionResult> Index(int? page)
        {
            // Pagination
            if (page == null || page < 1)
            {
                page = 1;
            }

            int PageSize = 10;

            var customersQuery = _context.Customers
              .Include(s => s.User)
              .Include(s => s.PhoneNumbers)
              .ThenInclude(pn => pn.Program)
              .Select(s => new UserCustomerModel
              {
                  user = s.User,
                  customer = s,
                  phoneNumber = s.PhoneNumbers.FirstOrDefault()
              });

            var totalCount = await customersQuery.CountAsync(); //total num of results

            var customersData = await customersQuery
                .Skip((page.Value - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            var pagedList = new StaticPagedList<UserCustomerModel>(
                customersData, page.Value, PageSize, totalCount);

            return View(pagedList);
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .Include(c => c.User)
                .Include(c => c.PhoneNumbers)
                .ThenInclude(pn => pn.Program)
                .FirstOrDefaultAsync(m => m.CustomerId == id);

            if (customer == null)
            {
                return NotFound();
            }

            var model = new UserCustomerModel
            {
                user = customer.User,
                customer = customer,
                phoneNumber = customer.PhoneNumbers.FirstOrDefault()
            };

            //ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", seller.UserId);
            return View(model);

        }

        // GET: Customers/Create
        public IActionResult Create()
        {

            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,UserId")] Customer customer)
        {
            if (ModelState.IsValid)
            {              
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", customer.UserId);
            return View(customer);
        }

        //function that calculate the total amount of the bill
        public decimal totalBillAmount(IEnumerable<Call> calls, Programme program)
        {
            decimal totalAmount = program.FixedCost;

            //total landline minutes
            var totalLandlineMin = calls
                .Where(c => c.DestinationNumber.StartsWith("21") && c.DestinationNumber.Length >= 10)
                .Sum(c => c.Duration);

            //total mobile minutes
            var totalMobileMin = calls
                .Where(c => c.DestinationNumber.StartsWith("69") && c.DestinationNumber.Length >= 10)
                .Sum(c => c.Duration);

            //cost for 5 digit calls
            var total5DigitCost = calls
                .Where(c => c.DestinationNumber.Length == 5)
                .Sum(c => c.Duration * program.FiveDigitFee);

            //check for extra landline minutes
            if (totalLandlineMin > program.LandlineMinutes)
            {
                var extraLandlineMin = totalLandlineMin - program.LandlineMinutes;
                totalAmount += extraLandlineMin * program.LandlineFee;
            }

            //check for extra mobile minutes
            if (totalMobileMin > program.MobileMinutes)
            {
                var extraMobileMin = totalMobileMin - program.MobileMinutes;
                totalAmount += extraMobileMin * program.MobileFee;
            }

            return totalAmount + total5DigitCost;

        }



        public async Task<decimal> CalculateBill(int customerId)
        {
            try
            {
                // Βρίσκουμε τον πελάτη
                var customer = await _context.Customers
                    .Include(c => c.PhoneNumbers)
                    .ThenInclude(pn => pn.Program)
                    .FirstOrDefaultAsync(c => c.CustomerId == customerId);

                if (customer == null)
                {
                    return 0;
                }

                var phoneNumber = customer.PhoneNumbers.FirstOrDefault();
                if (phoneNumber == null)
                {
                    return 0;
                }

                var program = phoneNumber.Program;
                if (program == null)
                {
                    return 0;
                }

                // Υπολογισμός περιόδου χρέωσης
                var lastBill = await _context.Bills
                    .Where(b => b.CustomerId == customer.CustomerId)
                    .OrderByDescending(b => b.IssueDate)
                    .FirstOrDefaultAsync();

                DateTime startDate = lastBill?.IssueDate ?? phoneNumber.CreatedAt ?? DateTime.UtcNow;
                DateTime endDate = DateTime.Now;

                // Εύρεση κλήσεων
                var calls = _context.Calls
                    .Where(c => c.PhoneId == phoneNumber.PhoneId
                                && c.CallDate >= startDate
                                && c.CallDate <= endDate)
                    .ToList();

                // Υπολογισμός κόστους
                decimal totalCost = program.FixedCost;
                if (calls.Any())
                {
                    totalCost = totalBillAmount(calls, program);
                }

                return totalCost;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }



        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
               .Include(c => c.User) //load the user
               .Include(c => c.PhoneNumbers)
               .FirstOrDefaultAsync(c => c.CustomerId == id);

            if (customer == null)
            {
                return NotFound();
            }

            var model = new UserCustomerModel
            {
                user = customer.User,
                customer = customer,
                phoneNumber = customer.PhoneNumbers.FirstOrDefault()
            };

            var bill = new Bill
            {
                IssueDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(31)
            };


            ViewData["ProgramId"] = new SelectList(_context.Programmes, "ProgramId", "ProgrName");
            return View(model);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserCustomerModel model)
        {
            if (id != model.customer.CustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var customer = await _context.Customers
                       .Include(s => s.User)
                       .Include(s => s.PhoneNumbers)
                       .FirstOrDefaultAsync(s => s.CustomerId == id);

                    if (customer == null)
                    {
                        return NotFound();
                    }

                    //check if the username exists
                    var usernameExists = await _context.Users
                        .FirstOrDefaultAsync(u => u.Username == model.user.Username
                        && u.UserId != customer.User.UserId);

                    if (usernameExists != null)
                    {
                        ModelState.AddModelError("user.Username", "This username is already in use. Please enter a unique one.");
                        ViewData["ProgramId"] = new SelectList(_context.Programmes, "ProgramId", "ProgrName");
                        return View(model);
                    }

                    if (customer.User.Username != model.user.Username) 
                    {
                        customer.User.Username = model.user.Username;
                    }

                    var phoneNumber = await _context.PhoneNumbers
                        .FirstOrDefaultAsync(c => c.CustomerId == model.customer.CustomerId);

                    if (phoneNumber == null)
                    {
                        return NotFound();
                    }

                    decimal billCost = 0;
                    //if he wants to change the program
                    if (model.phoneNumber.ProgramId != phoneNumber.ProgramId)
                    {
                        billCost = await CalculateBill(customer.CustomerId);

                        if (billCost == 0)
                        {
                            ModelState.AddModelError("", "The bill does not created. You cannot change the program right now");
                            ViewData["ProgramId"] = new SelectList(_context.Programmes, "ProgramId", "ProgrName");
                            return View(model);
                        }
                        else
                        {
                            phoneNumber.ProgramId = model.phoneNumber.ProgramId;
                        }

                    }

                    //update data of user
                    customer.User.FirstName = model.user.FirstName;
                    customer.User.LastName = model.user.LastName;
                    customer.User.Email = model.user.Email;
                    customer.User.Password = model.user.Password;

                    _context.Update(customer.User);
                    _context.Update(customer);

                    
                     //update data of phoneNumber 
                     phoneNumber.Phone = model.phoneNumber.Phone;
                     phoneNumber.PhoneType = model.phoneNumber.PhoneType;


                    _context.Update(phoneNumber);

                    var bill = new Bill
                    {
                        CustomerId = customer.CustomerId,
                        IssueDate = DateTime.Now,
                        DueDate = DateTime.Now.AddDays(31),
                        TotalAmount = billCost
                    };

                    _context.Bills.Add(bill);
                    await _context.SaveChangesAsync();


                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(model.customer.CustomerId))
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
            ViewData["ProgramId"] = new SelectList(_context.Programmes, "ProgramId", "ProgrName");
            return View(model);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .Include(c => c.User)
                .Include(c => c.PhoneNumbers)
                .ThenInclude(pn => pn.Program)
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            
            if (customer == null)
            {
                return NotFound();
            }    

            var model = new UserCustomerModel
            {
                user = customer.User,
                customer = customer,
                phoneNumber = customer.PhoneNumbers.FirstOrDefault()
            };

            return View(model);

        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers
                .Include(s => s.User)
                .Include(c => c.PhoneNumbers)
                .FirstOrDefaultAsync(m => m.CustomerId == id);

            if (customer != null)
            {
                _context.Users.Remove(customer.User);
                _context.PhoneNumbers.Remove(customer.PhoneNumbers.FirstOrDefault());
                _context.Customers.Remove(customer);   
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}

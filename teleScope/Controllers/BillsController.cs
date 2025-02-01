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
    public class BillsController : Controller
    {
        private readonly DBContext _context;

        public BillsController(DBContext context)
        {
            _context = context;
        }

        // GET: Bills
        public async Task<IActionResult> Index(int? page)
        {
            // Pagination
            if (page == null || page < 1)
            {
                page = 1;
            }

            int PageSize = 8;

            //get bills data
            var billsQuery = _context.Bills
                .Include(b => b.Customer)
                   .ThenInclude(s => s.User)
                .Include(b => b.Customer)
                  .ThenInclude(b => b.PhoneNumbers)
                    .ThenInclude(pn => pn.Program)
                 .Select(b => new BillsUserModel
                    {
                        user = b.Customer.User,
                        customer = b.Customer,
                        phoneNumber = b.Customer.PhoneNumbers.FirstOrDefault(),
                        bill = b
                    });

            var totalCount = await billsQuery.CountAsync(); //total num of results

            var billsData = await billsQuery
                .Skip((page.Value - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            var pagedList = new StaticPagedList<BillsUserModel>(
                billsData, page.Value, PageSize, totalCount);

            return View(pagedList);
            
        }

        // GET: Bills
        public async Task<IActionResult> customerBills(int? page)
        {
            // Pagination
            if (page == null || page < 1)
            {
                page = 1;
            }

            int PageSize = 8;

            //get bills data
            var billsQuery = _context.Bills
                .Include(b => b.Customer)
                   .ThenInclude(s => s.User)
                .Include(b => b.Customer)
                  .ThenInclude(b => b.PhoneNumbers)
                    .ThenInclude(pn => pn.Program)
                 .Where(b => b.CustomerId == 36)
                 .Select(b => new BillsUserModel
                 {
                     user = b.Customer.User,
                     customer = b.Customer,
                     phoneNumber = b.Customer.PhoneNumbers.FirstOrDefault(),
                     bill = b
                 });
                 

            var totalCount = await billsQuery.CountAsync(); //total num of results

            var billsData = await billsQuery
                .Skip((page.Value - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            var pagedList = new StaticPagedList<BillsUserModel>(
                billsData, page.Value, PageSize, totalCount);

            return View(pagedList);
        }

        // GET: Bills/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //get bills data
            var bill = await _context.Bills
                .Include(b => b.Customer)
                   .ThenInclude(s => s.User)
                .Include(b => b.Customer)
                  .ThenInclude(b => b.PhoneNumbers)
                    .ThenInclude(pn => pn.Program)
                .Where(b => b.BillId == id)
                .Select(b => new BillsUserModel
                {
                    user = b.Customer.User,
                    customer = b.Customer,
                    phoneNumber = b.Customer.PhoneNumbers.FirstOrDefault(),
                    bill = b
                }).FirstOrDefaultAsync();

            if (bill == null)
            {
                return NotFound();
            }

            return View(bill);
        }

        // GET: Bills/Create
        public IActionResult Create()
        {
            var customerList = _context.Customers
                 .Include(c => c.User)
                 .Include(c => c.PhoneNumbers)
                 .Select(c => new { CustomerId = c.CustomerId, Username = c.User.Username })
                 .ToList();

            var model = new CustomerBill
            {
                bill = new Bill
                {
                    IssueDate = DateTime.Now,
                    DueDate = DateTime.Now.AddDays(31)
                },

                user = new User(),
                phoneNumber = new PhoneNumber()

            };


            ViewData["CustomerId"] = new SelectList(customerList, "CustomerId", "Username");
            
            return View(model);
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

        [HttpGet]
        public async Task<IActionResult> CalculateBill(int customerId)
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
                    return Json(new { success = false, message = "Customer not found." });
                }

                var phoneNumber = customer.PhoneNumbers.FirstOrDefault();
                if (phoneNumber == null)
                {
                    return Json(new { success = false, message = "Phone number not found for this customer." });
                }

                var program = phoneNumber.Program;
                if (program == null)
                {
                    return Json(new { success = false, message = "Program not found for this phone number." });
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

                return Json(new { success = true, totalCost });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }


        public async Task<IActionResult> getDetailsByCustomerId(int customerId)
        {
            var customer = await _context.Customers
                 .Include(c => c.User)
                 .Include(c => c.PhoneNumbers)
                 .ThenInclude(pn => pn.Program)
                 .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null)
            {
                return Json(new { success = false });
            }

            var customerNumber = customer.PhoneNumbers.FirstOrDefault();
            var customerProgram = customerNumber?.Program?.ProgrName ?? "no program found";

            //return customer details as Json
            var customerDetails = new
            {
                firstName = customer.User.FirstName,
                lastName = customer.User.LastName,
                phone = customerNumber?.Phone,
                programName = customerProgram

            };

            return Json(customerDetails);
        }

        // POST: Bills/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerBill customerBill)
        {
            
            if (ModelState.IsValid)
            {
                if (customerBill.bill.CustomerId == 0 || customerBill.bill.CustomerId == null)
                {
                       ModelState.AddModelError("", "Please select a customer");
                       return View(customerBill.bill);
                }
            
                var customer = await _context.Customers
              .Include(c => c.User) //load the user
              .Include(c => c.PhoneNumbers)
              .FirstOrDefaultAsync(c => c.CustomerId == customerBill.bill.CustomerId);

              if (customer != null)
              {
                customerBill.user = customer.User;
                customerBill.phoneNumber = customer.PhoneNumbers.FirstOrDefault();
              }



                _context.Add(customerBill.bill);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var customerList = _context.Customers
                .Include(c => c.User)
                .Select(c => new { CustomerId = c.CustomerId, Username = c.User.Username })
                .ToList();

            ViewData["CustomerId"] = new SelectList(customerList, "CustomerId", "Username", customerBill.bill.CustomerId);
            return View(customerBill);
        }


        // GET: Bills/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bill = await _context.Bills.FindAsync(id);
            if (bill == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", bill.CustomerId);
            return View(bill);
        }

        // POST: Bills/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BillId,CustomerId,IssueDate,DueDate,TotalAmount,IsPaid")] Bill bill)
        {
            if (id != bill.BillId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bill);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BillExists(bill.BillId))
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", bill.CustomerId);
            return View(bill);
        }

        // GET: Bills/Edit/5
        public async Task<IActionResult> paymentBill(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bill = await _context.Bills.FindAsync(id);
            if (bill == null)
            {
                return NotFound();
            }
            return View(bill);
        }

        // POST: Bills/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> paymentBill(int id, [Bind("BillId,IsPaid")] Bill bill)
        {
            if (id != bill.BillId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingBill = await _context.Bills.FindAsync(id);

                    if (existingBill == null) { return NotFound(); }

                    existingBill.IsPaid = bill.IsPaid;
                   
                    _context.Update(existingBill);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BillExists(bill.BillId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("customerBills");
            }
            return View(bill);
        }


        // GET: Bills/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //get bills data
            var bill = await _context.Bills
                .Include(b => b.Customer)
                   .ThenInclude(s => s.User)
                .Include(b => b.Customer)
                  .ThenInclude(b => b.PhoneNumbers)
                    .ThenInclude(pn => pn.Program)
                .Where(b => b.BillId == id)
                .Select(b => new BillsUserModel
                {
                    user = b.Customer.User,
                    customer = b.Customer,
                    phoneNumber = b.Customer.PhoneNumbers.FirstOrDefault(),
                    bill = b
                }).FirstOrDefaultAsync();

            if (bill == null)
            {
                return NotFound();
            }

            return View(bill);
        }

        // POST: Bills/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            //get bills data
            var bill = await _context.Bills
                .Include(b => b.Customer)
                   .ThenInclude(s => s.User)
                .Include(b => b.Customer)
                  .ThenInclude(b => b.PhoneNumbers)
                    .ThenInclude(pn => pn.Program)
              .FirstOrDefaultAsync(b => b.BillId == id);

            if (bill != null)
            {
                _context.Bills.Remove(bill);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BillExists(int id)
        {
            return _context.Bills.Any(e => e.BillId == id);
        }
    }
}

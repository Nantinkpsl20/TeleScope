using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using teleScope.Models;

namespace teleScope.Controllers
{
    public class UsersController : Controller
    {
        private readonly DBContext _context;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly ILogger<UsersController> _logger;


        public UsersController(DBContext context, ILogger<UsersController> logger)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
            _logger = logger;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create Admin
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserAdminModel model)
        {
            if (ModelState.IsValid)
            {
                var usernameExists = await _context.Users
                   .FirstOrDefaultAsync(u => u.Username == model.user.Username);

                if (usernameExists != null)
                {
                    ModelState.AddModelError("user.Username", "This username is already in use. Please enter a unique one.");
                    return View(model);
                }

                var user = new User
                {
                    FirstName = model.user.FirstName,
                    LastName = model.user.LastName,
                    Username = model.user.Username,
                    Password = model.user.Password,
                    Email = model.user.Email,
                    UserRole = model.user.UserRole

                };

                user.Password = _passwordHasher.HashPassword(user, user.Password);

                _context.Add(user);
                await _context.SaveChangesAsync();

                int userId = user.UserId;

                 Admin admin = new Admin()
                 {
                    UserId = userId
                 };

                  _context.Admins.Add(admin);
                  await _context.SaveChangesAsync();

                   return RedirectToAction("Index", "Admins");
                }
            return View(model);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,FirstName,LastName,Username,Password,Email,UserRole")] User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
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
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }

        //----------Login------------//
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(User user)
        {

            var AppUser = _context.Users.Where(x => x.Username == user.Username).FirstOrDefault();

            if (AppUser != null)
            {
                var result = _passwordHasher
                    .VerifyHashedPassword(user, AppUser.Password, user.Password);
                if (result == PasswordVerificationResult.Success)
                {
                    return RedirectToAction("Index", "Home", new { area = "" });
                }
                else
                {
                    ViewBag.Message = "the password is incorrect";
                    return View();
                }

            }
            else
            {
                ViewBag.Message = "invalid username or password";
            }

            return View();

        }

        // GET: Users/Create
        public IActionResult CreateSeller()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSeller(UserSellerModel model)
        {
            if (ModelState.IsValid)
            {
                //check if the Username is unique
                var usernameExists = await _context.Users.
                    AnyAsync(u => u.Username == model.user.Username);

                if (usernameExists)
                {
                    ModelState.AddModelError("user.Username", "This username is already in use. Please enter a unique one.");
                    return View(model);
                }

                //check if the taxNumber is unique
                var taxNumberExists = await _context.Sellers
                    .AnyAsync(s => s.TaxNumber == model.seller.TaxNumber);

                if (taxNumberExists)
                {
                    ModelState.AddModelError("seller.TaxNumber", "This tax number already exists. Please enter a unique one.");
                    return View(model);
                }

                if (model.seller.TaxNumber.Value.ToString().Length < 9)
                {
                    ModelState.AddModelError("seller.TaxNumber", "The tax number must be exactly 9 digits");
                    return View(model);
                }

                var user = new User
                {
                    FirstName = model.user.FirstName,
                    LastName = model.user.LastName,
                    Username = model.user.Username,
                    Password = model.user.Password,
                    Email = model.user.Email,
                    UserRole = model.user.UserRole

                };

                user.Password = _passwordHasher.HashPassword(user, user.Password);

                _context.Add(user);
                await _context.SaveChangesAsync();

                int userId = user.UserId;

                var seller = new Seller()
                {
                    UserId = userId,
                    TaxNumber = model.seller.TaxNumber
                };

                _context.Sellers.Add(seller);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Sellers");
            }
            return View(model);
        }

        // GET: Users/Create
        public IActionResult CreateCustomer()
        {
            ViewData["ProgramId"] = new SelectList(_context.Programmes, "ProgramId", "ProgrName");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCustomer(UserCustomerModel model)
        {

            if (ModelState.IsValid)
            {
                //check if the username exists
                var usernameExists = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == model.user.Username);

                if (usernameExists != null) 
                {
                    ModelState.AddModelError("user.Username", "This username is already in use. Please enter a unique one.");
                    ViewData["ProgramId"] = new SelectList(_context.Programmes, "ProgramId", "ProgrName");
                    return View(model);
                }

                //check if the phoneNumber is unique
                var phoneNumberExists = await _context.PhoneNumbers
                    .AnyAsync(s => s.Phone == model.phoneNumber.Phone);

                if (phoneNumberExists)
                {
                    ModelState.AddModelError("phoneNumber.Phone", "This phone number already exists. Please enter a unique one.");
                    ViewData["ProgramId"] = new SelectList(_context.Programmes, "ProgramId", "ProgrName");
                    return View(model);
                }

                if (model.phoneNumber.Phone.Length != 10)
                {
                    ModelState.AddModelError("phoneNumber.Phone", "The phone number must be exactly 10 digits");
                    ViewData["ProgramId"] = new SelectList(_context.Programmes, "ProgramId", "ProgrName");
                    return View(model);
                }

                var user = new User
                {
                    FirstName = model.user.FirstName,
                    LastName = model.user.LastName,
                    Username = model.user.Username,
                    Password = model.user.Password,
                    Email = model.user.Email,
                    UserRole = model.user.UserRole

                };

                user.Password = _passwordHasher.HashPassword(user, user.Password);

                _context.Add(user);
                await _context.SaveChangesAsync();

                int userId = user.UserId;

                Customer customer = new Customer()
                {
                    UserId = userId
                };

                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();


                int customerId = customer.CustomerId;

                PhoneNumber phoneNum = new PhoneNumber()
                {
                    Phone = model.phoneNumber.Phone,
                    PhoneType = model.phoneNumber.PhoneType,
                    ProgramId = model.phoneNumber.ProgramId,
                    CustomerId = customerId,
                    CreatedAt = DateTime.Now
                };

                 _context.PhoneNumbers.Add(phoneNum);
                 await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Customers");
            }
            else
            {
                ViewData["ProgramId"] = new SelectList(_context.Programmes, "ProgramId", "ProgrName");
            }
            return View(model);
        }

    }
}

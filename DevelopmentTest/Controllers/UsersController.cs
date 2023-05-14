using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DevelopmentTest.Models;
using DevelopmentTest.ViewModels;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using NuGet.Common;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;

namespace DevelopmentTest.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        string BaseUrl = "https://api-sport-events.php6-02.test.voxteneo.com/api/v1/users/";

        // GET: Users
        public async Task<IActionResult> Index()
        {
            //return _context.Users != null ? 
            //            View(await _context.Users.ToListAsync()) :
            //            Problem("Entity set 'ApplicationDbContext.Users'  is null.");
            return View();
        }

        // Post: Users/Login
        public async Task<IActionResult> Login([Bind("Email,Password")] UserLogin user)
        {
            var userInfo = new CreateUserResponse();
            if (ModelState.IsValid)
            {

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BaseUrl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var content = new FormUrlEncodedContent(new[]
                    {
                            new KeyValuePair<string, string>("email", user.Email),
                            new KeyValuePair<string, string>("password", user.Password)
                        });
                    HttpResponseMessage Res = await client.PostAsync("login", content);
                    if (Res.IsSuccessStatusCode)
                    {
                        var userResponse = Res.Content.ReadAsStringAsync().Result;
                        userInfo = JsonConvert.DeserializeObject<CreateUserResponse>(userResponse);
                    }
                }
            }
            // Membuat objek cookie
            var cookieOptions = new CookieOptions
            {
                Path = "/",
                Expires = DateTime.UtcNow.AddHours(1),
                IsEssential = true, // Opsional, untuk menandai cookie yang penting
                HttpOnly = true, // Opsional, untuk melindungi cookie dari akses JavaScript
                Secure = true // Opsional, untuk mengirim cookie hanya melalui protokol HTTPS
            };

            // Menetapkan nilai cookie dengan token
            Response.Cookies.Append("TokenCookie", userInfo.Token, cookieOptions);

            return RedirectToAction(nameof(Details), new {id = userInfo.Id});
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            string token = Request.Cookies["TokenCookie"];
            var user = new User();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token); ;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage Res = await client.GetAsync($"{id}");
                if (Res.IsSuccessStatusCode)
                {
                    var userResponse = Res.Content.ReadAsStringAsync().Result;
                    user = JsonConvert.DeserializeObject<User>(userResponse);
                }
            }

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

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Email,Password,RepeatPassword")] CreateUserRequest user)
        {
            var userRes = new CreateUserResponse();
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BaseUrl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var content = new FormUrlEncodedContent(new[]
                    {
                            new KeyValuePair<string, string>("firstName", user.FirstName),
                            new KeyValuePair<string, string>("lastName", user.LastName),
                            new KeyValuePair<string, string>("email", user.Email),
                            new KeyValuePair<string, string>("Password", user.Password),
                            new KeyValuePair<string, string>("repeatPassword", user.RepeatPassword)
                        });
                    HttpResponseMessage Res = await client.PostAsync("", content);
                    if (Res.IsSuccessStatusCode)
                    {
                        var userResponse = Res.Content.ReadAsStringAsync().Result;
                        userRes = JsonConvert.DeserializeObject<CreateUserResponse>(userResponse);
                    }
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            string token = Request.Cookies["TokenCookie"];
            var user = new User();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token); 
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage Res = await client.GetAsync($"{id}");
                if (Res.IsSuccessStatusCode)
                {
                    var userResponse = Res.Content.ReadAsStringAsync().Result;
                    user = JsonConvert.DeserializeObject<User>(userResponse);
                }
            }

            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Email")] User user)
        {
            string token = Request.Cookies["TokenCookie"];
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (user != null)
                    {
                        using (var client = new HttpClient())
                        {
                            client.BaseAddress = new Uri(BaseUrl);
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                            var content = new FormUrlEncodedContent(new[]
                            {
                            new KeyValuePair<string, string>("firstName", user.FirstName),
                            new KeyValuePair<string, string>("lastName", user.LastName),
                            new KeyValuePair<string, string>("email", user.Email)
                        });
                            HttpResponseMessage Res = await client.PutAsync($"{id}", content);
                            if (Res.IsSuccessStatusCode)
                            {
                                var userResponse = Res.Content.ReadAsStringAsync().Result;
                                user = JsonConvert.DeserializeObject<User>(userResponse);
                            }
                        }
                    }
                    return RedirectToAction(nameof(Details), new { id = id });
                }
                catch (Exception ex)
                {
                        throw new ArgumentException(ex.InnerException.Message??ex.Message);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
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
            if (_context.Users == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Users'  is null.");
            }
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
          return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

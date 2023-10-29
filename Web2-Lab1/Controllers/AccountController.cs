using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SampleMvcApp.ViewModels;
using System.Linq;
using System.Security.Claims;
using Auth0.AspNetCore.Authentication;
using SampleMvcApp.Data;
using SampleMvcApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace SampleMvcApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly DataContext _context;

        public AccountController(DataContext context) 
        {
            this._context = context;
        }

        public async Task<IActionResult> LoginCallback()
        {
            var userIdentifier = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userIdentifier))
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserIdentifier == userIdentifier);

                if (user == null)
                {
                    user = new User
                    {
                        UserIdentifier = userIdentifier
                    };
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task Login(string returnUrl = "/Account/LoginCallback")
        {
            var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
                .WithRedirectUri(returnUrl)
                .Build();

            await HttpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
        }

        [Authorize]
        public async Task Logout()
        {
            var authenticationProperties = new LogoutAuthenticationPropertiesBuilder()
                // Indicate here where Auth0 should redirect the user after a logout.
                // Note that the resulting absolute Uri must be whitelisted in 
                .WithRedirectUri(Url.Action("Index", "Home"))
                .Build();

            await HttpContext.SignOutAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userIdentifier = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserIdentifier == userIdentifier);
            List<Natjecanje> natjecanja = await _context.Natjecanja.Where(n => n.UserId == user.Id).ToListAsync();

            return View(new UserNatjecanjaViewModel
            {
                Natjecanja = natjecanja
            });
        }

        [Authorize]
        public async Task<IActionResult> Natjecanje(int id)
        {
            var natjecanje = await _context.Natjecanja.FindAsync(id);
            
            List<Natjecatelj> natjecatelji = await _context.Natjecatelji.Where(n => n.NatjecanjeId == id).ToListAsync();
            List<int> natjecateljiId = natjecatelji.Select(n => n.Id).ToList();
            
            List<Rezultat> rezultati = await _context.Rezultati
                                               .Where(r => natjecateljiId.Contains(r.IdDomacin) || natjecateljiId.Contains(r.IdGost))
                                               .OrderBy(r => r.Kolo)
                                               .ToListAsync();

            return View( new NatjecanjeViewModel
            {
                Natjecanje = natjecanje,
                Natjecatelji = natjecatelji,
                Rezultati = rezultati
            });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Publish(int id)
        {
            var natjecanje = await _context.Natjecanja.FindAsync(id);
            natjecanje.IsPublic = true;
            natjecanje.PublicUrl = Url.Action("Natjecanje", "Home", new { id }, protocol: HttpContext.Request.Scheme);

            await _context.SaveChangesAsync();

            return RedirectToAction("Profile", "Account");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}

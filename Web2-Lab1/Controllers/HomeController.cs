using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SampleMvcApp.Data;
using SampleMvcApp.Data.Entities;
using SampleMvcApp.ViewModels;

namespace SampleMvcApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _context;

        public HomeController(DataContext context)
        {
            this._context = context;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("AuthenticatedIndex");
            }
            
            return View();
        }

        [Authorize]
        public IActionResult AuthenticatedIndex()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateNatjecanje(NatjecanjeInputViewModel inputModel)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(inputModel.Naziv))
                {
                    ModelState.AddModelError("Naziv", "Naziv natjecanja je obavezan.");
                }

                if (string.IsNullOrWhiteSpace(inputModel.PopisNatjecatelja))
                {
                    ModelState.AddModelError("PopisNatjecatelja", "Popis natjecatelja je obavezan.");
                } 
                else
                {
                    if (!Regex.IsMatch(inputModel.PopisNatjecatelja, @"^(\w+(\s\w+)?[;\n\r]*){4,8}$"))
                    {
                        ModelState.AddModelError("PopisNatjecatelja", "Neispravan format popisa natjecatelja.");
                    }
                }

                
                if (!Regex.IsMatch(inputModel.SustavBodovanja, @"^\d+/\d+/\d+$"))
                {
                    ModelState.AddModelError("SustavBodovanja", "Neispravan format sustava bodovanja. Ispravni format je 'pobjeda/remi/poraz'.");
                }

            }

            if (ModelState.IsValid)
            {
                var bodovanjeParts = inputModel.SustavBodovanja.Split('/');

                var userIdentifier = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserIdentifier == userIdentifier);

                var natjecanje = new Natjecanje
                {
                    Naziv = inputModel.Naziv,
                    BodoviPobjeda = int.Parse(bodovanjeParts[0]),
                    BodoviRemi = int.Parse(bodovanjeParts[1]),
                    BodoviPoraz = int.Parse(bodovanjeParts[2]),
                    IsPublic = false,
                    PublicUrl = null,
                    User = user
                };

                _context.Natjecanja.Add(natjecanje);

                var popisNatjecatelja = inputModel.PopisNatjecatelja.Split(new[] { ';', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var natjecateljNaziv in popisNatjecatelja)
                {
                    var natjecatelj = new Natjecatelj
                    {
                        Naziv = natjecateljNaziv,
                        BrojBodova = 0,
                        Razlika = 0,
                        BrojOdigranihKola = 0,
                        Natjecanje = natjecanje
                    };
                    _context.Natjecatelji.Add(natjecatelj);
                }

                await _context.SaveChangesAsync();

                List<Natjecatelj> natjecatelji = await _context.Natjecatelji
                                                               .Where(n => n.NatjecanjeId == natjecanje.Id)
                                                               .ToListAsync();

                var parovi = Parovi(natjecatelji);

                foreach(var par in parovi)
                {
                    var rezultat = new Rezultat
                    {
                        Domacin = par.Item1,
                        Gost = par.Item2,
                        Kolo = par.Item3,
                        Ishod = 3,
                        Vrijednost = null
                    };
                    _context.Rezultati.Add(rezultat);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            
            return View("AuthenticatedIndex", inputModel);
        }

        public IActionResult Error()
        {
            return View();
        }

        public List<Tuple<Natjecatelj, Natjecatelj, int>> Parovi(List<Natjecatelj> natjecatelji)
        {
            int brojNatjecatelja = natjecatelji.Count;

            if (brojNatjecatelja % 2 != 0)
            {
                natjecatelji.Add(null);
                brojNatjecatelja++;
            }

            List<Tuple<Natjecatelj, Natjecatelj, int>> parovi = new List<Tuple<Natjecatelj, Natjecatelj, int>>();

            int brojKola = brojNatjecatelja - 1;
            var indeksi = new List<int>();

            for (int i = 0; i < brojNatjecatelja; i++)
            {
                indeksi.Add(i);
            }

            for (var kolo = 1; kolo <= brojKola; kolo++)
            {
                for (int i = 0; i < brojNatjecatelja / 2; i++) 
                {
                    int j = brojNatjecatelja - i - 1;

                    Natjecatelj domacin;
                    Natjecatelj gost;

                    if (kolo % 2 == 0)
                    {
                        domacin = natjecatelji[indeksi[i]];
                        gost = natjecatelji[indeksi[j]];
                    }
                    else
                    {
                        domacin = natjecatelji[indeksi[j]];
                        gost = natjecatelji[indeksi[i]];
                    }

                    if (domacin != null && gost != null)
                    {
                        Tuple<Natjecatelj, Natjecatelj, int> susret = Tuple.Create(domacin, gost, kolo);
                        parovi.Add(susret);
                    }
                }

                int indeks = indeksi[1];
                indeksi.RemoveAt(1);
                indeksi.Add(indeks);

            }
            return parovi;
        }

        public async Task<IActionResult> Natjecanje(int id)
        {
            var natjecanje = await _context.Natjecanja.FindAsync(id);

            if (User.Identity.IsAuthenticated)
            {
                var userIdentifier = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserIdentifier == userIdentifier);

                if (user.Id == natjecanje.UserId)
                {
                    return RedirectToAction("Natjecanje", "Account", new { id });
                }
            }

            if (!natjecanje.IsPublic)
            {
                return NotFound("Natjecanje nije javno za prikazivanje.");
            }

            List<Natjecatelj> natjecatelji = await _context.Natjecatelji.Where(n => n.NatjecanjeId == id).ToListAsync();
            List<int> natjecateljiId = natjecatelji.Select(n => n.Id).ToList();
            
            List<Rezultat> rezultati = await _context.Rezultati
                                               .Where(r => natjecateljiId.Contains(r.IdDomacin) || natjecateljiId.Contains(r.IdGost))
                                               .OrderBy(r => r.Kolo)
                                               .ToListAsync();

            return View(new NatjecanjeViewModel
            {
                Natjecanje = natjecanje,
                Natjecatelji = natjecatelji,
                Rezultati = rezultati
            });
        }
    }
}

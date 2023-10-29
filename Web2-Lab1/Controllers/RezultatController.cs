using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SampleMvcApp.Data;
using SampleMvcApp.ViewModels;

namespace SampleMvcApp.Controllers
{
    public class RezultatController : Controller
    {
        private readonly DataContext _context;

        public RezultatController(DataContext context)
        {
            this._context = context;
        }

        [Authorize]
        public async Task<IActionResult> Uredi(int id, int id2)
        {
            var rezultat = await _context.Rezultati.FirstOrDefaultAsync(r => r.IdDomacin == id && r.IdGost == id2);

            if (rezultat == null)
            {
                return NotFound("Rezultat nije pronađen!");
            }

            var domacin = await _context.Natjecatelji.FindAsync(rezultat.IdDomacin);
            var gost = await _context.Natjecatelji.FindAsync(rezultat.IdGost);

            return View(new RezultatViewModel
            {
                vrijednost = rezultat.Vrijednost,
                ishod = rezultat.Ishod,
                domacinId = domacin.Id,
                domacin = domacin.Naziv,
                gostId = gost.Id,
                gost = gost.Naziv
            });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Spremi(RezultatViewModel model)
        {
            if (model.vrijednost != null && !Regex.IsMatch(model.vrijednost, @"^\d+:\d+$"))
            {
                ModelState.AddModelError("Rezultat", "Rezultat nije prazan ili je nije u formatu 'broj:broj'");
            }

            if (!ModelState.IsValid)
            {
                return View("Uredi", model);
            }

            var rezultat = await _context.Rezultati.FirstOrDefaultAsync(r => r.IdDomacin == model.domacinId && r.IdGost == model.gostId);
            var domacin = await _context.Natjecatelji.FindAsync(model.domacinId);
            var gost = await _context.Natjecatelji.FindAsync(model.gostId);
            var natjecanje = await _context.Natjecanja.FindAsync(domacin.NatjecanjeId);

            if (natjecanje == null || rezultat == null || domacin == null || gost == null)
            {
                return BadRequest();
            }

            if (model.ishod != rezultat.Ishod)
            {
                if (rezultat.Ishod == 0)
                {
                    domacin.BrojBodova -= natjecanje.BodoviRemi;
                    gost.BrojBodova -= natjecanje.BodoviRemi;
                    domacin.BrojOdigranihKola -= 1;
                    gost.BrojOdigranihKola -= 1;
                }
                else if (rezultat.Ishod == 1)
                {
                    domacin.BrojBodova -= natjecanje.BodoviPobjeda;
                    gost.BrojBodova -= natjecanje.BodoviPoraz;
                    domacin.BrojOdigranihKola -= 1;
                    gost.BrojOdigranihKola -= 1;
                }
                else if (rezultat.Ishod == 2)
                {
                    domacin.BrojBodova -= natjecanje.BodoviPoraz;
                    gost.BrojBodova -= natjecanje.BodoviPobjeda;
                    domacin.BrojOdigranihKola -= 1;
                    gost.BrojOdigranihKola -= 1;
                }

                rezultat.Ishod = model.ishod;

                if (rezultat.Ishod == 0)
                {
                    domacin.BrojBodova += natjecanje.BodoviRemi;
                    gost.BrojBodova += natjecanje.BodoviRemi;
                    domacin.BrojOdigranihKola += 1;
                    gost.BrojOdigranihKola += 1;
                }
                else if (rezultat.Ishod == 1)
                {
                    domacin.BrojBodova += natjecanje.BodoviPobjeda;
                    gost.BrojBodova += natjecanje.BodoviPoraz;
                    domacin.BrojOdigranihKola += 1;
                    gost.BrojOdigranihKola += 1;
                }
                else if (rezultat.Ishod == 2)
                {
                    domacin.BrojBodova += natjecanje.BodoviPoraz;
                    gost.BrojBodova += natjecanje.BodoviPobjeda;
                    domacin.BrojOdigranihKola += 1;
                    gost.BrojOdigranihKola += 1;
                }
            }

            if (rezultat.Vrijednost != model.vrijednost)
            {
                string[] vrijednost;
                int razlika;

                if (rezultat.Vrijednost != null && rezultat.Vrijednost != "")
                {
                    vrijednost = rezultat.Vrijednost.Split(':');
                    razlika = int.Parse(vrijednost[0]) - int.Parse(vrijednost[1]);

                    domacin.Razlika -= razlika;
                    gost.Razlika += razlika;
                }

                rezultat.Vrijednost = model.vrijednost;

                if (rezultat.Vrijednost != null && rezultat.Vrijednost != "")
                {
                    vrijednost = rezultat.Vrijednost.Split(':');
                    razlika = int.Parse(vrijednost[0]) - int.Parse(vrijednost[1]);

                    domacin.Razlika += razlika;
                    gost.Razlika -= razlika;
                }
            }

            _context.Update(rezultat);
            _context.Update(domacin);
            _context.Update(gost);
            await _context.SaveChangesAsync();

            return RedirectToAction("Natjecanje", "Account", new { id = natjecanje.Id });
        }
    }
}

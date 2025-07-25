using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaGestionJudicial.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaGestionJudicial.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaGestionJudicial.Controllers
{
    public class PoliceController : Controller
    {
        private readonly ProyectoContext _context;

        public PoliceController(ProyectoContext context)
        {
            _context = context;
        }

        // Mostrar lista + datos para crear/editar en un solo ViewModel
        public async Task<IActionResult> Polices()
        {
            var partes = await _context.PartesPoliciales
                .Include(p => p.IdPersonaPoliciaNavigation)
                .Include(p => p.IdDenunciaNavigation)
                .ThenInclude(d => d.IdDelitoNavigation)
                .ToListAsync();

            var personas = await _context.Personas
                .Where(p => p.IdRol == 5)
                .Select(p => new
                {
                    p.IdPersona,
                    NombreCompleto = p.Nombres + " " + p.Apellidos + " - " + p.Cedula
                })
                .ToListAsync();

            var denuncias = await _context.Denuncias
                .Include(d => d.IdDelitoNavigation)
                .Select(d => new
                {
                    d.IdDenuncia,
                    Texto = d.Descripcion + (d.IdDelitoNavigation != null ? $" ({d.IdDelitoNavigation.Nombre})" : "")
                })
                //    Texto = d.IdDelitoNavigation != null ? $" {d.IdDelitoNavigation.Nombre}" : ""
                //})
                .ToListAsync();

            ViewBag.Personas = new SelectList(personas, "IdPersona", "NombreCompleto");
            ViewBag.Denuncias = new SelectList(denuncias, "IdDenuncia", "Texto");

            var viewModel = new PoliceViewModel
            {
                Partes = partes
            };

            return View("~/Views/Home/Polices.cshtml", viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PoliceViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var ultimoId = await _context.PartesPoliciales
                    .OrderByDescending(p => p.IdParte)
                    .Select(p => p.IdParte)
                    .FirstOrDefaultAsync();

                var parte = new PartesPoliciale
                {
                    IdParte = ultimoId + 1,
                    IdPersonaPolicia = viewModel.Id_Persona_Policia,
                    IdDenuncia = viewModel.Id_Denuncia,
                    Descripcion = viewModel.Descripcion,
                    FechaParte = viewModel.Fecha_Parte
                };

                _context.Add(parte);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Parte policial creado exitosamente.";
                return RedirectToAction(nameof(Polices));
            }
            await LoadDenunciasAndPersonasForViewBag();
            TempData["ErrorMessage"] = "Error al crear el parte policial. Verifique los datos.";
            return RedirectToAction(nameof(Polices));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PoliceViewModel viewModel)
        {
            if (viewModel == null || viewModel.Id_Parte == 0)
            {
                TempData["ErrorMessage"] = "Datos de parte policial inválidos.";
                return RedirectToAction(nameof(Polices));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var parte = await _context.PartesPoliciales.FindAsync(viewModel.Id_Parte);
                    if (parte == null)
                    {
                        TempData["ErrorMessage"] = "El parte policial que intenta editar no existe.";
                        return RedirectToAction(nameof(Polices));
                    }

                    parte.IdPersonaPolicia = viewModel.Id_Persona_Policia;
                    parte.IdDenuncia = viewModel.Id_Denuncia;
                    parte.Descripcion = viewModel.Descripcion;
                    parte.FechaParte = viewModel.Fecha_Parte;

                    _context.Update(parte);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Parte policial actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction(nameof(Polices));
            }

            await LoadDenunciasAndPersonasForViewBag();
            TempData["ErrorMessage"] = "Error al actualizar el parte policial. Verifique los datos.";
            return RedirectToAction(nameof(Polices));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            var parte = await _context.PartesPoliciales.FindAsync(id);
            if (parte != null)
            {
                _context.PartesPoliciales.Remove(parte);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Parte policial eliminado exitosamente.";
            }
            else
            {
                TempData["ErrorMessage"] = "Parte policial no encontrado para eliminar.";
            }
            return RedirectToAction(nameof(Polices));
        }

        private async Task LoadDenunciasAndPersonasForViewBag()
        {
            var personas = await _context.Personas
                .Where(p => p.IdRol == 5)
                .Select(p => new
                {
                    p.IdPersona,
                    NombreCompleto = p.Nombres + " " + p.Apellidos + " - " + p.Cedula
                })
                .ToListAsync();
            ViewBag.Personas = new SelectList(personas, "IdPersona", "NombreCompleto");

            var denuncias = await _context.Denuncias
                .Include(d => d.IdDelitoNavigation) // Incluye el delito para poder mostrar su nombre
                .Select(d => new
                {
                    d.IdDenuncia,
                    Texto = d.Descripcion + (d.IdDelitoNavigation != null ? $" ({d.IdDelitoNavigation.Nombre})" : "")
                })
                //    Texto = d.IdDelitoNavigation != null ? $" {d.IdDelitoNavigation.Nombre}" : ""
                //})
                .ToListAsync();
            ViewBag.Denuncias = new SelectList(denuncias, "IdDenuncia", "Texto");
        }
    }
}
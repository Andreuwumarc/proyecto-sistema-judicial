//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;
//using SistemaGestionJudicial.Models;
//using System.Threading.Tasks;

//namespace SistemaGestionJudicial.Controllers
//{
//    public class PoliceController : Controller
//    {
//        private readonly SistemaContext _context;

//        public PoliceController(SistemaContext context)
//        {
//            _context = context;
//        }
//        public async Task<IActionResult> Polices()
//        {
//            var partes = await _context.PartesPoliciales
//                .Include(p => p.PersonaPolicia)
//                .Include(p => p.Denuncia)
//                .ToListAsync();

//            // Cargar ViewBag para el modal de Crear y Editar en la misma vista
//            var personas = await _context.Personas
//                .Where(p => p.Id_Rol == 5)
//                .Select(p => new
//                {
//                    p.Id_Persona,
//                    NombreCompleto = p.Nombres + " " + p.Apellidos + " - " + p.Cedula
//                })
//                .ToListAsync(); // Usar ToListAsync
//            ViewBag.Personas = new SelectList(personas, "Id_Persona", "NombreCompleto");

//            var denuncias = await _context.Denuncias // Usar await
//                .Select(d => new
//                {
//                    d.Id_Denuncia,
//                    Texto = d.Descripcion
//                })
//                .ToListAsync(); // Usar ToListAsync
//            ViewBag.Denuncias = new SelectList(denuncias, "Id_Denuncia", "Texto");

//            var viewModel = new PoliceViewModel
//            {
//                Partes = partes,
//                // Podrías inicializar otros campos si quieres
//            };

//            // ===> RUTA EXPLÍCITA A LA VISTA MOVIDA <===
//            return View("~/Views/Home/Polices.cshtml", viewModel);
//        }


//        // ========== Crear ==========

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create(PartePolicial parte)
//        {
//            if (ModelState.IsValid)
//            {
//                var ultimoId = await _context.PartesPoliciales
//                    .OrderByDescending(p => p.Id_Parte)
//                    .Select(p => p.Id_Parte)
//                    .FirstOrDefaultAsync();

//                parte.Id_Parte = ultimoId + 1;

//                _context.Add(parte);
//                await _context.SaveChangesAsync();
//                TempData["SuccessMessage"] = "Parte policial creado exitosamente."; // Agrega feedback
//                // ===> REDIRIGE A LA ACCIÓN POLICES EN EL MISMO CONTROLADOR <===
//                return RedirectToAction(nameof(Polices));
//            }

//            TempData["ErrorMessage"] = "Error al crear el parte policial. Verifique los datos."; // Agrega feedback
//            // Si la validación falla, redirigimos a la vista principal.
//            // Para mostrar errores en el modal sin recargar, se necesita AJAX.
//            return RedirectToAction(nameof(Polices));
//        }


//        // ========== EDIT ==========

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(PartePolicial parte)
//        {
//            if (parte == null || parte.Id_Parte == 0)
//            {
//                TempData["ErrorMessage"] = "Datos de parte policial inválidos.";
//                return RedirectToAction(nameof(Polices));
//            }

//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    _context.Update(parte);
//                    await _context.SaveChangesAsync();
//                    TempData["SuccessMessage"] = "Parte policial actualizado exitosamente.";
//                }
//                catch (DbUpdateConcurrencyException)
//                {
//                    if (!_context.PartesPoliciales.Any(e => e.Id_Parte == parte.Id_Parte))
//                    {
//                        TempData["ErrorMessage"] = "El parte policial que intenta editar no existe.";
//                    }
//                    else
//                    {
//                        throw;
//                    }
//                }
//                // ===> REDIRIGE A LA ACCIÓN POLICES EN EL MISMO CONTROLADOR <===
//                return RedirectToAction(nameof(Polices));
//            }

//            TempData["ErrorMessage"] = "Error al actualizar el parte policial. Verifique los datos.";
//            return RedirectToAction(nameof(Polices));
//        }

//        // ========== DELETE ==========
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Delete(long id)
//        {
//            var parte = await _context.PartesPoliciales.FindAsync(id);
//            if (parte != null)
//            {
//                _context.PartesPoliciales.Remove(parte);
//                await _context.SaveChangesAsync();
//                TempData["SuccessMessage"] = "Parte policial eliminado exitosamente.";
//            }
//            else
//            {
//                TempData["ErrorMessage"] = "Parte policial no encontrado para eliminar.";
//            }
//            // ===> REDIRIGE A LA ACCIÓN POLICES EN EL MISMO CONTROLADOR <===
//            return RedirectToAction(nameof(Polices));
//        }
//    }
//}

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
        private readonly SistemaContext _context;

        public PoliceController(SistemaContext context)
        {
            _context = context;
        }

        // Mostrar lista + datos para crear/editar en un solo ViewModel
        public async Task<IActionResult> Polices()
        {
            var partes = await _context.PartesPoliciales
                .Include(p => p.PersonaPolicia)
                .Include(p => p.Denuncia)
                .ToListAsync();

            var personas = await _context.Personas
                .Where(p => p.Id_Rol == 5)
                .Select(p => new
                {
                    p.Id_Persona,
                    NombreCompleto = p.Nombres + " " + p.Apellidos + " - " + p.Cedula
                })
                .ToListAsync();

            var denuncias = await _context.Denuncias
                .Select(d => new
                {
                    d.Id_Denuncia,
                    Texto = d.Descripcion
                })
                .ToListAsync();

            ViewBag.Personas = new SelectList(personas, "Id_Persona", "NombreCompleto");
            ViewBag.Denuncias = new SelectList(denuncias, "Id_Denuncia", "Texto");

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
                    .OrderByDescending(p => p.Id_Parte)
                    .Select(p => p.Id_Parte)
                    .FirstOrDefaultAsync();

                var parte = new PartePolicial
                {
                    Id_Parte = ultimoId + 1,
                    Id_Persona_Policia = viewModel.Id_Persona_Policia,
                    Id_Denuncia = viewModel.Id_Denuncia,
                    Descripcion = viewModel.Descripcion,
                    Fecha_Parte = viewModel.Fecha_Parte
                };

                _context.Add(parte);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Parte policial creado exitosamente.";
                return RedirectToAction(nameof(Polices));
            }

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

                    parte.Id_Persona_Policia = viewModel.Id_Persona_Policia;
                    parte.Id_Denuncia = viewModel.Id_Denuncia;
                    parte.Descripcion = viewModel.Descripcion;
                    parte.Fecha_Parte = viewModel.Fecha_Parte;

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
    }
}


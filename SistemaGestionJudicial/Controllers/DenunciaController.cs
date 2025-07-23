/*
Creado por MCP Alex Cordova
Fecha: 20-07-2025
*/

using Microsoft.AspNetCore.Mvc;
using SistemaGestionJudicial.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaGestionJudicial.Models;

namespace SistemaGestionJudicial.Controllers
{
    public class DenunciaController : Controller
    {
        private readonly ProyectoContext dbContext;

        public DenunciaController(ProyectoContext context)
        {
            dbContext = context;
        }

        [Route("/Home/Casos")]
        public async Task<IActionResult> Casos()
        {
            // Recuperamos todas las denuncias existentes.
            var denuncias = await dbContext.Denuncias
                .Include(p => p.IdPersonaDenunciaNavigation)
                .Include(d => d.IdDelitoNavigation)
                .ToListAsync();

            ViewBag.Personas = await dbContext.Personas
                .Select(p => new { p.IdPersona, p.Nombres, p.Apellidos })
                .ToListAsync();

            ViewBag.Delitos = await dbContext.Delitos
                .Select(d => new { d.IdDelito, d.Nombre })
                .ToListAsync();

            return View("~/Views/Home/Casos.cshtml", denuncias);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Denuncia denuncia)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Validación básica por si no se manda la fecha
            if (denuncia.FechaDenuncia == default)
                denuncia.FechaDenuncia = DateOnly.FromDateTime(DateTime.Today);

            // Obtener el máximo IdDenuncia actual
            var maxId = await dbContext.Denuncias.MaxAsync(d => (long?)d.IdDenuncia) ?? 0;

            // Asignar el nuevo ID manualmente
            denuncia.IdDenuncia = maxId + 1;

            dbContext.Denuncias.Add(denuncia);
            await dbContext.SaveChangesAsync();

            return Ok(new { success = true, id = denuncia.IdDenuncia });
        }

        /// <summary>
        /// Permite consultar una denuncia mediante su ID.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetById(long id)
        {
            var denuncia = await dbContext.Denuncias
                .Include(d => d.IdPersonaDenunciaNavigation)
                .Include(d => d.IdDelitoNavigation)
                .FirstOrDefaultAsync(d => d.IdDenuncia == id);

            if (denuncia == null)
                return NotFound();

            return Json(new
            {
                id = denuncia.IdDenuncia,
                lugarHecho = denuncia.LugarHecho,
                descripcion = denuncia.Descripcion,
                idPersona = denuncia.IdPersonaDenuncia,
                idDelito = denuncia.IdDelito,
                fechaDenuncia = denuncia.FechaDenuncia,

                // Campos para la vista Detalle. 
                denuncianteNombre = denuncia.IdPersonaDenunciaNavigation != null
            ? $"{denuncia.IdPersonaDenunciaNavigation.Nombres} {denuncia.IdPersonaDenunciaNavigation.Apellidos}"
            : null,

                delitoNombre = denuncia.IdDelitoNavigation?.Nombre
            });
        }


        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] Denuncia updatedDenuncia)
        {
            if (updatedDenuncia == null || updatedDenuncia.IdDenuncia <= 0)
            {
                return BadRequest("Datos inválidos para la denuncia.");
            }

            var existingDenuncia = await dbContext.Denuncias
                .FirstOrDefaultAsync(d => d.IdDenuncia == updatedDenuncia.IdDenuncia);

            if (existingDenuncia == null)
            {
                return NotFound("Denuncia no encontrada.");
            }

            // Actualizar campos
            existingDenuncia.Descripcion = updatedDenuncia.Descripcion;
            existingDenuncia.LugarHecho = updatedDenuncia.LugarHecho;
            existingDenuncia.IdPersonaDenuncia = updatedDenuncia.IdPersonaDenuncia;
            existingDenuncia.IdDelito = updatedDenuncia.IdDelito;
            existingDenuncia.FechaDenuncia = updatedDenuncia.FechaDenuncia;

            try
            {
                await dbContext.SaveChangesAsync();
                return Ok(new { message = "Denuncia actualizada correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("Denuncia/Delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var denuncia = await dbContext.Denuncias.FindAsync(id);
            if (denuncia == null)
                return NotFound("Denuncia no encontrada.");

            // Verificamos si existe un parte policial asociado
            bool tieneParte = await dbContext.PartesPoliciales.AnyAsync(p => p.IdDenuncia == id);
            if (tieneParte)
            {
                return BadRequest("No se puede eliminar la denuncia porque tiene un parte policial asociado.");
            }

            dbContext.Denuncias.Remove(denuncia);
            await dbContext.SaveChangesAsync();

            return Ok(new { success = true });
        }



        // GET: DenunciaController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: DenunciaController/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        // POST: DenunciaController/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        // GET: DenunciaController/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        // POST: DenunciaController/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}
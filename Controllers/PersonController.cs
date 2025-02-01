using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webApiPeople.Context;
using webApiPeople.Models;

namespace webApiPeople.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public PersonController(ApplicationContext context)
        {
            _context = context;
        }

        // ✅ OBTENER TODAS LAS PERSONAS
        // Este método devuelve una lista de todas las personas en la base de datos.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Person>>> GetPeople()
        {
            return await _context.Persons.ToListAsync();
        }

        // ✅ OBTENER UNA PERSONA POR SU ID
        // Este método busca una persona específica según su ID.
        // Si la encuentra, la devuelve; si no, devuelve un error 404.
        [HttpGet("{id}")]
        public async Task<ActionResult<Person>> GetPerson(int id)
        {
            var person = await _context.Persons.FindAsync(id);
            if (person == null)
                return NotFound(); // No se encontró la persona

            return person;
        }

        // ✅ CREAR UNA NUEVA PERSONA
        // Este método permite agregar una nueva persona a la base de datos.
        // Devuelve la persona creada con su ID asignado.
        [HttpPost]
        public async Task<ActionResult<Person>> CreatePerson(Person person)
        {
            _context.Persons.Add(person);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPerson), new { id = person.Id }, person);
        }

        // ✅ ACTUALIZAR UNA PERSONA EXISTENTE
        // Este método actualiza la información de una persona ya existente.
        // Si el ID no coincide o la persona no existe, devuelve un error.
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePerson(int id, Person person)
        {
            if (id != person.Id)
                return BadRequest("El ID de la URL no coincide con el ID del cuerpo.");

            _context.Entry(person).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonExists(id))
                    return NotFound(); // La persona no existe
                else
                    throw;
            }

            return NoContent(); // Actualización exitosa sin contenido que devolver
        }

        // ✅ ELIMINAR UNA PERSONA
        // Este método elimina una persona de la base de datos según su ID.
        // Si la persona no existe, devuelve un error 404.
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson(int id)
        {
            var person = await _context.Persons.FindAsync(id);
            if (person == null)
                return NotFound(); // La persona no existe

            _context.Persons.Remove(person);
            await _context.SaveChangesAsync();

            return NoContent(); // Eliminación exitosa
        }

        // ✅ VERIFICAR SI UNA PERSONA EXISTE
        // Este método verifica si una persona existe en la base de datos por su ID.
        private bool PersonExists(int id)
        {
            return _context.Persons.Any(e => e.Id == id);
        }
    }
}

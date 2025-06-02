using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P10___MédiLabo___Notes_API.Models;
using P10___MédiLabo___Notes_API.Services;

namespace P10___MédiLabo___Notes_API.Controllers;

[ApiController]
[Authorize]
[Route("api/notes")]
public class NotesController(NotesService notesService) : ControllerBase
{
    [HttpGet]
    public async Task<List<Notes>> Get() =>
        await notesService.GetAllAsync();
    
    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Notes>> Get(string id)
    {
        var note = await notesService.GetByIdAsync(id);
        if (note is null)
        {
            return NotFound();
        }
        return note;
    }
    
    [HttpPost]
    public async Task<IActionResult> Post(Notes note)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        await notesService.CreateAsync(note);
        return CreatedAtAction(nameof(Get), new { id = note.Id }, note);
    }
    
    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Put(string id, Notes note)
    {
        if (id != note.Id)
        {
            return BadRequest();
        }

        var existingNote = await notesService.GetByIdAsync(id);
        if (existingNote is null)
        {
            return NotFound();
        }

        await notesService.UpdateAsync(id, note);
        return NoContent();
    }
    
    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var existingNote = await notesService.GetByIdAsync(id);
        if (existingNote is null)
        {
            return NotFound();
        }

        await notesService.DeleteAsync(id);
        return NoContent();
    }
    
    [HttpGet("patient/{patientId}")]
    public async Task<ActionResult<List<Notes>>> GetByPatientId(int patientId)
    {
        var notes = await notesService.GetByPatientIdAsync(patientId);
        if (notes.Count == 0)
        {
            return NotFound(new { Message = "Aucune note trouvée pour ce patient." });
        }
        return notes;
    }
}
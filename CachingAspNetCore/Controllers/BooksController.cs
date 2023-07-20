using CachingAspNetCore.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace CachingAspNetCore.Controllers;

[ApiController]
[Route("api/books")]
public class BooksController: ControllerBase
{
    private readonly ILogger<BooksController> _logger;
    private readonly IBookRepository _bookRepos;
    public BooksController(IBookRepository bookRepos,ILogger<BooksController> logger)
    {
        _bookRepos = bookRepos;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetBooks()
    {
        try
        {
            var books = await _bookRepos.GetBooks();
            _logger.LogInformation("Retrieved books");
            return Ok(books);    
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError,"Something went wrong");
        }
        
    }
    

}
using CachingAspNetCore.Model;
using CachingAspNetCore.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace CachingAspNetCore.Controllers;

[ApiController]
[Route("api/books")]
public class BooksController : ControllerBase
{
    private readonly ILogger<BooksController> _logger;
    private readonly IBookRepository _bookRepos;
    public BooksController(IBookRepository bookRepos, ILogger<BooksController> logger)
    {
        _bookRepos = bookRepos;
        _logger = logger;
    }

    [HttpGet]
    [OutputCache(PolicyName = "evict", VaryByQueryKeys = new[] { "lang" })]
    public async Task<IActionResult> GetBooks(string? lang, string? title)
    {
        try
        {
            var books = await _bookRepos.GetBooks(lang, title);
            _logger.LogInformation("Retrieved books");
            // 👉 adding etag to response header
            // Response.Headers.ETag = $"\"{Guid.NewGuid():n}\"";
            return Ok(books);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong");
        }

    }

    [OutputCache(PolicyName = "evict")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var book = await _bookRepos.GetBook(id);
            if (book == null)
                return NotFound();
            _logger.LogInformation($"Retrieved book with id {id}");
            return Ok(book);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong");
        }
    }


    [HttpPost]
    public async Task<IActionResult> AddBook(Book book, IOutputCacheStore cache)
    {
        try
        {
            var createdBook = await _bookRepos.AddBook(book);
            _logger.LogInformation("Books have saved");
            // evicting the cache related to 'tag-book'
            await cache.EvictByTagAsync("tag-book", default);
            return CreatedAtAction(nameof(AddBook), createdBook);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    // for testing purpose only
    // [HttpPost("/purge")]
    // public async Task<IActionResult> PurgeBook(IOutputCacheStore cache)
    // {
    //     await cache.EvictByTagAsync("tag-book", default);
    //     return Ok();
    // }




}
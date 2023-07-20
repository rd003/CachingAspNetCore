using System.Collections;
using System.Text.Json;
using CachingAspNetCore.Model;

namespace CachingAspNetCore.Repositories;

public interface IBookRepository
{
    Task<IEnumerable<Book>> GetBooks();
}
public class BookRepository: IBookRepository
{
    public async Task<IEnumerable<Book>> GetBooks()
    {
        string jsonFilePath = "books.json";
        string jsonString = await File.ReadAllTextAsync(jsonFilePath);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling=JsonCommentHandling.Skip
        };
        List<Book> books = JsonSerializer.Deserialize<List<Book>>(jsonString, options);
        return books??Enumerable.Empty<Book>();
    }

}
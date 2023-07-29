using CachingAspNetCore.Controllers;
using CachingAspNetCore.Model;
using Dapper;
using Microsoft.Data.SqlClient;

namespace CachingAspNetCore.Repositories;

public interface IBookRepository
{
    Task<IEnumerable<Book>> GetBooks(string? lang, string? title);
    Task<Book> AddBook(Book book);
    Task<Book> GetBook(int id);
}
public class BookRepository : IBookRepository
{
    private IConfiguration _config;
    public BookRepository(IConfiguration config)
    {
        _config = config;
    }

    public async Task<IEnumerable<Book>> GetBooks(string? lang, string? title)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("default"));
        string sql = "select * from Book where 1=1";
        if (!string.IsNullOrWhiteSpace(lang))
        {
            sql += " and language like @Lang";
        }
        if (!string.IsNullOrWhiteSpace(title))
        {
            sql += " and title like @Title";
        }
        var books = await connection.QueryAsync<Book>(sql, new { lang = $"%{lang}%", Title = $"%{title}%" });
        return books;
    }

    public async Task<Book> AddBook(Book book)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("default"));
        string sql = @"insert into book(Author,Country,ImageLink,Language,Link,Pages,Title,Year,Price)
        values (@Author,@Country,@ImageLink,@Language,@Link,@Pages,@Title,@Year,@Price); SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY]";
        int id = await connection.ExecuteScalarAsync<int>(sql, book, commandType: System.Data.CommandType.Text);
        book.Id = id;
        return book;
    }


    public async Task<Book> GetBook(int id)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("default"));
        string sql = "select * from Book where id=@id";
        var book = await connection.QueryAsync<Book>(sql, new { id }, commandType: System.Data.CommandType.Text);
        return book.SingleOrDefault();
    }





}
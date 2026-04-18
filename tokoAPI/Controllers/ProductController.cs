using Microsoft.AspNetCore.Mvc;
using Npgsql;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly string _connection;

    public ProductController(IConfiguration config)
    {
        _connection = config.GetConnectionString("DefaultConnection");
    }

    // GET ALL
    [HttpGet]
    public IActionResult GetAll()
    {
        var list = new List<object>();

        using (var conn = new NpgsqlConnection(_connection))
        {
            conn.Open();
            var cmd = new NpgsqlCommand("SELECT * FROM products", conn);
            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new
                {
                    id = reader["id"],
                    name = reader["name"],
                    price = reader["price"],
                    category_id = reader["category_id"],
                    user_id = reader["user_id"]
                });
            }
        }

        return Ok(new { status = "success", data = list });
    }

    // GET BY ID
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        using var conn = new NpgsqlConnection(_connection);
        conn.Open();

        var cmd = new NpgsqlCommand("SELECT * FROM products WHERE id=@id", conn);
        cmd.Parameters.AddWithValue("id", id);

        var reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            return Ok(new
            {
                status = "success",
                data = new
                {
                    id = reader["id"],
                    name = reader["name"],
                    price = reader["price"],
                    category_id = reader["category_id"],
                    user_id = reader["user_id"]
                }
            });
        }

        return NotFound(new { status = "error", message = "Data tidak ditemukan" });
    }

    // POST
    [HttpPost]
    public IActionResult Create(Product p)
    {
        using var conn = new NpgsqlConnection(_connection);
        conn.Open();

        var cmd = new NpgsqlCommand(
            "INSERT INTO products(name,price,category_id,user_id) VALUES(@n,@p,@c,@u)",
            conn);

        cmd.Parameters.AddWithValue("n", p.Name);
        cmd.Parameters.AddWithValue("p", p.Price);
        cmd.Parameters.AddWithValue("c", p.CategoryId);
        cmd.Parameters.AddWithValue("u", p.UserId);

        cmd.ExecuteNonQuery();

        return Ok(new { status = "success", message = "Data ditambahkan" });
    }

    // PUT
    [HttpPut("{id}")]
    public IActionResult Update(int id, Product p)
    {
        using var conn = new NpgsqlConnection(_connection);
        conn.Open();

        var cmd = new NpgsqlCommand(
            "UPDATE products SET name=@n,price=@p,category_id=@c,user_id=@u WHERE id=@id",
            conn);

        cmd.Parameters.AddWithValue("id", id);
        cmd.Parameters.AddWithValue("n", p.Name);
        cmd.Parameters.AddWithValue("p", p.Price);
        cmd.Parameters.AddWithValue("c", p.CategoryId);
        cmd.Parameters.AddWithValue("u", p.UserId);

        int result = cmd.ExecuteNonQuery();

        if (result == 0)
            return NotFound(new { status = "error", message = "Data tidak ditemukan" });

        return Ok(new { status = "success", message = "Data diupdate" });
    }

    // DELETE
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        using var conn = new NpgsqlConnection(_connection);
        conn.Open();

        var cmd = new NpgsqlCommand("DELETE FROM products WHERE id=@id", conn);
        cmd.Parameters.AddWithValue("id", id);

        int result = cmd.ExecuteNonQuery();

        if (result == 0)
            return NotFound(new { status = "error", message = "Data tidak ditemukan" });

        return Ok(new { status = "success", message = "Data dihapus" });
    }
}
using LoginExam.Models;
using MySql.Data.MySqlClient;

namespace LoginExam.Database;

public class UserService
{
    private const string connectString = "server=localhost;user=root;password={your password}!;database=Login";
    
    public bool ConnectionTest()
    {
        try
        {
            using var conn = new MySqlConnection(connectString);

            conn.Open();

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public List<User> GetUser()
    {
        var list = new List<User>();
        var sql = "SELECT * FROM Login";

        using var conn = new MySqlConnection(connectString);
        
        conn.Open();

        var cmd = new MySqlCommand(sql, conn);

        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                list.Add(new User()
                {
                    id = Convert.ToInt32(reader["id"]),
                    user_id = reader["user_id"].ToString(),
                    user_password = reader["user_password"].ToString(),
                    user_name = reader["user_name"].ToString()
                });
            }
        }

        conn.Close();

        return list;
    }

    public void InsertUser(User user)
    {
        var sql = $"INSERT INTO Login(user_id, user_password, user_name) VALUES ('{user.user_id}', '{user.user_password}', '{user.user_name}')";

        using var conn = new MySqlConnection(connectString);

        try
        {
            conn.Open();

            var cmd = new MySqlCommand(sql, conn);

            if (cmd.ExecuteNonQuery() == 1)
            {
                Console.WriteLine("Insert 성공");
            }
            else
            {
                Console.WriteLine("Insert 실");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("데이터베이스 연결 오류");
            Console.WriteLine(e);
        }
        
        conn.Close();
    }
}
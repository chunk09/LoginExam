using LoginExam.Database;
using LoginExam.Models;
using Microsoft.AspNetCore.Mvc;

namespace LoginExam.Controllers;

public class UserController : Controller
{
    [HttpGet("/")]
    public IActionResult Index()
    {
        var db = new UserService();

        Console.WriteLine($"데이터베이스 : {db.ConnectionTest()}");

        return View();
    }

    [HttpGet("/register")]
    public IActionResult RegisterForm() => View("Register");

    [HttpGet("/login")]
    public IActionResult LoginForm() => View("Login");

    [HttpGet("/info")]
    public IActionResult InfoForm()
    {
        var db = new UserService();
        var sessionId = HttpContext.Session.GetInt32("Id");

        if (sessionId != null)
        {
            foreach (var user in db.GetUser())
            {
                if (user.id == sessionId)
                {
                    ViewData["name"] = user.user_name;
                    
                    break;
                }
            }
        }
        else
        {
            ViewData["name"] = "로그인을 해주세요";
        }
        
        return View("Info");
    }

    [HttpGet("/logout")]
    public RedirectResult Logout()
    {
        if (HttpContext.Session.GetInt32("Id") != null)
        {
            HttpContext.Session.Remove("Id");
        }
        else
        {
            Console.WriteLine("로그아웃 실패");
        }

        return Redirect("/info");
    }

    [HttpPost("/register-post")]
    public RedirectResult RegisterPost()
    {
        var db = new UserService();

        var user = new User
        {
            user_id = Request.Form["id"],
            user_password = Request.Form["password"],
            user_name = Request.Form["name"]
        };

        if (CheckRegister(user.user_id, user.user_password, Request.Form["password-check"], user.user_name))
        {
            db.InsertUser(user);
            
            Console.WriteLine($"{user.user_name}님이 회원가입에 성공하셨습니다.");
        }
        else
        {
            Console.WriteLine($"{user.user_id}(id)님이 회원가입 실패");

            return Redirect("/register");
        }
        
        return Redirect("/");
    }

    [HttpPost("/login-post")]
    public RedirectResult LoginPost()
    {
        var db = new UserService();
        var users = db.GetUser();

        var userId = Request.Form["id"];
        var userPassword = Request.Form["password"];

        foreach (var user in users.Where(user => userId == user.user_id && userPassword == user.user_password))
        {
            Console.WriteLine("----------------------------");
            Console.WriteLine("로그인 성공");
            
            Console.WriteLine($"이름 : {user.user_name}");
            Console.WriteLine($"아이디 : {user.user_id}");
            Console.WriteLine($"비밀번호 : {user.user_password}");
            Console.WriteLine("----------------------------");
                
            HttpContext.Session.SetInt32("Id", user.id);

            return Redirect("/info");
        }
        
        Console.WriteLine("로그인 오류");
        
        return Redirect("/login");
    }

    private bool CheckRegister(string id, string pw, string pwCheck, string name)
    {
        if (id.Length < 5) return false;
        if (pw.Length < 5) return false;
        if (name.Length == 0) return false;

        if (pw != pwCheck) return false;

        return true;
    }
}
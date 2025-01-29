using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Hello_Docker_Web.Models;

namespace Hello_Docker_Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private ApplicationDbContext _db;

    public HomeController( ApplicationDbContext db,ILogger<HomeController> logger)
    {
        _logger = logger;
        _db = db;

    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    //Register Route
    public IActionResult RegisterUser()
    {

        string? userName = Request.Form["UserName"];
        string? userAge = Request.Form["UserAge"];
        string? userPassword = Request.Form["Password"];
        string? userEmailAddress = Request.Form["EmailAddress"];


        if (!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(userAge) && !string.IsNullOrWhiteSpace(userPassword) && !string.IsNullOrWhiteSpace(userEmailAddress))
        {
            int userAgeInt;

            if (int.TryParse(userAge , out userAgeInt))
            {
                userAgeInt = int.Parse(userAge);
            }



            UsersModel user = new UsersModel
            {
                Name = userName,
                Age = userAgeInt,
                Password = userPassword,
                EmailAddress = userEmailAddress,


            };


            var existingUser = _db.users.FirstOrDefault(u => u.EmailAddress == userEmailAddress && u.Name == userName);

            if (existingUser != null)
            {

                ViewData["Opps"] = "Email address in use!";
                return View("Index");
            }

            _db.Add(user);
            _db.SaveChanges();

            return View("Welcome");
        }

        
        return View("Index");
    }

    public IActionResult LoginPage()
    {

        return View("LoginPage");
    }

    [HttpPost]
    public IActionResult LoginUser()
    {
        string? userName = Request.Form["UserName"];
        string? userAge = Request.Form["UserAge"];
        string? emailAddress = Request.Form["EmailAddress"];
        string? password = Request.Form["Password"];


        if (!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(emailAddress) && !string.IsNullOrWhiteSpace(userAge) && !string.IsNullOrWhiteSpace(password) )
        {
            int userAgeInt;

            if (int.TryParse(userAge , out userAgeInt))
            {
                userAgeInt = int.Parse(userAge);
            }


            var validUser = _db.users.SingleOrDefault(u => u.EmailAddress == emailAddress && u.Age == userAgeInt && u.Password == password ); 

            if(validUser != null)
            {

                string? nickname = validUser.Name;
                return View("Dashboard", nickname);
            }

            ViewData["Opps"] = "User not Found!";
            return View("LoginPage");
        }

        return View("LoginPage");
    }

    public IActionResult Privacy()
    {


        ViewData["Time"] = DateTime.Now;
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

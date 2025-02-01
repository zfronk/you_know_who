using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Hello_Docker_Web.Models;
using System.Reflection.Metadata;

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
    // Home Page
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    // Register Route
    public IActionResult RegisterUser()
    {

        // Form details
        string userName = Request.Form["UserName"];
        string userAge = Request.Form["UserAge"];
        string userPassword = Request.Form["Password"];
        string userEmailAddress = Request.Form["EmailAddress"];

        // No null entires
        if (!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(userAge) && !string.IsNullOrWhiteSpace(userPassword) && !string.IsNullOrWhiteSpace(userEmailAddress))
        {
            int userAgeInt;
            
            // Parse
            if (int.TryParse(userAge , out userAgeInt))
            {
                userAgeInt = int.Parse(userAge);
            }


            // Add new user
            UsersModel newUser = new UsersModel
            {
                Name = userName,
                Age = userAgeInt,
                Password = userPassword,
                EmailAddress = userEmailAddress,


            };

            // Check via email address
            var existingUser = _db.users.FirstOrDefault(u => u.EmailAddress == userEmailAddress);
            
            
            // If user doesn't exist
            if (existingUser == null)
            {

                _db.Add(newUser);
                _db.SaveChanges();


                var currentUser = newUser;

                // start session 
                HttpContext.Session.SetString("userId", currentUser.Id.ToString());
                HttpContext.Session.SetString("userName",currentUser.Name);

                return View("Welcome");

              
            }

            ViewData["Opps"] = "Email address already in use!";
            return View("Index");
        }

        
        return View("Index");
    }

    // Page
    public IActionResult LoginPage()
    {

        return View("LoginPage");
    }

    [HttpPost]
    // Login Route
    public IActionResult LoginUser()
    {

        // Form details
        string userName = Request.Form["UserName"];
        string userAge = Request.Form["UserAge"];
        string emailAddress = Request.Form["EmailAddress"];
        string password = Request.Form["Password"];

        // No null entries
        if (!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(emailAddress) && !string.IsNullOrWhiteSpace(userAge) && !string.IsNullOrWhiteSpace(password) )
        {
            int userAgeInt;

            if (int.TryParse(userAge , out userAgeInt))
            {
                userAgeInt = int.Parse(userAge);
            }


            var regUser = _db.users.SingleOrDefault(u => u.EmailAddress == emailAddress && u.Age == userAgeInt && u.Password == password && u.Name == userName ); 

            if( regUser != null)
            {
                // Create session
                HttpContext.Session.SetString("userId", regUser.Id.ToString());
                HttpContext.Session.SetString("userName", regUser.Name);
                


                ViewData["Nickname"] = regUser.Name;
                return View("Dashboard");
            }

            ViewData["Opps"] = "User not Found!";
            return View("LoginPage");
        }

        return View("LoginPage");
    }

    // Dashboard Page
    public IActionResult Dashboard()
    {
        // Retrieve session
        string userInSessionId = HttpContext.Session.GetString("userId");
        string userInSession = HttpContext.Session.GetString("userName");

        if (!string.IsNullOrEmpty(userInSessionId))
        {
            ViewData["Nickname"] = userInSession;
            return View("Dashboard");
        }

        ViewData["Opps"] = "Please login to view dashboard!";
        return View("LoginPage");
        
    }


    // Log Out 
    public IActionResult LogOut()
    {
        // Retrieve session
        var userInSession = HttpContext.Session.GetString("userId");

        // if not null boot out
        if (!string.IsNullOrEmpty(userInSession))
        {
            HttpContext.Session.Clear();
            return View("Bye");
        }

        ViewData["Opps"] = "Please login to be able to log out!";
        return View("LoginPage");
        
    }

    //  Create Post Page
    public IActionResult CreatePost()
    {
        var userInSession = HttpContext.Session.GetString("userId");

        if (!string.IsNullOrEmpty(userInSession))
        {
            return View("CreatePost");
        }
        ViewData["Opps"]= "Please login to create post!";
        return View("LoginPage");
    }


    // Post Message Route

    [HttpPost]
    public IActionResult PostMessage()
    {
        // Get session
        var userInSession = HttpContext.Session.GetString("userId");

        // If in session
        if (!string.IsNullOrEmpty(userInSession))
        {

            //Parse

            if (Guid.TryParse(userInSession , out Guid userID))
            {
                // Add post to db

                string postTitle = Request.Form["PostTitle"];
                string postBody = Request.Form["PostBody"];

                // If not null
                if (!string.IsNullOrWhiteSpace(postTitle) && !string.IsNullOrWhiteSpace(postBody))
                {
                    // Check if post is present
                    var existingPost = _db.posts.SingleOrDefault(u => u.PostTitle == postTitle && u.PostBody == postBody);

                    if (existingPost == null)
                    {
                        CreatedPostModel newPost = new CreatedPostModel
                        {
                            PostTitle = postTitle,
                            PostBody = postBody,
                            UserId = userID
                            
                        };

                        _db.posts.Add(newPost);
                        _db.SaveChanges();

                        return View("PostShared");

                    }

                    else
                    {
                        ViewData["Opps"] = "No spamming!";
                        return View("CreatePost");
                    }

                    
                }



            }



        }

        ViewData["Opps"] = "Please login to share post!";
        return View("LoginPage");

    }

    // Your posts page
    public IActionResult YourPosts()
    {
        // Retrieve session
        var userInSession = HttpContext.Session.GetString("userId");

        // If present
        if (!string.IsNullOrEmpty(userInSession))
        {

            // Parse

            if (Guid.TryParse(userInSession , out Guid userID))
            {

                var userposts = _db.posts.Where(u => u.UserId == userID).OrderByDescending(u => u.CreatedAt).ToList();

                
                // Pass userposts to view
                return View("YourPosts",userposts);
            }

        }

        ViewData["Opps"] = "Please login to view posts!";
        return View("LoginPage");


    }

    // Privacy Page
    public IActionResult Privacy()
    {

        ViewData["EffectiveDate"] = DateTime.Now.ToString("dddd, MMMM dd, yyyy");
        ViewData["LatestUpdate"] = "Thursday, January 30, 2025";
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

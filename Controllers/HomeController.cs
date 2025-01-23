using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MVC_Studio.Interfaces;
using MVC_Studio.Models;
using MVC_Studio.Service;

namespace MVC_Studio.Controllers;

    public class HomeController(ISqlService intsql, IJwtoken jwtoken) : Controller
    {
        private readonly ISqlService _intsql = intsql;
    private readonly IJwtoken _jwtoken = jwtoken;

    public string errorMessage = "";
    public string successMessage = "";

    public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

       



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



        //Register Model

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                // Check if user already exists by email
                bool userExists = await _intsql.UserExistsAsync(user.Email);

                if (userExists)
                {
                // If user exists, return a message
                ViewBag.errorMessage = "User Already exists";
                }
                else
                {
                    // Register the user
                    bool isUserRegistered = await _intsql.RegisterUserAsync(user.Name, user.Email, user.Password);

                    if (isUserRegistered)
                    {
                    // If registration successful, show a success message
                  ViewBag.successMessage = "User created suceessfully";
                    }
                    else
                    {
                   ViewBag.errorMessage = "Registration failed, Please try again";
                    }
                }
            }

            return View(user); // Return the view with validation messages
        }




    //Login Model




public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(Login login)
    {
        // Check if the input model is valid
        if (!ModelState.IsValid)
        {
            return View(login); // Return the view with validation errors
        }

        try
        {
            // Attempt to log in
            bool isLogged = await _intsql.LoginAsync(login.Email, login.Password);
            int? userid = await _intsql.GetuserIdByemail(login.Email);

            if (isLogged)
            {
                 
                string token = _jwtoken.Createtoken(userid.Value, login.Email);

                HttpContext.Response.Cookies.Append("AuthToken", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = DateTime.UtcNow.AddHours(3)
                });

                ViewBag.successMessage = "Logged in Successfully";
              
                
            }
            else
            {
                // If login fails, return the same view with an error message
                ViewBag.errorMessage = "Invalid Email or Password";
                return View(login);
               
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            // Log the exception (optional: use a logging framework like Serilog)
            ViewBag.errorMessage = "An unexpected error occurred. Please try again later.";
           
        }
        return View(login);
    }

    
}


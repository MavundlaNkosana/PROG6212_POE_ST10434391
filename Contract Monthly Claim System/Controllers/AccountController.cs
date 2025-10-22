using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(string email, string password)
    {
        // TODO: Add authentication logic
        if (email == "demo@uni.edu" && password == "password123")
            return RedirectToAction("Index", "Claims");

        ViewBag.Error = "Invalid credentials";
        return View();
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Register(string fullName, string email, string password)
    {
        // TODO: Add registration logic
        return RedirectToAction("Login");
    }

    public IActionResult Logout()
    {
        // TODO: Clear session/cookies
        return RedirectToAction("Index", "Home");
    }
}

using Application.DataTransferObject;
using Application.DataTransferObject.IdentityDTO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Plataforma_Front.Interfaces;
using Plataforma_Front.ViewModels;
using System.Security.Claims;

namespace Plataforma_Front.Controllers
{
    public class AuthenticaController : Controller
    {
        private readonly IAuthenticacao _auth;

        public AuthenticaController(IAuthenticacao auth)
        {
            _auth = auth;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Token(UsuarioViewModel model)
        {
           if(!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Login Inválido...");
                return View("Error");
            }

            var result = await _auth.AutenticaUsuario(model);

            if(result is null)
            {
                ModelState.AddModelError(string.Empty, "Login Inválido...");
                return View("Error");
            }

            // armazenar o TOKEN no cookie
            Response.Cookies.Append("X-Access-Token", result.Token, new CookieOptions()
            {
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Expires = result.Expiration 
            });

            Response.Cookies.Append("X-Refresh-Token", result.RefreshToken, new CookieOptions()
            {
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Expires = result.Expiration
            });


            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.UserName),
            };

            var identity = new ClaimsIdentity(
                claims,
                IdentityConstants.ApplicationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("Cookies", principal);

            return Redirect("/");
            
        }

        [HttpGet]
        public IActionResult Cadastro()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Cadastro(RegisterModelDTO model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Registro Inválido...");
                return View("Error");
            }
            try
            {
                await _auth.RegisterUsuario(model);

                return Redirect("/");
            }
            catch(Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                return View("Error");
            }                      
        }


        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete("X-Access-Token");
            Response.Cookies.Delete("X-Refresh-Token");

            await HttpContext.SignOutAsync("Cookies");

            return Redirect("/");
        }


    }
}

using Application.DataTransferObject;
using Application.DataTransferObject.IdentityDTO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Plataforma_Front.Interfaces;
using Plataforma_Front.ViewModels;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;


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
                return View("Login", model);
            }

            var result = await _auth.AutenticaUsuario(model);

            if(result is null)
            {
                ModelState.AddModelError(string.Empty, "Usuário ou senha inválidos.");
                return View("Login",model);
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


            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(result.Token);

            var claims = jwt.Claims.ToList();

            // mapeia Name
            var uniqueName = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName);
            
            if(uniqueName != null)
            {
                claims.Add(new Claim(ClaimTypes.Name, uniqueName.Value));
            }

            // Mapeia as Roles
            foreach (var role in claims.Where(c => c.Type == "role").ToList())
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Value));
            }


            var identity = new ClaimsIdentity( claims, IdentityConstants.ApplicationScheme); 
            
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
        public async Task<IActionResult> CadastroUser(RegisterModelDTO model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Verifique os dados informados."
                );

                return View("Cadastro", model);
            }

            try
            {
                await _auth.RegisterUsuario(model);

                return Redirect("/");
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Não foi possível realizar o cadastro. Tente novamente."
                );

                return View("Cadastro", model);
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

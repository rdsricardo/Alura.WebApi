using Alura.ListaLeitura.Seguranca;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Alura.ListaLeitura.Services
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly SignInManager<Usuario> signInManager;

        public LoginController(SignInManager<Usuario> signInManager)
        {
            this.signInManager = signInManager;
        }

        [HttpPost]
        public async Task<IActionResult> Token(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Login, model.Password, false, false);
                if (result.Succeeded)
                {
                    //criar token (header + payload >> claims (direitos) + signature)

                    var claims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, model.Login),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    };

                    var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(@"$_Qr@2uGWjjU=2^qy5DpaYYd2WPL%UG$pK*-Eb%^gdh%wtZR3F6+ydNVRVE+hsZShe-YaJA68YRLjMTx==Utry-fXUEV$tfvbEY4SRNw%xqm7VVF%h#cc!ysv5^7yLESqF*hsv=sy^%4C3x@a7Kc_BHr5?5FyvcWTwTYnzg^rd?^u!XUu+%N7v=akMZLMaX769U6bHZQQu^2D_N!nHM-G82DFB&zpqKeFpq9EHTngPwpkJzJj5-$rM=p&4#+rATx"));
                    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(
                        issuer: "Alura.WebApp",
                        audience: "Postman",
                        claims: claims,
                        signingCredentials: credentials,
                        expires: DateTime.Now.AddMinutes(30)
                    );

                    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                    return Ok(tokenString);
                }

                return Unauthorized();
            }
            return BadRequest();
        }
    }
}
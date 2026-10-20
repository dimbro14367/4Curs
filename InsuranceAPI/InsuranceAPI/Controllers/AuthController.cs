using System.Linq;
using System.Net;
using System.Web.Http;
using InsuranceAPI.Models;
using InsuranceAPI.Services;

namespace InsuranceAPI.Controllers
{
    [RoutePrefix("api/auth")]
    public class AuthController : ApiController
    {
        private readonly JwtService _jwt = new JwtService();

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public IHttpActionResult Login([FromBody] LoginDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Login) || string.IsNullOrWhiteSpace(dto.Password))
                return Content(HttpStatusCode.BadRequest,
                    new { error = "Введите логин и пароль", code = "EMPTY_CREDENTIALS" });

            using (var db = new user30_dbEntities1())
            {
                var user = db.ins_Users
                    .Include("ins_Roles")
                    .FirstOrDefault(u => u.Login == dto.Login && u.PasswordHash == dto.Password);

                if (user == null)
                    return Unauthorized();

                string role = user.ins_Roles != null ? user.ins_Roles.RoleName : "Client";
                string token = _jwt.GenerateToken(user.UserID, user.Login, role);

                return Ok(new LoginResponseDto
                {
                    UserId = user.UserID,
                    Login = user.Login,
                    Role = role,
                    Token = token
                });
            }
        }
    }
}
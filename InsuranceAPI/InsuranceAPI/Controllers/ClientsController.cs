using System.Linq;
using System.Web.Http;

namespace InsuranceAPI.Controllers
{
    [RoutePrefix("api/clients")]
    [Authorize]
    public class ClientsController : ApiController
    {
        [HttpGet]
        [Route("{id:int}")]
        public IHttpActionResult GetById(int id)
        {
            using (var db = new user30_dbEntities1())
            {
                var client = db.ins_Clients.Find(id);
                if (client == null) return NotFound();

                return Ok(new
                {
                    client.ClientID,
                    client.LastName,
                    client.FirstName,
                    client.MiddleName,
                    client.BirthDate,
                    client.PassportSeries,
                    client.PassportNumber,
                    client.Address,
                    client.Phone
                });
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        public IHttpActionResult Update(int id, [FromBody] ins_Clients dto)
        {
            if (dto == null) return BadRequest("Пустое тело запроса");

            using (var db = new user30_dbEntities1())
            {
                var client = db.ins_Clients.Find(id);
                if (client == null) return NotFound();

                client.LastName = dto.LastName;
                client.FirstName = dto.FirstName;
                client.MiddleName = dto.MiddleName;
                client.Address = dto.Address;
                client.Phone = dto.Phone;
                db.SaveChanges();
                return Ok();
            }
        }
    }
}
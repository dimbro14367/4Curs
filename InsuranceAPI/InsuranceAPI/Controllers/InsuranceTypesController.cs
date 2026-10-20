using System.Linq;
using System.Web.Http;

namespace InsuranceAPI.Controllers
{
    [RoutePrefix("api/insurance-types")]
    public class InsuranceTypesController : ApiController
    {
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public IHttpActionResult GetAll()
        {
            using (var db = new user30_dbEntities1())
            {
                var types = db.ins_InsuranceTypes
                    .OrderBy(t => t.TypeName)
                    .Select(t => new { t.TypeID, t.TypeName })
                    .ToList();
                return Ok(types);
            }
        }
    }
}
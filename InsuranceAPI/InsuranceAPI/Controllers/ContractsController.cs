using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using InsuranceAPI.Models;

namespace InsuranceAPI.Controllers
{
    [RoutePrefix("api/contracts")]
    [Authorize]
    public class ContractsController : ApiController
    {
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetByClient([FromUri] int? clientId = null, [FromUri] string status = null)
        {
            using (var db = new user30_dbEntities1())
            {
                var query = db.ins_InsuranceContracts.AsQueryable();

                if (clientId.HasValue)
                    query = query.Where(c => c.ClientID == clientId.Value);

                if (!string.IsNullOrWhiteSpace(status) && status == "pending")
                    query = query.Where(c => !c.IsAccept && c.RejectReason == null);
                else if (!string.IsNullOrWhiteSpace(status) && status == "approved")
                    query = query.Where(c => c.IsAccept);
                else if (!string.IsNullOrWhiteSpace(status) && status == "rejected")
                    query = query.Where(c => !c.IsAccept && c.RejectReason != null);

                var result = query.Select(c => new
                {
                    c.ContractID,
                    c.ContractNumber,
                    c.InsuredSum,
                    c.Premium,
                    c.StartDate,
                    c.EndDate,
                    Status = c.IsAccept ? "approved" : (c.RejectReason != null ? "rejected" : "pending")
                }).ToList();

                return Ok(result);
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        public IHttpActionResult GetById(int id)
        {
            using (var db = new user30_dbEntities1())
            {
                var c = db.ins_InsuranceContracts.Find(id);
                if (c == null) return NotFound();

                return Ok(MapToDto(c, db));
            }
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Create([FromBody] CreateContractDto dto)
        {
            if (dto == null)
                return Content(HttpStatusCode.BadRequest,
                    new { error = "Пустое тело запроса", code = "EMPTY_BODY" });

            if (dto.EndDate <= dto.StartDate)
                return Content(HttpStatusCode.BadRequest,
                    new { error = "Дата окончания должна быть позже даты начала", code = "INVALID_DATE_RANGE" });

            if (dto.InsuredSum <= 0 || dto.Premium <= 0)
                return Content(HttpStatusCode.BadRequest,
                    new { error = "Суммы должны быть положительными", code = "INVALID_SUM" });

            using (var db = new user30_dbEntities1())
            {
                var client = db.ins_Clients.Find(dto.ClientId);
                if (client == null)
                    return Content(HttpStatusCode.BadRequest,
                        new { error = "Клиент не найден", code = "CLIENT_NOT_FOUND" });

                var type = db.ins_InsuranceTypes.Find(dto.TypeId);
                if (type == null)
                    return Content(HttpStatusCode.BadRequest,
                        new { error = "Вид страхования не найден", code = "TYPE_NOT_FOUND" });

                string number = $"ДГ-{DateTime.Now:yyyy}-{new Random().Next(100, 999)}";
                while (db.ins_InsuranceContracts.Any(c => c.ContractNumber == number))
                    number = $"ДГ-{DateTime.Now:yyyy}-{new Random().Next(100, 999)}";

                int agentId = 1;
                if (!db.ins_Users.Any(u => u.UserID == agentId))
                {
                    var firstAgent = db.ins_Users.FirstOrDefault();
                    if (firstAgent == null)
                        return Content(HttpStatusCode.BadRequest,
                            new { error = "Нет агентов в системе", code = "NO_AGENT" });
                    agentId = firstAgent.UserID;
                }

                var contract = new ins_InsuranceContracts
                {
                    ContractNumber = number,
                    ClientID = dto.ClientId,
                    AgentID = agentId,
                    TypeID = dto.TypeId,
                    ConclusionDate = DateTime.Now,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    InsuredSum = dto.InsuredSum,
                    Premium = dto.Premium,
                    IsAccept = false
                };

                db.ins_InsuranceContracts.Add(contract);
                db.SaveChanges();

                return Created($"api/contracts/{contract.ContractID}",
                    MapToDto(contract, db));
            }
        }

        private ContractResponseDto MapToDto(ins_InsuranceContracts c, user30_dbEntities1 db)
        {
            return new ContractResponseDto
            {
                ContractId = c.ContractID,
                ContractNumber = c.ContractNumber,
                ClientName = db.ins_Clients.Where(x => x.ClientID == c.ClientID)
                    .Select(x => x.LastName + " " + x.FirstName).FirstOrDefault(),
                TypeName = db.ins_InsuranceTypes.Where(x => x.TypeID == c.TypeID)
                    .Select(x => x.TypeName).FirstOrDefault(),
                InsuredSum = c.InsuredSum,
                Premium = c.Premium,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                ConclusionDate = c.ConclusionDate,
                Status = c.IsAccept ? "approved" : (c.RejectReason != null ? "rejected" : "pending")
            };
        }
    }
}
using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using InsuranceAPI.Models;

namespace InsuranceAPI.Controllers
{
    [RoutePrefix("api/cases")]
    [Authorize]
    public class CasesController : ApiController
    {
        [HttpPost]
        [Route("")]
        public IHttpActionResult Create([FromBody] CreateCaseDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Description))
                return Content(HttpStatusCode.BadRequest,
                    new { error = "Заполните описание случая", code = "EMPTY_DESCRIPTION" });

            using (var db = new user30_dbEntities1())
            {
                var contract = db.ins_InsuranceContracts.Find(dto.ContractId);
                if (contract == null)
                    return NotFound();

                if (!contract.IsAccept)
                    return Content(HttpStatusCode.BadRequest,
                        new { error = "Договор ещё не одобрен или отклонён", code = "CONTRACT_NOT_APPROVED" });

                var insuranceCase = new ins_InsuranceCases
                {
                    ContractID = dto.ContractId,
                    CaseDate = dto.CaseDate,
                    Description = dto.Description,
                    PaymentAmount = dto.PaymentAmount,
                    IsApproved = false
                };

                db.ins_InsuranceCases.Add(insuranceCase);
                db.SaveChanges();

                return Created($"api/cases/{insuranceCase.CaseID}",
                    MapToDto(insuranceCase, contract));
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        public IHttpActionResult GetById(int id)
        {
            using (var db = new user30_dbEntities1())
            {
                var ic = db.ins_InsuranceCases.Find(id);
                if (ic == null) return NotFound();
                var contract = db.ins_InsuranceContracts.Find(ic.ContractID);
                return Ok(MapToDto(ic, contract));
            }
        }

        private CaseResponseDto MapToDto(ins_InsuranceCases c, ins_InsuranceContracts contract)
        {
            return new CaseResponseDto
            {
                CaseId = c.CaseID,
                ContractNumber = contract?.ContractNumber,
                CaseDate = c.CaseDate,
                Description = c.Description,
                PaymentAmount = c.PaymentAmount,
                Status = c.IsApproved ? "approved" : "pending"
            };
        }
    }
}
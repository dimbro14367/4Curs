using System;
using System.Linq;
using System.Web.Http;

namespace InsuranceAPI.Controllers
{
    [RoutePrefix("api/reports")]
    [Authorize(Roles = "Администратор,Андеррайтер")]
    public class ReportsController : ApiController
    {
        [HttpGet]
        [Route("payments")]
        public IHttpActionResult Payments([FromUri] DateTime? from = null, [FromUri] DateTime? to = null)
        {
            DateTime dateFrom = from ?? new DateTime(DateTime.Now.Year, 1, 1);
            DateTime dateTo = (to ?? DateTime.Now).Date.AddDays(1);

            using (var db = new user30_dbEntities1())
            {
                var payments = db.ins_InsuranceCases
                    .Where(c => c.CaseDate >= dateFrom && c.CaseDate < dateTo)
                    .OrderBy(c => c.CaseDate)
                    .ToList()
                    .Select(c => new
                    {
                        c.CaseID,
                        c.CaseDate,
                        c.Description,
                        c.PaymentAmount,
                        IsApproved = c.IsApproved,
                        ContractNumber = db.ins_InsuranceContracts
                            .Where(x => x.ContractID == c.ContractID)
                            .Select(x => x.ContractNumber).FirstOrDefault()
                    })
                    .ToList();

                decimal totalApproved = payments
                    .Where(p => p.IsApproved && p.PaymentAmount.HasValue)
                    .Sum(p => p.PaymentAmount.Value);

                return Ok(new
                {
                    period = new { from = dateFrom, to = dateTo.AddDays(-1) },
                    totalPaid = totalApproved,
                    count = payments.Count,
                    payments
                });
            }
        }
    }
}
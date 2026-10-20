using System;

namespace InsuranceAPI.Models
{
    public class CreateCaseDto
    {
        public int ContractId { get; set; }
        public DateTime CaseDate { get; set; }
        public string Description { get; set; }
        public decimal? PaymentAmount { get; set; }
    }

    public class CaseResponseDto
    {
        public int CaseId { get; set; }
        public string ContractNumber { get; set; }
        public DateTime CaseDate { get; set; }
        public string Description { get; set; }
        public decimal? PaymentAmount { get; set; }
        public string Status { get; set; }
    }
}
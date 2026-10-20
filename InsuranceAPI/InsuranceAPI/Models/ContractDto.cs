using System;

namespace InsuranceAPI.Models
{
    public class CreateContractDto
    {
        public int ClientId { get; set; }
        public int TypeId { get; set; }
        public decimal InsuredSum { get; set; }
        public decimal Premium { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class ContractResponseDto
    {
        public int ContractId { get; set; }
        public string ContractNumber { get; set; }
        public string ClientName { get; set; }
        public string TypeName { get; set; }
        public decimal InsuredSum { get; set; }
        public decimal Premium { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime ConclusionDate { get; set; }
    }
}
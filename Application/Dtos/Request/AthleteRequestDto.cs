﻿namespace Application.Dtos.Request
{
    public class AthleteRequestDto
    {
        public string AthleteName { get; set; } = string.Empty;
        public string AthleteLastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public string Genre { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool? Status { get; set; } = true;
        public int? MembershipId { get; set; }
        public DateOnly? StartMembershipDate { get; set; }
        public string? CardAccessCode { get; set; } = string.Empty;
        public string? DocumentID { get; set; }
    }
}

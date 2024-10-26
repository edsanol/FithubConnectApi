﻿namespace Infrastructure.Commons.Bases.Response
{
    public class AthleteAssistenceDto
    {
        public int AthleteId { get; set; }
        public string AthleteName { get; set; } = string.Empty;
        public string AthleteLastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateOnly DateAssistence { get; set; }
        public TimeOnly TimeAssistence { get; set; }
    }
}

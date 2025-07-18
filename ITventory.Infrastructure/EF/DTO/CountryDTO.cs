﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITventory.Infrastructure.EF.DTO
{
    public class CountryDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string CountryCode { get; set; }
        public string Region { get; set; }
        public string? Regulations { get; set; }

        public IEnumerable<LocationDTO>? Locations { get; set; }
    }
}

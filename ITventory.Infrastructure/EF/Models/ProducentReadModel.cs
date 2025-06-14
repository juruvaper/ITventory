﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITventory.Infrastructure.EF.Models
{
    public class ProducentReadModel
    {
       public Guid Id { get; set; }
       public string Name { get; set; }
       public Guid CountryId { get; set; }

        // załączenie całych obiektów w read modelu

       public CountryReadModel Country { get; set; }
       public ICollection<ModelReadModel> Models { get; set; }
    }
}

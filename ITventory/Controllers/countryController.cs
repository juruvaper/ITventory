﻿using ITventory.Application.Queries.Country;
using ITventory.Application.Services.CountryService.Add_country;
using ITventory.Application.Services.CountryService.Add_regulations;
using ITventory.Infrastructure.EF.DTO;
using ITventory.Infrastructure.EF.Queries.Country;
using ITventory.Shared.Abstractions.Commands;
using ITventory.Shared.Abstractions.Queries;
using Microsoft.AspNetCore.Mvc;

namespace ITventory.Controllers.Country
{
    public class countryController : BaseController
    {
        public countryController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher) : base(commandDispatcher, queryDispatcher)
        {
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateCountry command)
        {

            await _commandDispatcher.DispatchAsync(command);
            return Created();
        }

        [HttpPost("regulations")]
        public async Task<IActionResult> Post([FromBody] SetRegulations command)
        {

            await _commandDispatcher.DispatchAsync(command);
            return Created();
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CountryDTO>> GetById([FromRoute] Guid id)
        {
            var query = new GetCountryById { Id = id };
            var result = await _queryDispatcher.QueryAsync(query);
            return OkOrNotFound(result);
        }

        [HttpGet]
        public async Task<ActionResult<ICollection<CountryDTO>>> Get([FromQuery] GetCountry query)
        {
            var result = await _queryDispatcher.QueryAsync(query);
            return OkOrNotFound(result);
        }

    }
}

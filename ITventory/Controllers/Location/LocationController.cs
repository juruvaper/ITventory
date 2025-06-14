﻿using ITventory.Application.Services.CountryService.Add_regulations;
using ITventory.Application.Services.LocationService.AddLocation;
using ITventory.Shared.Abstractions.Commands;
using ITventory.Shared.Abstractions.Queries;
using Microsoft.AspNetCore.Mvc;

namespace ITventory.Controllers.Location
{
    public class LocationController : BaseController
    {
        public LocationController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher) : base(commandDispatcher, queryDispatcher)
        {
        }

        [HttpPost]

        public async Task<IActionResult> Post([FromBody] AddLocation command)
        {

            await _commandDispatcher.DispatchAsync(command);
            return Created();
        }
    }
}

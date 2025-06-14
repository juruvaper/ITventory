﻿using ITventory.Infrastructure.Identity.RegistrationService;
using ITventory.Shared.Abstractions.Commands;
using ITventory.Shared.Abstractions.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITventory.Controllers.Identity
{
    public class IdentityController : BaseController
    {
        public IdentityController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher) : base(commandDispatcher, queryDispatcher)
        {
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RegisterUser command)
        {

            await _commandDispatcher.DispatchAsync(command);
            return Created();
        }

    }
}

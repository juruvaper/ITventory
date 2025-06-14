﻿using ITventory.Application.Services.Equipment_service.Add_equipment;
using ITventory.Shared.Abstractions.Commands;
using ITventory.Shared.Abstractions.Queries;
using Microsoft.AspNetCore.Mvc;

namespace ITventory.Controllers.Equipment
{
    public class EquipmentController : BaseController
    {
        public EquipmentController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher) : base(commandDispatcher, queryDispatcher)
        {
            
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AddEquipment command)
        {
            await _commandDispatcher.DispatchAsync(command);
            return Created();
        }
    }
}

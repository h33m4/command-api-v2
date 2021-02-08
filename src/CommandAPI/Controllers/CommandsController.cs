using System.Collections.Generic;
using CommandAPI.Data;
using CommandAPI.Models;
using CommandAPI.Dtos;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;


namespace CommandAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandAPIRepo _repository;
        private readonly IMapper _mapper;

        public CommandsController(ICommandAPIRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetAllCommands()
        {
            var commandItems = _repository.GetAllCommands();

            var commandReadItems = _mapper.Map<IEnumerable<CommandReadDto>>(commandItems);

            return Ok(commandReadItems);
        }

        [HttpGet("{id}", Name = "GetCommandById")]
        public ActionResult<Command> GetCommandById(int id)
        {
            var commandItem = _repository.GetCommandById(id);

            if (commandItem == null)
            {
                return NotFound();
            }

            var commandReadItem = _mapper.Map<CommandReadDto>(commandItem);

            return Ok(commandReadItem);
        }

        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommand(CommandCreateDto commandCreateDto)
        {
            var commandModel = _mapper.Map<Command>(commandCreateDto);
            _repository.CreateCommand(commandModel);
            _repository.SaveChanges();

            var commandReadDto = _mapper.Map<CommandReadDto>(commandModel);

            return CreatedAtRoute(nameof(GetCommandById), new { Id = commandReadDto.Id }, commandReadDto);
        }

        [HttpPut("{id}")]
        public ActionResult UpdateCommand(int id, CommandUpdateDto commandUpdateDto)
        {
            var commandModelfromRepo = _repository.GetCommandById(id);

            if (commandModelfromRepo == null)
            {
                return NotFound();
            }

            _mapper.Map(commandUpdateDto, commandModelfromRepo);

            _repository.UpdateCommand(commandModelfromRepo);
            _repository.SaveChanges();

            return NoContent();
        }

        

        [HttpPatch("{id}")]
        public ActionResult PartialCommandUpdate(int id, JsonPatchDocument<CommandUpdateDto> patchDoc)
        {
            var commandModelFromRepo = _repository.GetCommandById(id);

            if (commandModelFromRepo == null)
            {
                return NotFound();
            }

            var commandToPatch = _mapper.Map<CommandUpdateDto>(commandModelFromRepo);
            patchDoc.ApplyTo(commandToPatch, ModelState);

            if (!TryValidateModel(commandToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(commandToPatch, commandModelFromRepo);

            _repository.UpdateCommand(commandModelFromRepo);
            _repository.SaveChanges();

            return NoContent();

        }

        [HttpDelete("{id}")]
        public ActionResult DeleteCommand(int id)
        {
            var commandItem = _repository.GetCommandById(id);

            if (commandItem == null)
            {
                return NotFound();
            }

            _repository.DeleteCommand(commandItem);
            _repository.SaveChanges();

            return NoContent();
        }
    }
}
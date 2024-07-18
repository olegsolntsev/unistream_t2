using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using unistream_t2.Interfaces;
using unistream_t2.Models;

namespace unistream_t2.Controllers;

[ApiController]
[Route("")]
public class EntitiesController : ControllerBase
{
    private readonly IEntityRepo _entityRepo;
    private readonly ILogger<EntitiesController> _logger;

    public EntitiesController(IEntityRepo entityRepo, ILogger<EntitiesController> logger)
    {
        // Через DI внедряем текущую реализацию (в виде хранилища в памяти) интерфейса репозитория
        // В будущем (или в зависимости от окружения и/или настроек) будем внедрять боевую реализацию (какое-нибудь распределенное хранилище кластерного типа и все такое...)
        _entityRepo = entityRepo ?? throw new ArgumentNullException(nameof(entityRepo));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    private static ProblemDetails GetBadRequestProblemDetails(string detail) => new ProblemDetails() { Status = 400, Title = "Bad request", Detail = detail };

    [HttpGet]
    public async Task<ActionResult<Entity>> Get([FromQuery] Guid get)
    {
        var entity = await _entityRepo.GetAsync(get);
        if (entity == null) return NotFound();
        return Ok(entity); // Здесь автоматическая сериализация в json
    }

    [HttpPost]
    public async Task<ActionResult> Insert([FromQuery] string insert)
    {
        Entity entity;

        try
        {
            entity = JsonSerializer.Deserialize<Entity>(insert, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (Exception e)
        {
            _logger.LogError("Json serialization exception: {ExceptionMessage}", e.Message);
            return BadRequest(GetBadRequestProblemDetails($"Json serialization exception: {e.Message}")); // Для унификации отправки сообщений об ошибках при межсервисном обмене 
        }

        if (await _entityRepo.InsertAsync(entity))
        {
            _logger.LogInformation("Successfull insert: entity Id = {Entity.Id}", entity.Id);
            return Ok();
        }
        else
        {
            _logger.LogError("Insert failed, entity already exists: entity Id = {Entity.Id}", entity.Id);
            return BadRequest(GetBadRequestProblemDetails("Insert failed, entity already exists")); // Для унификации отправки сообщений об ошибках при межсервисном обмене 
        }
    }
}
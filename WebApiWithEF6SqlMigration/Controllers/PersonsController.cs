using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiWithEF6SqlMigration.Data;
using WebApiWithEF6SqlMigration.Models;

namespace WebApiWithEF6SqlMigration.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PersonsController : ControllerBase
    {
        private readonly PersonApiDbContext _dbContext;
        private readonly ILogger<PersonsController> _logger;

        public PersonsController(PersonApiDbContext dbContext, ILogger<PersonsController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetPersonsAsync()
        {
            try
            {
                return new OkObjectResult (await _dbContext.Persons.ToListAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError($"there was a Problem {ex.Message}");
            }
            return Problem(statusCode: StatusCodes.Status500InternalServerError);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPersonByIiAsync(string id)
        {
            try
            {
                var person = await _dbContext.Persons.FindAsync(id);
                if (person == null)
                {
                    return NotFound();
                }

                return new OkObjectResult (person);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error while trying to get person by id {ex.Message}");
            }
            return Problem(statusCode: StatusCodes.Status500InternalServerError);
        }

        [HttpPost]
        public async Task<IActionResult> AddPersonAsync([FromBody] Person person)
        {
            try
            {
                await _dbContext.Persons.AddAsync(person);
                await _dbContext.SaveChangesAsync();

                return new OkObjectResult(person);
            }
            catch (Exception ex)
            {
                _logger.LogError($"there was a Problem {ex.Message}");
            }
            return Problem(statusCode: StatusCodes.Status500InternalServerError);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePersonByIdAsync([FromRoute] string id, [FromBody] Person person)
        {
            try
            {
                var dbPerson = await _dbContext.Persons.FindAsync(id);
                if (dbPerson == null)
                {
                    await _dbContext.Persons.AddAsync(person);
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation($"the person you searched for was not found added a new person to database");

                    return new OkObjectResult(person);
                }
                UpdatePersonInfo(person, dbPerson);
                await _dbContext.SaveChangesAsync();
                return new OkObjectResult(dbPerson);
            }
            catch (Exception ex)
            {
                _logger.LogError($"there was a problem while updating database {ex.Message}");
            }
            return Problem(statusCode: StatusCodes.Status500InternalServerError);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePersonByIiAsync(string id)
        {
            try
            {
                var person = await _dbContext.Persons.FindAsync(id);
                if (person == null)
                {
                    return NotFound();
                }
                _dbContext.Remove(person);
                await _dbContext.SaveChangesAsync();

                return new OkObjectResult(person);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while trying to delete person by id {ex.Message}");
            }
            return Problem(statusCode: StatusCodes.Status500InternalServerError);
        }

        private static void UpdatePersonInfo(Person person, Person? dbPerson)
        {
            dbPerson.FullName = person.FullName;
            dbPerson.Education = person.Education;
            dbPerson.Age = person.Age;
            dbPerson.Email = person.Email;
        }
    }
}

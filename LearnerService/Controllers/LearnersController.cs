using LearnerService.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LearnerService.Data;
using Microsoft.AspNetCore.Connections;
using System.Text;
using RabbitMQ.Client;
using Newtonsoft.Json;

namespace LearnerService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LearnersController : ControllerBase
    {
        private readonly SchoolContext _context;

        public LearnersController(SchoolContext context)
        {
            _context = context;
        }

        // GET: api/LearnersAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Learner>>> GetLearners()
        {
            if (_context.Learners == null)
            {
                return NotFound();
            }
            
            return await _context.Learners.Include(m=>m.Major).ToListAsync();
        }

        // GET: api/LearnersAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Learner>> GetLearner(int id)
        {
            if (_context.Learners == null)
            {
                return NotFound();
            }
            var learner = await _context.Learners.FindAsync(id);

            if (learner == null)
            {
                return NotFound();
            }

            return learner;
        }

        // PUT: api/LearnersAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLearner(int id, Learner learner)
        {
            if (id != learner.LearnerId)
            {
                return BadRequest();
            }

            _context.Entry(learner).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LearnerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/LearnersAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Learner>> PostLearner([Bind("LastName,FirstMidName,EnrollmentDate,MajorId")] Learner learner)
        {
            if (_context.Learners == null)
            {
                return Problem("Entity set 'SchoolContext.Learners'  is null.");
            }
            _context.Learners.Add(learner);
            await _context.SaveChangesAsync();
            var integrationEventData = JsonConvert.SerializeObject(new
            {
                id = learner.LearnerId,
                lname = learner.LastName,
                lfname = learner.FirstMidName
            }); 
            PublishToMessageQueue("learner.add", integrationEventData);

            return CreatedAtAction("GetLearner", new { id = learner.LearnerId }, learner);
        }
        private void PublishToMessageQueue(string integrationEvent, string eventData)
        {
            // TOOO: Reuse and close connections and channel, etc, 
            var factory = new ConnectionFactory();
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            var body = Encoding.UTF8.GetBytes(eventData);
            channel.BasicPublish(exchange: "learner",
                                 routingKey: integrationEvent,
                                 basicProperties: null,
                                 body: body);
        }
        // DELETE: api/LearnersAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLearner(int id)
        {
            if (_context.Learners == null)
            {
                return NotFound();
            }
            var learner = await _context.Learners.FindAsync(id);
            if (learner == null)
            {
                return NotFound();
            }

            _context.Learners.Remove(learner);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LearnerExists(int id)
        {
            return (_context.Learners?.Any(e => e.LearnerId == id)).GetValueOrDefault();
        }
    }
}

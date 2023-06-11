using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RwaMovies.DTOs;
using RwaMovies.Models;
using System.Net.Mail;

namespace RwaMovies.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly RwaMoviesContext _context;
        private readonly IMapper _mapper;

        public NotificationsController(RwaMoviesContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Notification>>> GetNotifications()
        {
            return await _context.Notifications.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Notification>> GetNotification(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
                return NotFound();
            return notification;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutNotification(int id, NotificationRequest notificationRequest)
        {
            if (!ModelState.IsValid || id != notificationRequest.Id)
                return BadRequest();
            try
            {
                var notification = _mapper.Map<Notification>(notificationRequest);
                _context.Entry(notification).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NotificationExists(id))
                    return NotFound();
                throw;
            }
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Notification>> PostNotification(NotificationRequest notificationRequest)
        {
            var notification = _mapper.Map<Notification>(notificationRequest);
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetNotification", new { id = notification.Id });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
                return NotFound();
            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("[action]")]
        public async Task<ActionResult> GetUnsentCount()
        {
            return Ok(await _context.Notifications.CountAsync(x => !x.SentAt.HasValue));
        }

        [HttpPost("[action]")]
        public ActionResult SendAllUnsent()
        {
            var client = new SmtpClient("127.0.0.1", 25);
            var sender = "admin@my-cool-webapi.com";
            try
            {
                var unsentNotifications = _context.Notifications.Where(x => !x.SentAt.HasValue);
                foreach (var notification in unsentNotifications)
                {
                    try
                    {
                        var mail = new MailMessage(
                            from: new MailAddress(sender),
                            to: new MailAddress(notification.ReceiverEmail));
                        mail.Subject = notification.Subject;
                        mail.Body = notification.Body;
                        client.Send(mail);
                        notification.SentAt = DateTime.UtcNow;
                        _context.SaveChanges();
                    }
                    catch (Exception)
                    {
                        // Black hole for notification is bad handling :(
                    }
                }
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        private bool NotificationExists(int id)
        {
            return (_context.Notifications?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RwaMovies.DTOs;
using RwaMovies.Models;
using RwaMovies.Services;

namespace RwaMovies.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly RwaMoviesContext _context;
        private readonly IMapper _mapper;
        private readonly IMailService _mail;

        public NotificationsController(RwaMoviesContext context, IMapper mapper, IMailService mail)
        {
            _context = context;
            _mapper = mapper;
            _mail = mail;
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

        [HttpGet("[action]")]
        public async Task<ActionResult> SendAllUnsent()
        {
            var notifications = await _context.Notifications.Where(x => !x.SentAt.HasValue).ToListAsync();
            foreach (var notification in notifications.Take(1))
            {
                await _mail.Send(notification.ReceiverEmail, notification.Subject, notification.Body);
                notification.SentAt = DateTime.Now;
            }
            await _context.SaveChangesAsync();
            return Ok();
        }

        private bool NotificationExists(int id)
        {
            return (_context.Notifications?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

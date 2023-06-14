using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RwaMovies.Services;
using RwaMovies.Models.DAL;
using RwaMovies.Models.Shared;

namespace RwaMovies.Controllers.API
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [Route("api/[controller]"), Area("API")]
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
                return Ok();
            }
            catch
            {
                if (!NotificationExists(id))
                    return NotFound();
                throw;
            }
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

        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [HttpGet("[action]")]
        public async Task<ActionResult> GetUnsentCount()
        {
            return Ok(await _context.Notifications.CountAsync(x => !x.SentAt.HasValue));
        }

        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [HttpGet("[action]")]
        public async Task<ActionResult> SendAllUnsent()
        {
            var notifications = await _context.Notifications.Where(x => !x.SentAt.HasValue).ToListAsync();
            foreach (var notification in notifications)
            {
                await _mail.Send(notification.ReceiverEmail, notification.Subject, notification.Body);
                notification.SentAt = DateTime.Now;
            }
            await _context.SaveChangesAsync();
            return Ok("Success");
        }

        private bool NotificationExists(int id)
        {
            return (_context.Notifications?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

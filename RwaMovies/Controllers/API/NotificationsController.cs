using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RwaMovies.Services;
using RwaMovies.Models.DAL;
using RwaMovies.Models.Shared;
using Microsoft.AspNetCore.SignalR;

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
        private readonly IHubContext<NotificationsHub> _hubContext;
        private static bool isSending = false;

        public NotificationsController(RwaMoviesContext context, IMapper mapper, IMailService mail, IHubContext<NotificationsHub> hubContext)
        {
            _context = context;
            _mapper = mapper;
            _mail = mail;
            _hubContext = hubContext;
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
                return BadRequest(ModelState);
            try
            {
                var notification = await _context.Notifications.FindAsync(id);
                _mapper.Map(notificationRequest, notification);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch
            {
                if (!NotificationExists(id))
                    return NotFound();
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostNotification(NotificationRequest notificationRequest)
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
        public async Task<ActionResult<int>> GetUnsentCount()
        {
            return Ok(await _context.Notifications.CountAsync(x => !x.SentAt.HasValue));
        }

        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [HttpGet("[action]")]
        public async Task<ActionResult<string>> SendAllUnsent()
        {
            if (isSending)
                return BadRequest("Notifications are already being sent.");
            isSending = true;
            var notifications = await _context.Notifications.Where(x => !x.SentAt.HasValue).ToListAsync();
            try
            {
                for (int i = notifications.Count - 1; i >= 0; i--)
                {
                    var n = notifications[i];
                    await _mail.Send(n.ReceiverEmail, n.Subject, n.Body);
                    n.SentAt = DateTime.Now;
                    await _hubContext.Clients.All.SendAsync("RemainingNotificationsCount", i);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            finally
            {
                await _context.SaveChangesAsync();
                isSending = false;
            }
            return Ok("Success");
        }

        private bool NotificationExists(int id)
        {
            return (_context.Notifications?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }

    public class NotificationsHub : Hub
    {
        public async Task SendMessage(string user, string message)
            => await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}

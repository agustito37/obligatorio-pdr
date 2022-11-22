using Microsoft.AspNetCore.Mvc;
using Shared;
using Logs;
using Shared.domain;

namespace AdministrationWebApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class LogsController : ControllerBase
    {
        static readonly SettingsManager settingsMgr = new SettingsManager();

        public LogsController()
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        }

        [HttpGet()]
        public ActionResult GetFiltered([FromQuery] int? type, [FromQuery] string? term, [FromQuery] string? date)
        {
            List<Log> logs = Persistence.Instance.GetLogs().FindAll((l) =>
                (type == null || l.Type == (LogType)type)
                && (term == null || l.Message.Contains(term))
                && (date == null || l.Date.ToShortDateString() == date)
            );
            return StatusCode(200, logs);
        }
    }      
}

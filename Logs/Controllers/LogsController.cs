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
        public ActionResult GetFiltered(int type)
        {
            List<Log> logs = Persistence.Instance.GetLogs().FindAll((l) => type == 0 || l.Type == (LogType)type);
            return StatusCode(200, logs);
        }
    }      
}

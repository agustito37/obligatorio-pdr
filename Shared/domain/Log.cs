using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.domain
{
    public class Log
    {
        public DateTime Date { get; set; }
        public LogType Type { get; set; }
        public string Message { get; set; } = "";

        public static string Encoder(Log log)
        {
            return $"{(int)log.Type}|{log.Message}";
        }

        public static Log Decoder(string encoded)
        {
            string[] parts = encoded.Split("|");

            return new Log()
            {
                Type = (LogType)Int32.Parse(parts[0]),
                Message = parts[1]
            };
        }
    }
}
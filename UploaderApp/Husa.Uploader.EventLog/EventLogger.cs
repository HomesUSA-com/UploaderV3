using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Husa.Cargador.EventLog
{
    public class EventLogWriter
    {
        public static int DEFAULT_MAX_MESSAGE_SIZE = 32766;

        public static void Write(string log, string source, int id, string message, EventLogEntryType type, bool isSmallMessage = false)
        {
            try
            {
                int messageMaxLenght = isSmallMessage ? 1024 : DEFAULT_MAX_MESSAGE_SIZE;
                if (!System.Diagnostics.EventLog.SourceExists(source))
                    System.Diagnostics.EventLog.CreateEventSource(source, log);

                if (message.Length > messageMaxLenght)
                    message = message.Substring(0, messageMaxLenght);

                System.Diagnostics.EventLog.WriteEntry(source, message, type, id);
            }
            catch {  }
        }

        public static void Write(SystemLog log, string source, int id, string message, EventLogEntryType type, bool isSmallMessage = false)
        {
            try
            {
                int messageMaxLenght = isSmallMessage ? 1024 : DEFAULT_MAX_MESSAGE_SIZE;

                if (!System.Diagnostics.EventLog.SourceExists(source))
                    System.Diagnostics.EventLog.CreateEventSource(source, EnumStringIdentifierAttribute.GetNameFromEnum(log));

                if (message.Length > messageMaxLenght)
                    message = message.Substring(0, messageMaxLenght);

                System.Diagnostics.EventLog.WriteEntry(source, message, type, id);
            }
            catch {  }
        }
    }

    public enum SystemLog
    {
        [EnumStringIdentifier("UA", "Uploader Application")]
        UploaderApp
    }
}

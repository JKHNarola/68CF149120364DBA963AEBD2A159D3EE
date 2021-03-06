﻿namespace App.BL
{
    public class AppSettings
    {
        public string DefaultPassword { get; set; }
        public int TokenAliveTime { get; set; }
        public string UTCOffset { get; set; }
        public string TimezoneName { get; set; }
        public string ExceptionEmailSendToName { get; set; }
        public string ExceptionEmailSendTo { get; set; }
    }
}

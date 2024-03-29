﻿using System.Collections.Generic;

namespace IdentityUtils.Commons.Mailing
{
    public enum EmailTypes { HTML, PLAINTEXT }

    public class MailMessageSimple
    {
        public MailMessageSimple(string from, string to, string subject, string content) :
            this(from, new string[] { to }, subject, content)
        {
        }

        public MailMessageSimple(string from, IEnumerable<string> to, string subject, string content)
        {
            From = from;
            Subject = subject;
            Content = content;
            To.AddRange(to);
        }

        public EmailTypes EmailType { get; set; } = EmailTypes.HTML;
        public string From { get; set; }
        public List<string> To { get; set; } = new List<string>();
        public string Subject { get; set; }
        public string Content { get; set; }
    }
}
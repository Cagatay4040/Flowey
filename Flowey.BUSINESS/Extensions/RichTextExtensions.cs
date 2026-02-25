using Ganss.Xss;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.Extensions
{
    public static class RichTextExtensions
    {
        private static readonly HtmlSanitizer _sanitizer;

        static RichTextExtensions()
        {
            _sanitizer = new HtmlSanitizer();

            _sanitizer.AllowedTags.Add("table");
            _sanitizer.AllowedTags.Add("tr");
            _sanitizer.AllowedTags.Add("td");
            _sanitizer.AllowedTags.Add("img");

            // Allow attributes used by quill-mention
            _sanitizer.AllowedAttributes.Add("data-id");
            _sanitizer.AllowedAttributes.Add("data-value");
            _sanitizer.AllowedAttributes.Add("data-denotation-char");
            _sanitizer.AllowedAttributes.Add("class");
            _sanitizer.AllowedClasses.Add("mention");
        }

        public static string ToSafeRichText(this string rawContent)
        {
            if (string.IsNullOrEmpty(rawContent))
                return rawContent;

            return _sanitizer.Sanitize(rawContent);
        }
    }
}

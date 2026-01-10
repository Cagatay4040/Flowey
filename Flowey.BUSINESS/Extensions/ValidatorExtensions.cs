using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.Extensions
{
    public static class ValidatorExtensions
    {
        private static readonly Regex HtmlTagRegex = new Regex("<.*?>", RegexOptions.Compiled);

        public static IRuleBuilderOptions<T, string> NotContainHtml<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .Must(content => string.IsNullOrEmpty(content) || !HtmlTagRegex.IsMatch(content));
        }
    }
}

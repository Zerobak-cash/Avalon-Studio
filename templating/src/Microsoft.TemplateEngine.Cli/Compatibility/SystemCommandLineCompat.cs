// Patch42: Extend SystemCommandLineCompat with additional helpers required for the templating CLI.
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Linq;

namespace Microsoft.TemplateEngine.Cli.Compatibility
{
    public static class SystemCommandLineCompat
    {
        // Check whether a parse result contains an option with the alias and that it has a token.
        public static bool HasOption(this ParseResult parseResult, string alias)
        {
            if (parseResult == null) return false;
            var opt = parseResult.CommandResult.Children
                .OfType<OptionResult>()
                .FirstOrDefault(o => o.Symbol?.Aliases?.Contains(alias) == true);
            return opt != null && opt.Tokens.Count > 0;
        }

        // Return token values for a SymbolResult as strings
        public static IReadOnlyList<string> ArgumentsList(this SymbolResult sym)
        {
            if (sym == null) return Array.Empty<string>();
            return sym.Tokens.Select(t => t.Value).ToArray();
        }

        // Option.RawAliases compatibility (returns primary aliases)
        public static IEnumerable<string> RawAliases(this Option option)
        {
            return option?.Aliases ?? Array.Empty<string>();
        }

        // GetValueForOption compatibility: return the typed value for an Option<T> if present, otherwise default
        public static T GetValueForOption<T>(this ParseResult parseResult, Option<T> option)
        {
            if (parseResult == null || option == null) return default!;
            // Try to find corresponding OptionResult by matching aliases
            var optResult = parseResult.CommandResult.Children
                .OfType<OptionResult>()
                .FirstOrDefault(o => o.Symbol?.Aliases?.Any(a => option.Aliases.Contains(a)) == true);
            if (optResult == null) return default!;
            try
            {
                // Use existing GetValueOrDefault behavior if available, otherwise parse tokens
                var val = optResult.GetValueOrDefault<T>();
                return val;
            }
            catch
            {
                // fallback: attempt simple conversion from first token
                if (optResult.Tokens.Count == 0) return default!;
                var tokenVal = optResult.Tokens[0].Value;
                return (T)Convert.ChangeType(tokenVal, typeof(T));
            }
        }

        // Provide a small helper shim to call OptionResult.GetValueOrDefault<T> safely (extension method if missing)
        public static T GetValueOrDefault<T>(this OptionResult optResult)
        {
            if (optResult == null) return default!;
            // Recent System.CommandLine has TryGetValue but older helpers differ — attempt the common approach:
            try
            {
                // If binding worked, Value is available via GetValueOrDefault on OptionResult
                var mi = typeof(OptionResult).GetMethod("GetValueOrDefault", Type.EmptyTypes);
                if (mi != null)
                {
                    return (T)mi.MakeGenericMethod(typeof(T)).Invoke(optResult, null)!;
                }
            }
            catch { }

            // Fallback: parse token
            if (optResult.Tokens.Count == 0) return default!;
            var tokenVal = optResult.Tokens[0].Value;
            return (T)Convert.ChangeType(tokenVal, typeof(T));
        }
    }
}

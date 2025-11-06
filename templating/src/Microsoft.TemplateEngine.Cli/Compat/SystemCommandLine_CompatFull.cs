using System;
using System.Collections;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace Microsoft.TemplateEngine.Cli.Compat
{
    public static class SystemCommandLineCompatFull
    {
        public static bool HasOption(this ParseResult parseResult, string alias)
        {
            if (parseResult == null) return false;
            try
            {
                var cmd = parseResult.CommandResult;
                if (cmd != null)
                {
                    var childrenProp = cmd.GetType().GetProperty("Children");
                    if (childrenProp != null)
                    {
                        var children = childrenProp.GetValue(cmd) as IEnumerable;
                        foreach (var c in children)
                        {
                            var symbol = c.GetType().GetProperty("Symbol")?.GetValue(c) ?? c.GetType().GetProperty("Name")?.GetValue(c);
                            if (symbol != null && symbol.ToString().TrimStart('-') == alias.TrimStart('-')) return true;
                        }
                    }
                }
                var tokensProp = parseResult.GetType().GetProperty("Tokens");
                if (tokensProp != null)
                {
                    var tokens = tokensProp.GetValue(parseResult) as IEnumerable;
                    foreach (var t in tokens)
                    {
                        if (t != null && t.ToString().Contains(alias)) return true;
                    }
                }
            }
            catch { }
            return false;
        }

        public static bool TryGetValue<T>(this ParseResult parseResult, Option option, out T value)
        {
            value = default;
            if (parseResult == null || option == null) return false;
            try
            {
                var method = typeof(ParseResult).GetMethod("GetValueForOption", new Type[] { option.GetType() });
                if (method != null)
                {
                    var v = method.Invoke(parseResult, new object[] { option });
                    if (v is T t) { value = t; return true; }
                    if (v != null) { value = (T)Convert.ChangeType(v, typeof(T)); return true; }
                }
                var cmd = parseResult.CommandResult;
                var childrenProp = cmd?.GetType().GetProperty("Children");
                if (childrenProp != null)
                {
                    var children = childrenProp.GetValue(cmd) as IEnumerable;
                    foreach (var c in children)
                    {
                        if (c == null) continue;
                        var sym = c.GetType().GetProperty("Symbol")?.GetValue(c) ?? c.GetType().GetProperty("Name")?.GetValue(c);
                        if (sym != null && sym.ToString().TrimStart('-') == (option.Name ?? option.ToString()).TrimStart('-'))
                        {
                            var argsProp = c.GetType().GetProperty("Arguments") ?? c.GetType().GetProperty("Tokens");
                            var args = argsProp?.GetValue(c);
                            if (args is IEnumerable en)
                            {
                                foreach (var a in en)
                                {
                                    if (a is T tt) { value = tt; return true; }
                                }
                            }
                        }
                    }
                }
            }
            catch { }
            return false;
        }
    }
}

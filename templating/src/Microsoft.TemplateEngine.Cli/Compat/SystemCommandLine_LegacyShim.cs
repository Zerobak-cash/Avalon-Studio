// SystemCommandLine_LegacyShim.cs - provides Arguments property on SymbolResult via extension to ease migration
using System;
using System.CommandLine.Parsing;

namespace Microsoft.TemplateEngine.Cli.Compat
{
    public static class SymbolResultExtensions
    {
        public static object Arguments(this SymbolResult sr)
        {
            if (sr == null) return null;
            try
            {
                var argsProp = sr.GetType().GetProperty("Arguments") ?? sr.GetType().GetProperty("Tokens");
                return argsProp?.GetValue(sr);
            }
            catch { return null; }
        }
    }
}

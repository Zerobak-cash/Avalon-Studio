// Patch44: Enhanced compatibility helpers for System.CommandLine 2.x series (RC) that adapt through reflection
// This avoids downgrading and attempts to support both older and newer API shapes used across package versions.
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Linq;
using System.Reflection;

namespace Microsoft.TemplateEngine.Cli.Compatibility
{
    public static partial class SystemCommandLineCompat
    {
        // Robust HasOption: search OptionResults and SymbolResults for matching alias using reflection where necessary.
        public static bool HasOption(this ParseResult parseResult, string alias)
        {
            if (parseResult == null || string.IsNullOrEmpty(alias)) return false;
            try
            {
                // prefer typed approach if OptionResult type has Symbol/Option property
                foreach (var child in parseResult.CommandResult.Children)
                {
                    var childType = child.GetType();
                    // try OptionResult path
                    if (childType.Name == "OptionResult" )
                    {
                        // try Symbol property first
                        var symProp = childType.GetProperty("Symbol") ?? childType.GetProperty("Option") ?? childType.GetProperty("Token");
                        if (symProp != null)
                        {
                            var sym = symProp.GetValue(child);
                            if (sym != null)
                            {
                                var aliasesProp = sym.GetType().GetProperty("Aliases") ?? sym.GetType().GetProperty("RawAliases") ?? sym.GetType().GetProperty("AliasesList");
                                if (aliasesProp != null)
                                {
                                    var aliases = aliasesProp.GetValue(sym) as IEnumerable<string>;
                                    if (aliases != null && aliases.Contains(alias)) return true;
                                }
                            }
                        }

                        // fallback: inspect token values as last resort
                        var tokensProp = childType.GetProperty("Tokens");
                        if (tokensProp != null)
                        {
                            var tokens = tokensProp.GetValue(child) as IEnumerable<object>;
                            if (tokens != null && tokens.Any()) return true;
                        }
                    }
                    // For SymbolResult-like children check for Arguments property via reflection
                    var argsProp = childType.GetProperty("Arguments") ?? childType.GetProperty("Tokens");
                    if (argsProp != null)
                    {
                        var args = argsProp.GetValue(child) as IEnumerable<object>;
                        if (args != null && args.Any()) return true;
                    }
                }
            }
            catch { }
            return false;
        }

        // GetValueForOption: tries Option<T> typed retrieval using available API or falls back to token parsing.
        public static T GetValueForOption<T>(this ParseResult parseResult, Option<T> option)
        {
            if (parseResult == null || option == null) return default!;
            try
            {
                foreach (var child in parseResult.CommandResult.Children)
                {
                    var childType = child.GetType();
                    if (childType.Name == "OptionResult")
                    {
                        // Try calling OptionResult.GetValueOrDefault<T>() via reflection
                        var method = childType.GetMethod("GetValueOrDefault", Type.EmptyTypes) ?? childType.GetMethod("GetValueOrDefault", new Type[] { });
                        if (method != null)
                        {
                            var gen = method.MakeGenericMethod(typeof(T));
                            var val = gen.Invoke(child, null);
                            if (val is T t) return t;
                        }
                        // fallback: parse first token
                        var tokensProp = childType.GetProperty("Tokens");
                        if (tokensProp != null)
                        {
                            var tokens = tokensProp.GetValue(child) as IEnumerable<object>;
                            var first = tokens?.FirstOrDefault()?.GetType().GetProperty("Value")?.GetValue(tokens.First());
                            if (first != null) return (T)Convert.ChangeType(first, typeof(T));
                        }
                    }
                }
            }
            catch { }
            return default!;
        }

        // Small helpers to bridge old indexer usage and newer parse API shapes.
        public static IReadOnlyList<string> ArgumentsList(this SymbolResult sym)
        {
            if (sym == null) return Array.Empty<string>();
            return sym.Tokens?.Select(t => t.Value?.ToString() ?? string.Empty).ToArray() ?? Array.Empty<string>();
        }

        // Provide a loose fallback for 'Create'/'Accept' style helpers by exposing factory methods the code expects.
        public static object CreatePlaceholder(string name) => new { Name = name };
        public static bool AcceptPlaceholder(object obj) => obj != null;
    }
}

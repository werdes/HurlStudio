using Avalonia.Platform;
using HurlStudio.Model.Enums;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.Extensions.Logging;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using TextMateSharp.Grammars;
using TextMateSharp.Internal.Grammars.Reader;
using TextMateSharp.Internal.Themes.Reader;
using TextMateSharp.Internal.Types;
using TextMateSharp.Registry;
using TextMateSharp.Themes;

namespace HurlStudio.UI.Editor
{
    public class LocalResourceGrammarRegistryOptions : IRegistryOptions
    {
        private ApplicationTheme _theme;
        private ILogger _log;

        public LocalResourceGrammarRegistryOptions(ApplicationTheme theme, ILogger log)
        {
            _theme = theme;
            _log = log;
        }

        public IRawTheme GetDefaultTheme()
        {
            return GetTheme(_theme.ToString());
        }

        public IRawTheme GetTheme(string scopeName)
        {
            string fileName = $"{scopeName}.json";
            string? assemblyName = Assembly.GetExecutingAssembly()?.GetName()?.Name;

            if (!string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(assemblyName))
            {
                Uri path = new Uri($"avares://{assemblyName}/Assets/SyntaxHighlighting/Themes/{fileName}");

                if (AssetLoader.Exists(path))
                {
                    Stream themeStream = AssetLoader.Open(path);
                    if (themeStream != null)
                    {
                        using (themeStream)
                        {
                            using (StreamReader reader = new StreamReader(themeStream))
                            {
                                IRawTheme rawTheme = ThemeReader.ReadThemeSync(reader);
                                return rawTheme;
                            }
                        }
                    }
                    else
                    {
                        _log.LogWarning($"Asset [{path}] can't be opened");
                    }
                }
                else
                {
                    _log.LogWarning($"Asset [{path}] doesn't exist");
                }
            }
#pragma warning disable CS8603
            return null;
#pragma warning restore CS8603
        }

        public IRawGrammar GetGrammar(string scopeName)
        {
            string fileName = $"{scopeName}.json";
            string? assemblyName = Assembly.GetExecutingAssembly()?.GetName()?.Name;

            if (!string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(assemblyName))
            {
                Uri path = new Uri($"avares://{assemblyName}/Assets/SyntaxHighlighting/Grammars/{fileName}");

                if (AssetLoader.Exists(path))
                {
                    Stream grammarStream = AssetLoader.Open(path);
                    if (grammarStream != null)
                    {
                        using (grammarStream)
                        {
                            using (StreamReader reader = new StreamReader(grammarStream))
                            {
                                IRawGrammar grammar = GrammarReader.ReadGrammarSync(reader);
                                return grammar;
                            }
                        }
                    }
                    else
                    {
                        _log.LogWarning($"Asset [{path}] can't be opened");
                    }
                }
                else
                {
                    _log.LogWarning($"Asset [{path}] doesn't exist");
                }
            }
#pragma warning disable CS8603 
            return null;
#pragma warning restore CS8603
        }

        public ICollection<string> GetInjections(string scopeName)
        {
#pragma warning disable CS8603
            return null;
#pragma warning restore CS8603
        }

    }
}

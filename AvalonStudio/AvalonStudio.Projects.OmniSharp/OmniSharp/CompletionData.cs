﻿namespace AvalonStudio.Languages.CSharp.OmniSharp
{
    public class CompletionData
    {
        public string CompletionText { get; set; }
        public string Description { get; set; }
        public string DisplayText { get; set; }
        public string RequiredNamespaceImport { get; set; }
        public string MethodHeader { get; set; }
        public string ReturnType { get; set; }
        public string Snippet { get; set; }
        public string Kind { get; set; }
    }
}
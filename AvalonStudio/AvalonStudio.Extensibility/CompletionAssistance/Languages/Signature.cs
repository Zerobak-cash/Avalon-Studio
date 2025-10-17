﻿using System.Collections.Generic;

namespace AvalonStudio.Extensibility.Languages.CompletionAssistance
{
    public class Signature
    {
        public Signature()
        {
            Parameters = new List<Parameter>();
            Exceptions = new List<SignatureException>();
        }

        public string Name { get; set; }
        public string BuiltInReturnType { get; set; }
        public string ReturnType { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public List<Parameter> Parameters { get; set; }
        public List<SignatureException> Exceptions { get; set; }
    }
}
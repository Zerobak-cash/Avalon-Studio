// Patch44: Re-declare Vtbl without 'override' to match generated base in newer CorApi.
using System;
namespace AvalonStudio.CorApi.Portable
{
    public partial class ManagedCallbackShadow
    {
        protected Vtbl Vtbl { get; set; }
        public class Vtbl { }
    }
}

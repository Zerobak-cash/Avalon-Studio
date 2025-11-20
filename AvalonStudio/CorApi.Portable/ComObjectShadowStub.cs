
namespace SharpGen.Runtime
{
    // Public shim so project code can inherit/compile even if the original ComObjectShadow
    // type in the SharpGen runtime assembly is internal or inaccessible.
    // This shim intentionally provides no implementation beyond a public base type.
    public class ComObjectShadow
    {
        // Intentionally empty - used as a public base type for generated shadows.
    }
}

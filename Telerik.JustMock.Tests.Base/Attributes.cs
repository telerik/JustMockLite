#if XUNIT
using System;
#if XUNIT2
using System.Collections.Generic;
using System.Linq;
#endif
using Xunit;
#if XUNIT2
using Xunit.Abstractions;
#endif
#endif

#if XUNIT2
#pragma warning disable xUnit1013 
#endif

namespace Telerik.JustMock.XUnit.Test.Attributes
{
#if XUNIT
    [SerializableAttribute]
    [AttributeUsageAttribute(AttributeTargets.Class, AllowMultiple = false)]
    public class EmptyTestClassAttribute : System.Attribute
    {
    }

    [SerializableAttribute]
    [AttributeUsageAttribute(AttributeTargets.Method, AllowMultiple = false)]
    public class EmptyTestMethodAttribute : System.Attribute
    {
    }

    [SerializableAttribute]
    [AttributeUsageAttribute(AttributeTargets.Method, AllowMultiple = false)]
    public class EmptyTestInitializeAttribute : System.Attribute
    {
    }

    [SerializableAttribute]
    [AttributeUsageAttribute(AttributeTargets.Method, AllowMultiple = false)]
    public class EmptyTestCleanupAttribute : System.Attribute
    {
    }

    [AttributeUsageAttribute(AttributeTargets.Method, AllowMultiple = true)]
#if !XUNIT2
    public class XUnitCategoryAttribute : Xunit.TraitAttribute
    {
        public XUnitCategoryAttribute(string category) : base("Category", category)
        {
        }
    }
#else
    [Xunit.Sdk.TraitDiscovererAttribute("Telerik.JustMock.XUnit.Test.Attributes.XunitCategoryDiscoverer", "Telerik.JustMock.XUnit.Tests")]
    public class XUnitCategoryAttribute : Attribute, Xunit.Sdk.ITraitAttribute
    {
        public XUnitCategoryAttribute(string category)
        {
        }
    }
    public class XunitCategoryDiscoverer : Xunit.Sdk.ITraitDiscoverer
    {
        /// <inheritdoc/>
        public virtual IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
        {
            var ctorArgs = traitAttribute.GetConstructorArguments().Cast<string>().ToList();
            yield return new KeyValuePair<string, string>("Category", ctorArgs[0]);
        }
    }
#endif
#endif
}

#if XUNIT2
#pragma warning restore xUnit1013 
#endif

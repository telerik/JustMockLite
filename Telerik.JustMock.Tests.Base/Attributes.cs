#if XUNIT
using System;
using Xunit;
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
    public class XUnitCategoryAttribute : Xunit.TraitAttribute
    {
        public XUnitCategoryAttribute(string category) : base("Category", category) 
        {
        }
    }
#endif
}
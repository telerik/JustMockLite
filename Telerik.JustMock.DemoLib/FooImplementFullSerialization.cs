using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.JustMock.DemoLib
{
    [Serializable]
    public class FooImplementSerializationAttributeAndInterface : ISerializable
    {
        public virtual int Value { get; set; }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }
}

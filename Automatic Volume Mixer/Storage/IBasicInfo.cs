using System;
using System.Collections;

namespace Avm.Storage
{
    public interface IBasicInfo : ICloneable
    {
        string Name { get; set; }
        bool Enabled { get; set; }
        string GetDetails();
        Guid Id { get; set; }
    }
}
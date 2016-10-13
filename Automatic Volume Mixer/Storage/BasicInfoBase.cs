using System;
using System.Collections.Generic;
using System.ComponentModel;
using Klocman.Extensions;

namespace Avm.Storage
{
    public abstract class BasicInfoBase : IBasicInfo
    {
        protected BasicInfoBase()
        {
            ID = Guid.NewGuid();
        }

        private static readonly IEnumerable<string> NameTrimmers = new[] {"trigger", "action"};
        private string _name;

        [Category("Basic")]
        [Description("If false this element will not be processed.")]
        [DefaultValue(true)]
        public virtual bool Enabled { get; set; } = true;

        [Category("Basic")]
        [Description("Name of this element.")]
        public virtual string Name
        {
            get { return _name ?? (_name = TrimName(GetType().Name)); }
            set { _name = value; }
        }
        public Guid ID { get; set; }

        public abstract string GetDetails();
        public abstract object Clone();

        public static string TrimName(string toTrim)
            => toTrim.ExtendedTrimEndAny(NameTrimmers, StringComparison.OrdinalIgnoreCase).ToTitleCase();
    }
}
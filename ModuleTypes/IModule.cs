using System.Collections.Generic;
using System.Drawing;

namespace ModuleTypes
{
    public interface IModule
    {
        Bitmap Canvas { get; }
        Dictionary<string, string> Fields { get; }
        string Description { get; }

        void Generate();
    }
}

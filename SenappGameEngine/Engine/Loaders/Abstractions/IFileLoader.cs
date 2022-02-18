using System.IO;

namespace Senapp.Engine.Loaders.Abstractions
{
    public interface IFileLoader<TOuput>
    {
        public bool LoadFile(StringReader fileData, out TOuput output);
    }
}

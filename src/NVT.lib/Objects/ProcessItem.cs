using System.Diagnostics;

namespace NVT.lib.Objects
{
    public class ProcessItem
    {
        public string Name => _process.ProcessName;

        public string FileName => _process.MainModule.FileName;

        public int ID => _process.Id;

        private readonly Process _process;

        public ProcessItem(Process process)
        {
            _process = process;
        }
    }
}
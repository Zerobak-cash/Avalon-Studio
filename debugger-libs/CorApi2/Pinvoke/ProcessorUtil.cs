using System;

namespace PinvokeKit
{
    [CLSCompliant(false)]
    public static class ProcessorUtil
    {
        public static readonly ProcessorArchitecture Architecture;
        public static readonly ProcessorArchitecture NativeArchitecture;

        static unsafe ProcessorUtil()
        {
            if (PlatformUtil.IsRunningUnderWindows)
            {
                var systemInfo = new SYSTEM_INFO();

                Kernel32Dll.GetSystemInfo(&systemInfo);
                Architecture = (ProcessorArchitecture) systemInfo.wProcessorArchitecture;

                try
                {
                    // Note: No this function in Windows 2000 !!!
                    Kernel32Dll.GetNativeSystemInfo(&systemInfo);
                }
                catch (EntryPointNotFoundException) {}

                NativeArchitecture = (ProcessorArchitecture) systemInfo.wProcessorArchitecture;
            }
            else
            {
                var architecture = Environment.Is64BitProcess
                    ? ProcessorArchitecture.PROCESSOR_ARCHITECTURE_AMD64
                    : ProcessorArchitecture.PROCESSOR_ARCHITECTURE_INTEL;

                Architecture = architecture;
                NativeArchitecture = architecture;
            }
        }
    }
}
using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace HurlStudio.Utility
{
    public static class PlatformUtils
    {
        private static bool _isWindows;
        private static bool _isLinux;
        private static bool _isMacOS;

        static PlatformUtils()
        {
            _isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            _isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            _isMacOS = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        }
        
        public static bool IsWindows => _isWindows;
        public static bool IsLinux => _isLinux;
        public static bool IsMacOS => _isMacOS;
    }
}
﻿namespace Spotlight.Core.Memory
{
    using System;
    using System.Runtime.InteropServices;

    using Rage;

    internal static unsafe class GameMemory
    {
        public static CCoronaDrawQueue* CoronaDrawQueue { get; private set; }
        public static int TlsAllocatorOffset0 { get; private set; }
        public static int TlsAllocatorOffset1 { get; private set; }
        public static int TlsAllocatorOffset2 { get; private set; }

        public static bool Init()
        {
            IntPtr address = Game.FindPattern("48 8D 0D ?? ?? ?? ?? F3 0F 11 44 24 ?? F3 0F 11 64 24 ?? E8 ?? ?? ?? ?? 4C 8D 9C 24 ?? ?? ?? ??");
            if (AssertAddress(address, nameof(CCoronaDrawQueue)))
            {
                address = address + *(int*)(address + 3) + 7;
                CoronaDrawQueue = (CCoronaDrawQueue*)address;
            }

            address = Game.FindPattern("B8 ?? ?? ?? ?? 48 89 1C 10 B8 ?? ?? ?? ?? 48 89 1C 10 B8 ?? ?? ?? ?? 48 89 1C 10 E8 ?? ?? ?? ?? 48 8D 15 ?? ?? ?? ??");
            if (AssertAddress(address, "TlsAllocatorOffsets"))
            {
                TlsAllocatorOffset0 = *(int*)(address + 1);
                TlsAllocatorOffset1 = *(int*)(address + 10);
                TlsAllocatorOffset2 = *(int*)(address + 19);
            }

            return !anyAssertFailed;
        }

        private static bool anyAssertFailed = false;
        private static bool AssertAddress(IntPtr address, string name)
        {
            if (address == IntPtr.Zero)
            {
                Game.LogTrivial($"Incompatible game version, couldn't find {name} instance.");
                anyAssertFailed = true;
                return false;
            }

            return true;
        }
    }
}

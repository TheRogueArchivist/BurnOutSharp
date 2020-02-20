﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class DiscGuard
    {
        public static string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                if (files.Any(f => Path.GetFileName(f).Equals("IOSLINK.VXD", StringComparison.OrdinalIgnoreCase))
                    && files.Any(f => Path.GetFileName(f).Equals("IOSLINK.SYS", StringComparison.OrdinalIgnoreCase)))
                {
                    return "DiscGuard";
                }
            }
            else
            {
                if (Path.GetFileName(path).Equals("IOSLINK.VXD", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("IOSLINK.DLL", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("IOSLINK.SYS", StringComparison.OrdinalIgnoreCase))
                {
                    return "DiscGuard";
                }
            }

            return null;
        }
    }
}

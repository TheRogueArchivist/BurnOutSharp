﻿using System.Linq;
using System.Text;
using BurnOutSharp.ExecutableType.Microsoft;

namespace BurnOutSharp.ProtectionType
{
    public class ThreePLock : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug, PortableExecutable pex, NewExecutable nex)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            //This produced false positives in some DirectX 9.0c installer files
            //"Y" + (char)0xC3 + "U" + (char)0x8B + (char)0xEC + (char)0x83 + (char)0xEC + "0SVW"

            // Get the .ldr and .ldt sections, if they exist -- TODO: Confirm if both are needed or either/or is fine
            var cmsdSection = sections.FirstOrDefault(s => Encoding.ASCII.GetString(s.Name).StartsWith(".ldr"));
            var cmstSection = sections.FirstOrDefault(s => Encoding.ASCII.GetString(s.Name).StartsWith(".ldt"));
            if (cmsdSection != null || cmstSection != null)
                return $"3PLock";

            return null;
        }
    }
}

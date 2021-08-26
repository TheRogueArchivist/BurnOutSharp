using System.Collections.Generic;
using System.IO;
using System.Linq;
using BurnOutSharp.ExecutableType.Microsoft.Tables;

namespace BurnOutSharp.ExecutableType.Microsoft.Sections
{
    /// <summary>
    /// All image files that import symbols, including virtually all executable (EXE) files, have an .idata section.
    /// A typical file layout for the import information follows:
    ///     Directory Table
    ///     Null Directory Entry
    ///     DLL1 Import Lookup Table
    ///     Null
    ///     DLL2 Import Lookup Table
    ///     Null
    ///     DLL3 Import Lookup Table
    ///     Null
    ///     Hint-Name Table
    /// </summary>
    /// <remarks>https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#the-idata-section</remarks>
    internal class ImportDataSection
    {
        /// <summary>
        /// Import directory table
        /// </summary>
        public ImportDirectoryTable ImportDirectoryTable;

        /// <summary>
        /// Import lookup tables
        /// </summary>
        public ImportLookupTable[] ImportLookupTables;

        /// <summary>
        /// Hint/Name table
        /// </summary>
        public HintNameTable HintNameTable;

        public static ImportDataSection Deserialize(Stream stream, bool pe32plus, int hintCount)
        {
            var ids = new ImportDataSection();

            ids.ImportDirectoryTable = ImportDirectoryTable.Deserialize(stream);

            List<ImportLookupTable> tempLookupTables = new List<ImportLookupTable>();
            while (true)
            {
                var tempLookupTable = ImportLookupTable.Deserialize(stream, pe32plus);
                if (tempLookupTable.EntriesPE32 == null && tempLookupTable.EntriesPE32Plus == null)
                    break;
                
                tempLookupTables.Add(tempLookupTable);
            }

            ids.HintNameTable = HintNameTable.Deserialize(stream, hintCount);

            return ids;
        }

        public static ImportDataSection Deserialize(byte[] content, int offset,  bool pe32plus, int hintCount)
        {
            var ids = new ImportDataSection();

            ids.ImportDirectoryTable = ImportDirectoryTable.Deserialize(content, offset); offset += 20 * ids.ImportDirectoryTable.Entries.Length;

            List<ImportLookupTable> tempLookupTables = new List<ImportLookupTable>();
            while (true)
            {
                var tempLookupTable = ImportLookupTable.Deserialize(content, offset, pe32plus);
                if (tempLookupTable.EntriesPE32 != null)
                    offset += 4 * tempLookupTable.EntriesPE32.Length;
                else if (tempLookupTable.EntriesPE32Plus != null)
                    offset += 8 * tempLookupTable.EntriesPE32Plus.Length;
                else
                    break;

                tempLookupTables.Add(tempLookupTable);
            }

            // TODO: Update the offset, if possible
            ids.HintNameTable = HintNameTable.Deserialize(content, offset, hintCount);

            return ids;
        }
    }
}
using System;
using System.IO;
using System.Runtime.InteropServices;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ExecutableType.Microsoft
{
    [StructLayout(LayoutKind.Sequential)]
    internal class IMAGE_OPTIONAL_HEADER
    {
        #region Standard Fields

        /// <summary>
        /// The unsigned integer that identifies the state of the image file.
        /// The most common number is 0x10B, which identifies it as a normal executable file.
        /// 0x107 identifies it as a ROM image, and 0x20B identifies it as a PE32+ executable.
        /// </summary>
        public OptionalHeaderType Magic;
        
        /// <summary>
        /// The linker major version number.
        /// </summary>
        public byte MajorLinkerVersion;
        
        /// <summary>
        /// The linker minor version number.
        /// </summary>
        public byte MinorLinkerVersion;
        
        /// <summary>
        /// The size of the code (text) section, or the sum of all code sections if there are multiple sections.
        /// </summary>
        public uint SizeOfCode;
        
        /// <summary>
        /// The size of the initialized data section, or the sum of all such sections if there are multiple data sections.
        /// </summary>
        public uint SizeOfInitializedData;
        
        /// <summary>
        /// The size of the uninitialized data section (BSS), or the sum of all such sections if there are multiple BSS sections.
        /// </summary>
        public uint SizeOfUninitializedData;
        
        /// <summary>
        /// The address of the entry point relative to the image base when the executable file is loaded into memory.
        /// For program images, this is the starting address.
        /// For device drivers, this is the address of the initialization function.
        // An entry point is optional for DLLs.
        /// When no entry point is present, this field must be zero.
        /// </summary>
        public uint AddressOfEntryPoint;
        
        /// <summary>
        /// The address that is relative to the image base of the beginning-of-code section when it is loaded into memory.
        /// </summary>
        public uint BaseOfCode;
        
        /// <summary>
        /// The address that is relative to the image base of the beginning-of-data section when it is loaded into memory.
        /// </summary>
        public uint BaseOfData;

        #endregion

        #region Windows-Specific Fields

        /// <summary>
        /// The preferred address of the first byte of image when loaded into memory; must be a multiple of 64 K.
        /// The default for DLLs is 0x10000000.
        /// The default for Windows CE EXEs is 0x00010000.
        /// The default for Windows NT, Windows 2000, Windows XP, Windows 95, Windows 98, and Windows Me is 0x00400000.
        /// </summary>
        public uint ImageBasePE32;
        
        /// <summary>
        /// The preferred address of the first byte of image when loaded into memory; must be a multiple of 64 K.
        /// The default for DLLs is 0x10000000.
        /// The default for Windows CE EXEs is 0x00010000.
        /// The default for Windows NT, Windows 2000, Windows XP, Windows 95, Windows 98, and Windows Me is 0x00400000.
        /// </summary>
        public ulong ImageBasePE32Plus;
        
        /// <summary>
        /// The alignment (in bytes) of sections when they are loaded into memory.
        /// It must be greater than or equal to FileAlignment.
        /// The default is the page size for the architecture.
        /// </summary>
        public uint SectionAlignment;
        
        /// <summary>
        /// The alignment factor (in bytes) that is used to align the raw data of sections in the image file.
        /// The value should be a power of 2 between 512 and 64 K, inclusive.
        /// The default is 512.
        /// If the SectionAlignment is less than the architecture's page size, then FileAlignment must match SectionAlignment.
        /// </summary>
        public uint FileAlignment;
        
        /// <summary>
        /// The major version number of the required operating system.
        /// </summary>
        public ushort MajorOperatingSystemVersion;
        
        /// <summary>
        /// The minor version number of the required operating system.
        /// </summary>
        public ushort MinorOperatingSystemVersion;
        
        /// <summary>
        /// The major version number of the image.
        /// </summary>
        public ushort MajorImageVersion;
        
        /// <summary>
        /// The minor version number of the image.
        /// </summary>
        public ushort MinorImageVersion;
        
        /// <summary>
        /// The major version number of the subsystem.
        /// </summary>
        public ushort MajorSubsystemVersion;
        
        /// <summary>
        /// The minor version number of the subsystem.
        /// </summary>
        public ushort MinorSubsystemVersion;
        
        /// <summary>
        /// Reserved, must be zero.
        /// </summary>
        public uint Reserved1;
        
        /// <summary>
        /// The size (in bytes) of the image, including all headers, as the image is loaded in memory.
        /// It must be a multiple of SectionAlignment.
        /// </summary>
        public uint SizeOfImage;
        
        /// <summary>
        /// The combined size of an MS-DOS stub, PE header, and section headers rounded up to a multiple of FileAlignment.
        /// </summary>
        public uint SizeOfHeaders;
        
        /// <summary>
        /// The image file checksum.
        /// The algorithm for computing the checksum is incorporated into IMAGHELP.DLL.
        /// The following are checked for validation at load time: all drivers, any DLL loaded at boot time, and any DLL that is loaded into a critical Windows process.
        /// </summary>
        public uint CheckSum;
        
        /// <summary>
        /// The subsystem that is required to run this image.
        /// </summary>
        public WindowsSubsystem Subsystem;
        
        /// <summary>
        /// DLL Characteristics
        /// </summary>
        public DllCharacteristics DllCharacteristics;
        
        /// <summary>
        /// The size of the stack to reserve.
        /// Only SizeOfStackCommit is committed; the rest is made available one page at a time until the reserve size is reached.
        /// </summary>
        public uint SizeOfStackReservePE32;

        /// <summary>
        /// The size of the stack to reserve.
        /// Only SizeOfStackCommit is committed; the rest is made available one page at a time until the reserve size is reached.
        /// </summary>
        public ulong SizeOfStackReservePE32Plus;
        
        /// <summary>
        /// The size of the stack to commit.
        /// </summary>
        public uint SizeOfStackCommitPE32;

        /// <summary>
        /// The size of the stack to commit.
        /// </summary>
        public ulong SizeOfStackCommitPE32Plus;
        
        /// <summary>
        /// The size of the local heap space to reserve.
        /// Only SizeOfHeapCommit is committed; the rest is made available one page at a time until the reserve size is reached.
        /// </summary>
        public uint SizeOfHeapReservePE32;

        /// <summary>
        /// The size of the local heap space to reserve.
        /// Only SizeOfHeapCommit is committed; the rest is made available one page at a time until the reserve size is reached.
        /// </summary>
        public ulong SizeOfHeapReservePE32Plus;
        
        /// <summary>
        /// The size of the local heap space to commit.
        /// </summary>
        public uint SizeOfHeapCommitPE32;

        /// <summary>
        /// The size of the local heap space to commit.
        /// </summary>
        public ulong SizeOfHeapCommitPE32Plus;
        
        /// <summary>
        /// Reserved, must be zero.
        /// </summary>
        public uint LoaderFlags;
        
        /// <summary>
        /// The number of data-directory entries in the remainder of the optional header.
        /// Each describes a location and size.
        /// </summary>
        public uint NumberOfRvaAndSizes;

        /// <summary>
        /// Data-directory entries following the optional header
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.IMAGE_NUMBEROF_DIRECTORY_ENTRIES)]
        public IMAGE_DATA_DIRECTORY[] DataDirectories;

        #endregion

        public static IMAGE_OPTIONAL_HEADER Deserialize(Stream stream)
        {
            var ioh = new IMAGE_OPTIONAL_HEADER();

            ioh.Magic = (OptionalHeaderType)stream.ReadUInt16();
            ioh.MajorLinkerVersion = stream.ReadByteValue();
            ioh.MinorLinkerVersion = stream.ReadByteValue();
            ioh.SizeOfCode = stream.ReadUInt32();
            ioh.SizeOfInitializedData = stream.ReadUInt32();
            ioh.SizeOfUninitializedData = stream.ReadUInt32();
            ioh.AddressOfEntryPoint = stream.ReadUInt32();
            ioh.BaseOfCode = stream.ReadUInt32();

            // Only standard PE32 has this value
            if (ioh.Magic == OptionalHeaderType.PE32)
                ioh.BaseOfData = stream.ReadUInt32();

            // PE32+ has an 8-byte value here
            if (ioh.Magic == OptionalHeaderType.PE32Plus)
                ioh.ImageBasePE32Plus = stream.ReadUInt64();
            else
                ioh.ImageBasePE32 = stream.ReadUInt32();

            ioh.SectionAlignment = stream.ReadUInt32();
            ioh.FileAlignment = stream.ReadUInt32();
            ioh.MajorOperatingSystemVersion = stream.ReadUInt16();
            ioh.MinorOperatingSystemVersion = stream.ReadUInt16();
            ioh.MajorImageVersion = stream.ReadUInt16();
            ioh.MinorImageVersion = stream.ReadUInt16();
            ioh.MajorSubsystemVersion = stream.ReadUInt16();
            ioh.MinorSubsystemVersion = stream.ReadUInt16();
            ioh.Reserved1 = stream.ReadUInt32();
            ioh.SizeOfImage = stream.ReadUInt32();
            ioh.SizeOfHeaders = stream.ReadUInt32();
            ioh.CheckSum = stream.ReadUInt32();
            ioh.Subsystem = (WindowsSubsystem)stream.ReadUInt16();
            ioh.DllCharacteristics = (DllCharacteristics)stream.ReadUInt16();

            // PE32+ uses 8-byte values
            if (ioh.Magic == OptionalHeaderType.PE32Plus)
            {
                ioh.SizeOfStackReservePE32Plus = stream.ReadUInt64();
                ioh.SizeOfStackCommitPE32Plus = stream.ReadUInt64();
                ioh.SizeOfHeapReservePE32Plus = stream.ReadUInt64();
                ioh.SizeOfHeapCommitPE32Plus = stream.ReadUInt64();
            }
            else
            {
                ioh.SizeOfStackReservePE32 = stream.ReadUInt32();
                ioh.SizeOfStackCommitPE32 = stream.ReadUInt32();
                ioh.SizeOfHeapReservePE32 = stream.ReadUInt32();
                ioh.SizeOfHeapCommitPE32 = stream.ReadUInt32();
            }

            ioh.LoaderFlags = stream.ReadUInt32();
            ioh.NumberOfRvaAndSizes = stream.ReadUInt32();
            ioh.DataDirectories = new IMAGE_DATA_DIRECTORY[Constants.IMAGE_NUMBEROF_DIRECTORY_ENTRIES];
            for (int i = 0; i < Constants.IMAGE_NUMBEROF_DIRECTORY_ENTRIES; i++)
            {
                ioh.DataDirectories[i] = IMAGE_DATA_DIRECTORY.Deserialize(stream);
            }

            return ioh;
        }

        public static IMAGE_OPTIONAL_HEADER Deserialize(byte[] content, int offset)
        {
            var ioh = new IMAGE_OPTIONAL_HEADER();

            ioh.Magic = (OptionalHeaderType)BitConverter.ToUInt16(content, offset); offset += 2;
            ioh.MajorLinkerVersion = content[offset]; offset++;
            ioh.MinorLinkerVersion = content[offset]; offset++;
            ioh.SizeOfCode = BitConverter.ToUInt32(content, offset); offset += 4;
            ioh.SizeOfInitializedData = BitConverter.ToUInt32(content, offset); offset += 4;
            ioh.SizeOfUninitializedData = BitConverter.ToUInt32(content, offset); offset += 4;
            ioh.AddressOfEntryPoint = BitConverter.ToUInt32(content, offset); offset += 4;
            ioh.BaseOfCode = BitConverter.ToUInt32(content, offset); offset += 4;

            // Only standard PE32 has this value
            if (ioh.Magic == OptionalHeaderType.PE32)
                ioh.BaseOfData = BitConverter.ToUInt32(content, offset); offset += 4;

            // PE32+ has an 8-bit value here
            if (ioh.Magic == OptionalHeaderType.PE32Plus)
            {
                ioh.ImageBasePE32Plus = BitConverter.ToUInt64(content, offset); offset += 8;
            }
            else
            {
                ioh.ImageBasePE32 = BitConverter.ToUInt32(content, offset); offset += 4;
            }
            
            ioh.SectionAlignment = BitConverter.ToUInt32(content, offset); offset += 4;
            ioh.FileAlignment = BitConverter.ToUInt32(content, offset); offset += 4;
            ioh.MajorOperatingSystemVersion = BitConverter.ToUInt16(content, offset); offset += 2;
            ioh.MinorOperatingSystemVersion = BitConverter.ToUInt16(content, offset); offset += 2;
            ioh.MajorImageVersion = BitConverter.ToUInt16(content, offset); offset += 2;
            ioh.MinorImageVersion = BitConverter.ToUInt16(content, offset); offset += 2;
            ioh.MajorSubsystemVersion = BitConverter.ToUInt16(content, offset); offset += 2;
            ioh.MinorSubsystemVersion = BitConverter.ToUInt16(content, offset); offset += 2;
            ioh.Reserved1 = BitConverter.ToUInt32(content, offset); offset += 4;
            ioh.SizeOfImage = BitConverter.ToUInt32(content, offset); offset += 4;
            ioh.SizeOfHeaders = BitConverter.ToUInt32(content, offset); offset += 4;
            ioh.CheckSum = BitConverter.ToUInt32(content, offset); offset += 4;
            ioh.Subsystem = (WindowsSubsystem)BitConverter.ToUInt16(content, offset); offset += 2;
            ioh.DllCharacteristics = (DllCharacteristics)BitConverter.ToUInt16(content, offset); offset += 2;

            // PE32+ uses 8-byte values
            if (ioh.Magic == OptionalHeaderType.PE32Plus)
            {
                ioh.SizeOfStackReservePE32Plus = BitConverter.ToUInt64(content, offset); offset += 8;
                ioh.SizeOfStackCommitPE32Plus = BitConverter.ToUInt64(content, offset); offset += 8;
                ioh.SizeOfHeapReservePE32Plus = BitConverter.ToUInt64(content, offset); offset += 8;
                ioh.SizeOfHeapCommitPE32Plus = BitConverter.ToUInt64(content, offset); offset += 8;
            }
            else
            {
                ioh.SizeOfStackReservePE32 = BitConverter.ToUInt32(content, offset); offset += 4;
                ioh.SizeOfStackCommitPE32 = BitConverter.ToUInt32(content, offset); offset += 4;
                ioh.SizeOfHeapReservePE32 = BitConverter.ToUInt32(content, offset); offset += 4;
                ioh.SizeOfHeapCommitPE32 = BitConverter.ToUInt32(content, offset); offset += 4;
            }

            ioh.LoaderFlags = BitConverter.ToUInt32(content, offset); offset += 4;
            ioh.NumberOfRvaAndSizes = BitConverter.ToUInt32(content, offset); offset += 4;
            ioh.DataDirectories = new IMAGE_DATA_DIRECTORY[Constants.IMAGE_NUMBEROF_DIRECTORY_ENTRIES];
            for (int i = 0; i < Constants.IMAGE_NUMBEROF_DIRECTORY_ENTRIES; i++)
            {
                ioh.DataDirectories[i] = IMAGE_DATA_DIRECTORY.Deserialize(content, offset); offset += 8;
            }

            return ioh;
        }
    }
}
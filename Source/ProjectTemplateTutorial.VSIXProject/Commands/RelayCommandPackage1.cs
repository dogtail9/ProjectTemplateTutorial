namespace ProjectTemplateTutorial.VSIXProject.Commands
{
    using System;
    
    /// <summary>
    /// Helper class that exposes all GUIDs used across VS Package.
    /// </summary>
    internal sealed partial class PackageGuids
    {
        public const string guidRelayCommandPackageString = "edc30286-8947-4257-9355-8d5d25829c5d";
        public const string guidRelayCommandPackageCmdSetString = "977a44b1-3da7-4b57-9e13-253a15116874";
        public const string guidImagesString = "9a4ae56f-11f2-443e-8533-e1c6a67b471d";
        public static Guid guidRelayCommandPackage = new Guid(guidRelayCommandPackageString);
        public static Guid guidRelayCommandPackageCmdSet = new Guid(guidRelayCommandPackageCmdSetString);
        public static Guid guidImages = new Guid(guidImagesString);
    }
    /// <summary>
    /// Helper class that encapsulates all CommandIDs uses across VS Package.
    /// </summary>
    internal sealed partial class PackageIds
    {
        public const int ProjectContextGroup = 0x0100;
        public const int ProjectContextMenu = 0x0200;
        public const int ProjectContextMenuGroup = 0x0300;
        public const int AddCopyrightCommand = 0x0400;
        public const int bmpPic1 = 0x0001;
        public const int bmpPic2 = 0x0002;
        public const int bmpPicSearch = 0x0003;
        public const int bmpPicX = 0x0004;
        public const int bmpPicArrows = 0x0005;
        public const int bmpPicStrikethrough = 0x0006;
    }
}

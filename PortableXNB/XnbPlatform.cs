namespace PortableXNB;

public enum XnbPlatform : byte
{
    WindowsDirectX = (byte)'w',
    Xbox360 = (byte)'x',
    iOS = (byte)'i',
    Android = (byte)'a', 
    DesktopGL = (byte)'d', 
    MacOSX = (byte)'X', 
    NativeClient = (byte)'n', 
    RaspberryPi = (byte)'r', 
    PlayStation4 = (byte)'P', 
    PlayStation5 = (byte)'5', 
    XboxOne = (byte)'O', 
    NintendoSwitch = (byte)'S',
    WebAssembly = (byte)'b',
    DesktopVK = (byte)'V', 
    WindowsGDK = (byte)'G',
    XboxSeries = (byte)'s', 
    
    // Legacy IDs
    WindowsStoreApp = (byte)'W', 
    WindowsPhone8 = (byte)'M', 
    WindowsPhone7_0 = (byte)'m',
    PlayStationMobile = (byte)'p', 
    PSVita = (byte)'v', 
    WindowsGL = (byte)'g',
    Linux = (byte)'l', 
}
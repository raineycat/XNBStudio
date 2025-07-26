namespace PortableXNB.TypeReaders;

public interface ITypeReader
{
    string Name { get; }
    string DefaultFileExtension { get; }

    bool MatchType(string type);
    object LoadAsset(XnbFile file);
    TAsset LoadAsset<TAsset>(XnbFile file) => (TAsset)LoadAsset(file);
}
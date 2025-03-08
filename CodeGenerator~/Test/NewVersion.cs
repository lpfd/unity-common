using Leap.Forward.JsonVersioning;

[JsonVersion(PreviousVersion =typeof(OldVersion), Version = 1)]
public partial class NewVersion
{
    public NewVersion()
    {
    }
    public NewVersion(OldVersion oldVersion)
    {
    }
}
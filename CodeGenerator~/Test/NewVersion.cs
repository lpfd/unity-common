using Leap.Forward.JsonVersioning;

[JsonVersion(PreviousVersion =typeof(OldVersion), Version = 1)]
public partial class NewVersion
{
    public NewVersion()
    {
    }
    private void UpgradeFrom(OldVersion oldVersion)
    {
    }
}
namespace Leap.Forward.SaveFiles
{
    public interface ISaveManager
    {
        bool HasSaveAtSlot(int slotIndex);
        void LoadGame(int slotIndex);
        void SaveGame(int slotIndex);
    }
}

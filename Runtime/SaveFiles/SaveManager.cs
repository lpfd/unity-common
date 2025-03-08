using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

namespace Leap.Forward.SaveFiles
{
    public class SaveManager: ISaveManager
    {
        private ISceneLoader _sceneLoader;
        private IGameStateMachine _stateMachine;

        public SaveManager(ISceneLoader sceneLoader, IGameStateMachine stateMachine)
        {
            _sceneLoader = sceneLoader;
            _stateMachine = stateMachine;
        }

        public bool HasSaveAtSlot(int slotIndex)
        {
            return File.Exists(GetFileName(slotIndex));
        }

        private string GetFileName(int slotIndex)
        {
            return Path.Combine(EnsureSavePath(), $"Save{slotIndex}.save");
        }

        public void LoadGame(int slotIndex)
        {
            var saveFileContent = JsonConvert.DeserializeObject<SaveFileContent>(File.ReadAllText(GetFileName(slotIndex)));
            var stateType = Type.GetType(saveFileContent.StateName);
            if (stateType == null)
            {
                foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
                {
                    stateType = a.GetType(saveFileContent.StateName);
                    if (stateType != null)
                        break;
                }
            }
            _stateMachine.Enter(stateType, saveFileContent);
        }

        public void SaveGame(int slotIndex)
        {
            var content = new SaveFileContent();
            content.SceneName = _sceneLoader.CurrentSceneName;
            content.StateName = _stateMachine.ActiveState?.GetType()?.FullName;

            File.WriteAllText(GetFileName(slotIndex), JsonConvert.SerializeObject(content));
        }

        private string EnsureSavePath()
        {
            var path = Path.Combine(Application.persistentDataPath, "Saves");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

    }
}

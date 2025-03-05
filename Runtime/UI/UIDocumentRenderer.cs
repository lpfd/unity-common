using UnityEngine;
using UnityEngine.UIElements;

namespace Leap.Forward.UI
{
    [RequireComponent(typeof(UIDocument))]
    [RequireComponent(typeof(MeshRenderer))]
    public class UIDocumentRenderer : MonoBehaviour
    {
        public Vector2Int _panelSize = new Vector2Int(512, 512);
        public float _pixelsPerUnit = 512.0f;
        private RenderTexture renderTexture;
        private PanelSettings panelSettingsInstance;

        void Awake()
        {
            // Get components
            var uiDocument = GetComponent<UIDocument>();
            var meshRenderer = GetComponent<MeshRenderer>();

            // Setup mesh renderer
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            meshRenderer.receiveShadows = false;
            meshRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
            meshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            meshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

            var descriptor = uiDocument.panelSettings.targetTexture.descriptor;
            descriptor.width = _panelSize.x;
            descriptor.height = _panelSize.y;

            // Create a new RenderTexture
            renderTexture = new RenderTexture(descriptor)
            {
                name = $"{name} RenderTexture"
            };
            renderTexture.Create();

            // Assign the RenderTexture to the MeshRenderer's material
            var shaderName = uiDocument.panelSettings.colorClearValue.a < 1.0f ? "Unlit/Transparent" : "Unlit/Texture";
            var material = new Material(Shader.Find(shaderName));
            material.mainTexture = renderTexture;
            meshRenderer.sharedMaterial = material;

            // Duplicate the assigned PanelSettings
            panelSettingsInstance = Instantiate(uiDocument.panelSettings);

            // Assign the RenderTexture to the duplicated PanelSettings
            panelSettingsInstance.targetTexture = renderTexture;

            // Assign the duplicated PanelSettings to the UIDocument
            uiDocument.panelSettings = panelSettingsInstance;

            transform.localScale = new Vector3(_panelSize.x / _pixelsPerUnit, _panelSize.y / _pixelsPerUnit, 1.0f);
        }

        void OnDestroy()
        {
            // Clean up
            if (renderTexture != null)
            {
                renderTexture.Release();
                Destroy(renderTexture);
            }
            if (panelSettingsInstance != null)
            {
                Destroy(panelSettingsInstance);
            }
        }
    }
}
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityIoc.Runtime;
using UnityIoc.Runtime.Types;

namespace _PackageRoot.Editor.Scripts
{
    [CustomEditor(typeof(IocConfig))]
    public class IocConfigEditor : UnityEditor.Editor
    {

        // public override void OnInspectorGUI() {
        //     DrawDefaultInspector();
        // }

        public override VisualElement CreateInspectorGUI() {
            var container = new VisualElement();

            InspectorElement.FillDefaultInspector(container, this.serializedObject, this);
            
            var addBootstrapperButton = new Button(() =>
            {
                var derivedTypes = TypeCache.GetTypesDerivedFrom<Bootstrapper>();
                
                var selectionList = new VisualElement();
                var wrapper = new Box() {
                    style = {
                        paddingTop    = 6,
                        paddingLeft   = 6,
                        paddingRight  = 6,
                        paddingBottom = 6
                    }
                };
                wrapper.Add(new Label("Select your bootstrapper:") {
                    style = {
                        marginTop    = 10,
                        marginBottom = 10
                    }
                });

                wrapper.Add(selectionList);


                foreach (var derivedType in derivedTypes) {
                    var button = new Button(() =>
                    {
                        IocConfig.Instance.IocBootstrapperType = new TypeReference {Type = derivedType};
                        EditorUtility.SetDirty(IocConfig.Instance);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                        
                        wrapper.RemoveFromHierarchy();
                        
                        Debug.Log($"Set IocConfig.Instance.IocBootstrapperType to {derivedType.Name}");
                    }) {
                        text = $"{derivedType.Name}( {derivedType.Namespace} )"
                    };

                    selectionList.Add(button);
                }

                container.Add(wrapper);
            }) {
                text = "Add Bootstrapper",
            };

            container.Add(addBootstrapperButton);

            return container;
        }
    }

}
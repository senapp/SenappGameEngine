using ImGuiNET;
using OpenTK;
using Senapp.Engine.Base;
using Senapp.Engine.Entities;
using Senapp.Engine.Events;
using Senapp.Engine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Senapp.Engine.ImGUI
{
    public class EditorWindow
    {
        public static bool enabled = false;
        private static ImGuiController _controller;
        public static void Init(GameWindow gw)
        {
            _controller = new ImGuiController(gw.Width, gw.Height);
        }
        public static void Render(GameWindow gw, GameRenderedEventArgs e)
        {
            if (!enabled) return;

            _controller.Update(gw, (float)e.DeltaTime);

            onDrawGUI();

            _controller.Render();
        }
        private static void onDrawGUI()
        {
            if (ImGui.Begin("Insprector"))
            {
                if (ImGui.TreeNode("Objects"))
                {
                    for (int i = 0; i < GameObject.GameObjects.Count; i++)
                    {
                        var obj = GameObject.GameObjects[i];
                        if (!obj.excludeFromEditor)
                        {
                            ImGui.PushID(obj.id);
                            if (ImGui.TreeNode("GameObject"))
                            {
                                ImGui.InputText("Name", ref obj.name, 32);
                                ImGui.Checkbox("Enabled", ref obj.enabled);
                                if (ImGui.TreeNode("Transform"))
                                {
                                    ImGui.Text("Position");

                                    Vector3 tmp = obj.transform.position;
                                    System.Numerics.Vector3 v = new System.Numerics.Vector3(tmp.X, tmp.Y, tmp.Z);

                                    ImGui.DragFloat3("Position", ref v, 0.1f);
                                    obj.transform.position = new Vector3(v.X, v.Y, v.Z);

                                    ImGui.Text("Rotation");

                                    tmp = obj.transform.rotation;
                                    v = new System.Numerics.Vector3(tmp.X, tmp.Y, tmp.Z);

                                    ImGui.DragFloat3("Rotation", ref v, 0.1f);
                                    obj.transform.rotation = new Vector3(v.X, v.Y, v.Z);

                                    ImGui.Text("Scale");

                                    tmp = obj.transform.localScale;
                                    v = new System.Numerics.Vector3(tmp.X, tmp.Y, tmp.Z);

                                    ImGui.DragFloat3("Scale", ref v, 0.1f);
                                    obj.transform.localScale = new Vector3(v.X, v.Y, v.Z);

                                    ImGui.TreePop();
                                }
                                var components = obj.componentManager.GetComponents();
                                for (int c = 0; c < components.Count; c++)
                                {
                                    var component = obj.componentManager.GetComponents().ElementAt(c);
                                    if (ImGui.TreeNode(component.Key.ToString()))
                                    {
                                        BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
                                        foreach (var field in component.Key.GetFields(bindingFlags))
                                        {
                                            string value = "";
                                            value = field.GetValue(component.Value).ToString();
                                            string name = field.Name;
                                            if (name.Contains("<"))
                                            {
                                                name = name.Remove(0, name.IndexOf("<") + 1);
                                                name = name.Remove(name.IndexOf(">"));
                                            }
                                            char ch = name[0];
                                            if (!char.IsUpper(ch))
                                            {
                                                name = name.Remove(0, 1);
                                                name = name.Insert(0, char.ToUpper(ch).ToString());
                                            }
                                            ImGui.SetNextItemWidth(300f);
                                            ImGui.LabelText(value, Regex.Replace(name, "([a-z])([A-Z])", "$1 $2"));
                                        }
                                        ImGui.TreePop();
                                    }
                                }
                                ImGui.TreePop();
                            }
                            ImGui.PopID();
                        }
                    }
                    ImGui.TreePop();
                }
                if (ImGui.TreeNode("Entities And Lights"))
                {
                    for (int i = 0; i < GameObject.GameObjects.Count; i++)
                    {
                        var obj = GameObject.GameObjects[i];
                        if (!obj.excludeFromEditor)
                        {
                            if (obj.HasComponent<Entity>() || obj.HasComponent<Light>())
                            {
                                ImGui.PushID(obj.id);
                                string name = "";
                                if (obj.HasComponent<Light>()) name = "Light";
                                else
                                {
                                    name = obj.GetComponent<Entity>().model.texture.Name;
                                }

                                if (ImGui.TreeNode(name))
                                {
                                    if (ImGui.TreeNode("Transform"))
                                    {
                                        ImGui.Text("Position");

                                        Vector3 tmp = obj.transform.position;
                                        System.Numerics.Vector3 v = new System.Numerics.Vector3(tmp.X, tmp.Y, tmp.Z);

                                        ImGui.DragFloat3("Position", ref v, 0.1f);
                                        obj.transform.position = new Vector3(v.X, v.Y, v.Z);

                                        ImGui.Text("Rotation");

                                        tmp = obj.transform.rotation;
                                        v = new System.Numerics.Vector3(tmp.X, tmp.Y, tmp.Z);

                                        ImGui.DragFloat3("Rotation", ref v, 0.1f);
                                        obj.transform.rotation = new Vector3(v.X, v.Y, v.Z);

                                        ImGui.Text("Scale");

                                        tmp = obj.transform.localScale;
                                        v = new System.Numerics.Vector3(tmp.X, tmp.Y, tmp.Z);

                                        ImGui.DragFloat3("Scale", ref v, 0.1f);
                                        obj.transform.localScale = new Vector3(v.X, v.Y, v.Z);

                                        ImGui.TreePop();
                                    }
                                    if (obj.HasComponent<Light>())
                                    {
                                        if (ImGui.TreeNode("Light"))
                                        {
                                            var light = obj.GetComponent<Light>();

                                            ImGui.Text("Colour");

                                            Vector3 tmp = light.colour;
                                            System.Numerics.Vector3 v = new System.Numerics.Vector3(tmp.X, tmp.Y, tmp.Z);

                                            ImGui.DragFloat3("Colour", ref v, 0.1f);
                                            light.colour = new Vector3(v.X, v.Y, v.Z);
                                        }
                                    }
                                    if (obj.HasComponent<Entity>())
                                    {
                                        if (ImGui.TreeNode("Entity"))
                                        {
                                            var entityTexture = obj.GetComponent<Entity>().model;

                                            bool hasTransparency = entityTexture.hasTransparency;
                                            ImGui.Checkbox("Has Transparency", ref hasTransparency);
                                            entityTexture.hasTransparency = hasTransparency;

                                            bool fakeLighting = entityTexture.useFakeLighting;
                                            ImGui.Checkbox("Use Fake Lighting", ref fakeLighting);
                                            entityTexture.useFakeLighting = fakeLighting;

                                            float shineDamper = entityTexture.shineDamper;
                                            ImGui.DragFloat("Shine damper", ref shineDamper, 0.1f);
                                            entityTexture.shineDamper = shineDamper;

                                            float reflectivity = entityTexture.reflectivity;
                                            ImGui.DragFloat("Reflectivity", ref reflectivity, 0.1f);
                                            entityTexture.reflectivity = reflectivity;

                                            float luminosity = entityTexture.luminosity;
                                            ImGui.DragFloat("Luminosity", ref luminosity, 0.1f);
                                            entityTexture.luminosity = luminosity;
                                        }
                                    }
                                    ImGui.TreePop();
                                }
                                ImGui.PopID();
                            }                
                        }
                    }
                    ImGui.TreePop();
                }
                ImGui.End();
            }
        }
        public static void OnKeyPress(KeyPressEventArgs e)
        {
            if (!enabled) return;
            _controller.PressChar(e.KeyChar);
        }
        public static void OnResize(GameWindow gw)
        {
            _controller.WindowResized(gw.Width, gw.Height);
        }
    }
}

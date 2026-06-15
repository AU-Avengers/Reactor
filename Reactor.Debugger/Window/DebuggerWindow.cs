using System;
using AmongUs.Data;
using Reactor.Debugger.Window.Tabs;
using Reactor.Utilities.ImGui;
using UnityEngine;

namespace Reactor.Debugger.Window;

internal sealed class DebuggerWindow : MonoBehaviour
{
    private DragWindow _window = null!;

    
    public BaseTab[] Tabs { get; } =
    {
        new ConfigTab(),
        new GameTab(),
        new AutoJoinTab(),
    };

    
    public BaseTab SelectedTab { get; private set; } = null!;

    private void Awake()
    {
        SelectedTab = Tabs[0];

        _window = new DragWindow(new Rect(20, 20, 0, 0), "Reactor.Debugger", () =>
        {
            var clientName = DataManager.Player.Customization.Name;
            if (AmongUsClient.Instance && AmongUsClient.Instance.AmHost) clientName += " (host)";
            GUILayout.Label("Name: " + clientName);

            if (GUILayout.Button("Hard crash"))
            {
                Environment.FailFast("Hard crash requested from Reactor.Debugger");
            }

            GUILayout.BeginHorizontal();
            {
                foreach (var tab in Tabs)
                {
                    if (GUILayout.Toggle(tab == SelectedTab, tab.Name, new GUIStyle(GUI.skin.button)))
                    {
                        SelectedTab = tab;
                    }
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(5f);

            SelectedTab.OnGUI();
        })
        {
            Enabled = false,
        };
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            _window.Enabled = !_window.Enabled;
        }
    }

    public void OnGUI()
    {
        _window.OnGUI();
    }
}

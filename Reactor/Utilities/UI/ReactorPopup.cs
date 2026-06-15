using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Reactor.Utilities.UI;

/// <summary>
/// Wrapper over <see cref="GenericPopup"/> that adds hyperlink and controller support.
/// </summary>
internal sealed class ReactorPopup : MonoBehaviour
{
    private readonly List<SelectableHyperLink> _selectableHyperLinks = new();

    public GenericPopup Popup { get; private set; } = null!;
    public TextMeshPro TextArea { get; private set; } = null!;
    public PassiveButton BackButton { get; private set; } = null!;
    public SpriteRenderer Background { get; private set; } = null!;

    
    public void Show(string text)
    {
        Popup.Show(text);

        ControllerManager.Instance.OpenOverlayMenu(name, BackButton);

        SelectableHyperLinkHelper.AddSelectableUiForHyperlinks(_selectableHyperLinks, name);
        TextArea.text = SelectableHyperLinkHelper.DecomposeAnnouncementText(TextArea, _selectableHyperLinks, name, TextArea.text);
        SelectableHyperLinkHelper.UpdateHyperlinkPositions(TextArea, _selectableHyperLinks, name);

        ControllerManager.Instance.AddSelectableUiElement(BackButton, true);
    }

    public void OnDisable()
    {
        ControllerManager.Instance.CloseOverlayMenu(name);
    }

    
    public static ReactorPopup Create(string name)
    {
        var genericPopup = Instantiate(DiscordManager.Instance.discordPopup, Camera.main!.transform);
        var gameObject = genericPopup.gameObject;
        var reactorPopup = gameObject.AddComponent<ReactorPopup>();

        reactorPopup.Popup = genericPopup;
        reactorPopup.TextArea = genericPopup.TextAreaTMP;
        reactorPopup.BackButton = gameObject.transform.Find("ExitGame").GetComponent<PassiveButton>();
        reactorPopup.Background = gameObject.transform.Find("Background").GetComponent<SpriteRenderer>();

        gameObject.name = name;
        genericPopup.destroyOnClose = true;
        reactorPopup.TextArea.fontSizeMin = 2;

        return reactorPopup;
    }
}

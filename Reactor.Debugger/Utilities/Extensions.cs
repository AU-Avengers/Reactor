using System;
using System.Collections;
using UnityInterop.Runtime;
using InnerNet;

namespace Reactor.Debugger.Utilities;

internal static class Extensions
{
    public static IEnumerator CoCreateLocalGame(this AmongUsClient client, bool isFreePlay = false)
    {
        try
        {
            if (isFreePlay)
            {
                client.NetworkMode = NetworkModes.FreePlay;

                InnerNetServer.Instance.StartAsLocalServer();

                client.MainMenuScene = "MainMenu";
                client.OnlineScene = "Tutorial";
            }
            else
            {
                client.NetworkMode = NetworkModes.LocalGame;

                InnerNetServer.Instance.StartAsServer();

                client.MainMenuScene = "MatchMaking";
                client.OnlineScene = "OnlineGame";
            }

            client.SetEndpoint(Constants.LocalNetAddress, Constants.GamePlayPort, false);
        }
        catch (UnityException e)
        {
            Error(e);
            DisconnectPopup.Instance.ShowCustom(e.Message[..e.Message.IndexOf("\n--- BEGIN Unity STACK TRACE ---\n", StringComparison.Ordinal)]);
            MatchMaker.Instance.NotConnecting();
            yield break;
        }

        client.Connect(MatchMakerModes.HostAndClient, null);

        yield return client.WaitForConnectionOrFail();

        DestroyableSingleton<MatchMaker>.Instance.NotConnecting();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.RemoteConfig;
using System;

public class BotDifficultyManager : MonoBehaviour
{
    [SerializeField] Bot bot;
    [SerializeField] int selectedDifficulty;
    [SerializeField] BotStats[] botDifficulties;

    [Header("Remote Config Parameters")]
    [SerializeField] bool enableRemotConfig = false;
    
    [SerializeField] string difficultyKey = "Difficulty";
    struct userAttribute{};
    struct appAtributes{};
    IEnumerator Start()
    {
        // tunggu bot setup
        yield return new WaitUntil(()=>bot.IsReady);

        
        var newStats = botDifficulties[selectedDifficulty];
        bot.SetStats(newStats);

        if(enableRemotConfig == false)
            yield break;
        
        yield return new WaitUntil(
            ()=>
                UnityServices.State == ServicesInitializationState.Initialized
                &&
                AuthenticationService.Instance.IsSignedIn
        );

        RemoteConfigService.Instance.FetchCompleted += onRemoteConfigFetched;
        
        RemoteConfigService.Instance.FetchConfigs(
            new userAttribute(),new appAtributes()
        );
    }

    private void onRemoteConfigFetched(ConfigResponse response)
    {
        if(RemoteConfigService.Instance.appConfig.HasKey("Difficulty") == false)
        {    
            Debug.LogWarning($"Diffculty Key: {difficultyKey} not found on remote config server");
            return;
        }
            switch (response.requestOrigin)
            {
                case ConfigOrigin.Default:
                case ConfigOrigin.Cached:
                    break;
                case ConfigOrigin.Remote:
                    selectedDifficulty = RemoteConfigService.Instance.appConfig.GetInt(difficultyKey);
                    selectedDifficulty = Mathf.Clamp(selectedDifficulty, 0, botDifficulties.Length - 1);
                    var newStats = botDifficulties[selectedDifficulty];
                    bot.SetStats(newStats,true);    
                    break;
            }
    }
}

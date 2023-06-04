using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class ScenarioManager : MonoBehaviour
{
    private StoryModel story;
    private List<AudioClip> audioClips;

    [SerializeField]
    private AudioSource dialogueSource;

    [SerializeField]
    private TextMeshProUGUI subtitles;

    [SerializeField]
    private TextMeshProUGUI requestor;

    private int scenarioProgress;

    [SerializeField] private CharacterBehaviour spongebob;
    [SerializeField] private CharacterBehaviour patrick;
    [SerializeField] private CharacterBehaviour squidward;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("LoadStory", 2f);
    }

    void LoadStory()
    {
        StartCoroutine(GetStory());
    }


    void PlayScenario()
    {
        if (scenarioProgress < audioClips.Count)
        {
            PlayScene();
        }
        else
        {
            subtitles.text = "";
            requestor.text = "Waiting For Story";
            Invoke("LoadStory", 2f);
        }
    }

    void PlayScene()
    {
        dialogueSource.clip = audioClips[scenarioProgress];
        dialogueSource.Play();

        subtitles.text = story.scenario[scenarioProgress].character + ": " + story.scenario[scenarioProgress].text;

        switch (story.scenario[scenarioProgress].character)
        {
            //TODO change animations and states of characters here
        }


        scenarioProgress++;
        Invoke("PlayScenario", audioClips[scenarioProgress - 1].length + 0.5f);
    }


    // Update is called once per frame
    IEnumerator GetStory()
    {
        audioClips = new List<AudioClip>();

        using (UnityWebRequest webRequest = UnityWebRequest.Get("http://localhost:3001/story/getScenario"))
        {
            yield return webRequest.SendWebRequest();

            story = JsonUtility.FromJson<StoryModel>(webRequest.downloadHandler.text);

            //No story found
            if (story == null)
            {
                Invoke("LoadStory", 2f);
                yield break;
            }

            //Initialize entire list with nulls so you can substitute at the right place as they come back asynchronously.
            for (int i = 0; i < story.scenario.Count; i++)
            {
                audioClips.Add(null);
            }

            for (int i = 0; i < story.scenario.Count; i++)
            {
                StartCoroutine(GetAudioClip(story.scenario[i].sound, i));
            }

            Invoke("CheckForSounds", 3f);
        }
    }

    IEnumerator GetAudioClip(string clipUrl, int pos)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(clipUrl, AudioType.WAV))
        {
            yield return www.SendWebRequest();
            audioClips[pos] = DownloadHandlerAudioClip.GetContent(www);
        }
    }

    void CheckForSounds()
    {
        for (int i = 0; i < audioClips.Count; i++)
        {
            if (audioClips[i] == null)
            {
                Invoke("CheckForSounds", 3f);
                return;
            }
        }

        scenarioProgress = 0;
        requestor.text = "Requested By: " + story.requestor_id;
        PlayScenario();
    }
}

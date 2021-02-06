using OsuParsers.Beatmaps;
using OsuParsers.Decoders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class SongManager : MonoBehaviour
{
    public static List<Song> Songs { get; set; }
    private static IEnumerable<string> audioPaths;
    private static bool isScanningSongDirectory = false;
    private static SongManager instance;

    private void Start()
    {
        Songs = new List<Song>
        {
            Audio.Instance.Triangles
        };
    }

    public static IEnumerator ScanSongDirectory()
    {
        Debug.Log("Scanning song directory");
        if (!isScanningSongDirectory)
        {
            isScanningSongDirectory = true;
        }
        else
        {
            yield break;
        }
        Songs.Clear();
        try
        {
            audioPaths = Directory.EnumerateFiles(PlayerData.PersistentPlayerData.BeatmapLocation, "*.mp3", SearchOption.AllDirectories);
        }
        catch (ArgumentException e)
        {
            Debug.Log(e.Message);
        }
        foreach (string path in audioPaths ?? Enumerable.Empty<string>())
        {
            try
            {
                Song song = new Song(Audio.Mp3ToAudioClip(File.ReadAllBytes(path)));
                //song.Background.LoadImage(File.ReadAllBytes(backgroundPath));
                song.MetadataSection.Title = path.Substring(path.LastIndexOf('\\') + 1).Replace(".mp3", "");
                Songs.Add(song);
            }
            catch (FileNotFoundException e)
            {
                Debug.Log(e.Message);
            }
            Debug.Log(path);
            yield return null;
        }
        isScanningSongDirectory = false;
        SongList.Instance.UpdateSongList();
    }

    public static List<Song> GetUniqueSongList()
    {
        List<Song> result = new List<Song>();
        for (int i = 0; i < Songs.Count; i++)
        {
            if (!result.Exists((s) => s.MetadataSection.Title == Songs[i].MetadataSection.Title))
            {
                result.Add(Songs[i]);
            }
        }
        return result;
    }

    private SongManager()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
}
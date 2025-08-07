using UnityEngine;

[CreateAssetMenu(fileName = "New Chapter", menuName = "Journey/Chapter Info")]
public class ChapterInfo : ScriptableObject
{
    public string chapterId; 
    public string chapterNumberText; 
    public string chapterTitleText; 
    public string sceneToLoad;
}
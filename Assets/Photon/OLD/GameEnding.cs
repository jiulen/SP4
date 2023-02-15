//using UnityEngine;
//using UnityEngine.SceneManagement;
//using Photon.Pun;

//public class GameEnding : MonoBehaviour
//{
//    public float fadeDuration = 1f;
//    public float displayImageDuration = 1f;
//    public CanvasGroup winImageCanvasGroup;
//    public AudioSource winAudio;
//    public CanvasGroup loseImageCanvasGroup;
//    public AudioSource loseAudio;

//    bool win;
//    bool lose;
//    float m_Timer;
//    bool m_HasAudioPlayed;

//    public void ActivateWin()
//    {
//        lose = false;
//        win = true;
//    }
//    public void ActivateLose()
//    {
//        lose = true;
//        win = false;
//    }

//    void Update ()
//    {
//        if (win)
//        {
//            EndLevel (winImageCanvasGroup, winAudio);
//        }
//        else if (lose)
//        {
//            EndLevel (loseImageCanvasGroup, loseAudio);
//        }
//    }

//    void EndLevel (CanvasGroup imageCanvasGroup, AudioSource audioSource)
//    {
//        if (!m_HasAudioPlayed)
//        {
//            audioSource.Play();
//            m_HasAudioPlayed = true;
//        }
            
//        m_Timer += Time.deltaTime;
//        imageCanvasGroup.alpha = m_Timer / fadeDuration;
//        Debug.Log(imageCanvasGroup.alpha);
//        if (m_Timer > fadeDuration + displayImageDuration)
//        {
//            //if (doRestart)
//            //{
//            //    SceneManager.LoadScene (0);
//            //}
//            //else
//            //{
//            //    Application.Quit ();
//            //}
//        }
//    }
//}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class Play : MonoBehaviour
{
         // class doesn't matter, add to anything in the Editor folder
     // for any beginners reading, this is c#
     
     [MenuItem("Edit/Play-Stop, But From Prelaunch Scene %0")]
     public static void PlayFromPrelaunchScene()
         {
          if ( EditorApplication.isPlaying == true )
             {
             EditorApplication.isPlaying = false;
             return;
             }
         
         EditorApplication.SaveCurrentSceneIfUserWantsTo();
         EditorApplication.OpenScene("Assets/Scenes/auth.unity");
         EditorApplication.isPlaying = true;
         }
}

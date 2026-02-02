using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Tutorials.Core.Editor;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Implement your Tutorial callbacks here.
/// </summary>
[CreateAssetMenu(fileName = DefaultFileName, menuName = "Tutorials/" + DefaultFileName + " Instance")]
public class TutorialCallbacks : ScriptableObject
{
    public const string DefaultFileName = "TutorialCallbacks";

    public static ScriptableObject CreateInstance()
    {
        return ScriptableObjectUtils.CreateAsset<TutorialCallbacks>(DefaultFileName);
    }

    public static void OpenConsole()
    {
        Debug.Log("Hello from the console :) KIT109 is Awesome!");
        // Get the System.Type of the window
        System.Type windowType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.ConsoleWindow");
        // Show the window
        EditorWindow window = EditorWindow.GetWindow(windowType);
        
    }

    // Example callback for basic UnityEvent
    public void ExampleMethod()
    {
        Debug.Log("ExampleMethod");
    }

    // Example callbacks for ArbitraryCriterion's BoolCallback
    public bool DoesFooExist()
    {
        return GameObject.Find("Foo") != null;
    }

    //step 1-
    public bool SceneCalledMain()
    {
        var sceneName = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name;
        return sceneName.ToLower() == "main";
    }

    //step 17, step 27
    public bool DoesGameObjectWithNameExist(string name)
    {
        return GameObject.Find(name) != null;
    }
    //step 20
    public bool ObjectSelected(string name)
    {
        return Selection.activeGameObject && Selection.activeGameObject.name == name; 
    }

    //step 23
    Vector3 initialPos;
    public void SetBallInitialPos()
    {
        initialPos = GameObject.Find("Ball").transform.position;
    }
    public bool BallMovedDistance()
    {
        return (GameObject.Find("Ball").transform.position - initialPos).magnitude > 1;
    }

    //step 25
    /*Color initialColor;
    bool initialFlip;
    bool initialFlipY;
    public void SetBallInitialSpriteVals()
    {
        var sr = GameObject.Find("Ball").GetComponent<SpriteRenderer>();
        initialColor = sr.color;
        initialFlip = sr.flipX;
        initialFlipY = sr.flipY;
    }*/
    public bool SpriteModified()
    {
        var sr = GameObject.Find("Ball").GetComponent<SpriteRenderer>();
        if (sr == null) return false;
        return sr.color.Equals(Color.white) == false && (sr.flipX || sr.flipY);
        //return initialColor.Equals(sr.color) == false && (initialFlip != sr.flipX || initialFlipY != sr.flipY);
    }

    //step 26
    public bool BallCount(int requiredCount)
    {
        var all = GameObject.FindObjectsByType <GameObject>(FindObjectsSortMode.None);
        int ballCount = 0;
        for (int i=0; i<all.Length; i++) {
            if (all[i].name.Contains("Ball")) {
                ballCount++;
            }
        }
        return ballCount >= requiredCount;
    }
    public bool BallsInDifferentLocations()
    {
        return CommonTutorialCallbacks.ObjectsInDifferentLocations(CommonTutorialCallbacks.GameObjectsContaining("Ball"));
    }

    //step 27
    public bool AllBallsGreen()
    {
        var all = GameObject.FindObjectsByType <GameObject>(FindObjectsSortMode.None);
        //Debug.Log(all.Length);
        for (int i=0; i<all.Length; i++) {
            if (all[i].name.Contains("Ball")) {
                var sr = all[i].GetComponent<SpriteRenderer>();
                //Debug.Log(sr.color);
                if (sr.color.Equals(Color.green) == false) return false;
            }
        }
        return true;
    }
    //step 29
    public bool AtLeastOneRigidbody()
    {
        var all = GameObject.FindObjectsByType <Rigidbody2D>(FindObjectsSortMode.None);
        return all.Length > 0; 
    }

    //step 34
    public bool OneBallAboveTheOther()
    {
        Transform highestBall = null;
        Transform lowestBall = null;

        var all = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        if (all.Length <= 1) return false;

        bool atLeastOneNonMovingBallBelowAMovingBall = false;

        for (int i = 0; i < all.Length; i++)
        {
            var ball = all[i];
            if (ball.name.Contains("Ball"))
            {
                if (ball.GetComponent<Rigidbody2D>())
                {
                    var sr = ball.GetComponent<SpriteRenderer>();
                    if (sr)
                    {
                        for (int j = 0; j < all.Length; j++)
                        {
                            if (i == j) continue;

                            var other = all[j];

                            if (other.GetComponent<Rigidbody2D>() == null)
                            {
                                if (ball.transform.position.y - sr.bounds.size.y > other.transform.position.y)
                                {
                                    if ((ball.transform.position.x - sr.bounds.size.x < other.transform.position.x) && (ball.transform.position.x + sr.bounds.size.x > other.transform.position.x))
                                    {
                                        atLeastOneNonMovingBallBelowAMovingBall = true;
                                    }
                                }
                        }
                        }
                    }

                }

                if (highestBall == null)
                {
                    highestBall = ball.transform;
                }
                if (lowestBall == null)
                {
                    lowestBall = ball.transform;
                }
                if (ball.transform.position.y > highestBall.transform.position.y)
                {
                    highestBall = ball.transform;
                }
                if (ball.transform.position.y < lowestBall.transform.position.y)
                {
                    lowestBall = ball.transform;
                }

            }
        }
        //return lowestBall.GetComponent<Rigidbody2D>() == null;
        return atLeastOneNonMovingBallBelowAMovingBall;
    }

    //step 36
    public bool AllBallsHaveCircleColliders() 
    {
        var all = GameObject.FindObjectsByType <GameObject>(FindObjectsSortMode.None);
        for (int i=0; i<all.Length; i++) {
            if (all[i].name.Contains("Ball")) {
                var c = all[i].GetComponent<CircleCollider2D>();
                if (c == null) return false;
            }
        }
        return true;
    }
    
    //step 37
    public bool ColliderSizeLessThanOriginal()//bleh, hard coded the original size as we only need it for this tute
    {

        var all = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        for (int i = 0; i < all.Length; i++)
        {
            if (all[i].name.Contains("Ball"))
            {
                var c = all[i].GetComponent<CircleCollider2D>();
                if (c == null || c.radius >= 1.28f) return false;
            }
        }
        return true;
    }

    //step 41
    //public bool AtLeastOneWrapAround() 
    //{
    //    var all = GameObject.FindObjectsByType <GameObject>(FindObjectsSortMode.None);
    //    for (int i=0; i<all.Length; i++) {
    //        if (all[i].name.Contains("Ball")) {
    //            var c = all[i].GetComponent("WrapAround");
    //            var rb = all[i].GetComponent<Rigidbody2D>();
    //            if (c != null && rb != null) return true;
    //        }
    //    }
    //    return false;
    //}

    public bool AtLeastOneWrapAround()
    {
        // 1. Get all GameObjects in the scene
        var allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        if (allObjects.Length == 0)
        {
            Criterion.globalLastKnownError = "No GameObjects found.";
            //Debug.Log("From callback: " + TutorialParagraph.lastKnownError);
            return false;
        }    

        // 2. Filter for objects with "Ball" in the name
        var ballObjects = allObjects.Where(obj => obj.name.Contains("Ball"));

        if (ballObjects.Count() == 0)
        {
            Criterion.globalLastKnownError = "No GameObjects found with part of their name as \"Ball\".";
            //Debug.Log("From callback: " + TutorialParagraph.lastKnownError);
            return false;
        }

        // 3. Filter for objects that have BOTH the "WrapAround" component and a Rigidbody2D
        // Note: GetComponent(string) returns a Component, so we check for null
        var objectsWithWrapAround = ballObjects.Where(obj =>
            obj.GetComponent("WrapAround") != null
        );

        if (objectsWithWrapAround.Count() == 0)
        {
            Criterion.globalLastKnownError = "No balls found containing the WrapAround script.";
            //Debug.Log("From callback: " + Criterion.globalLastKnownError);
            return false;
        }

        var objectsWithRigidbody = objectsWithWrapAround.Where(obj =>
            obj.GetComponent<Rigidbody2D>() != null
        );

        if (objectsWithRigidbody.Count() == 0)
        {
            Criterion.globalLastKnownError = "Balls with the WrapAround script need to also have a RigidBody2D component (so they can move).";
            //Debug.Log("From callback: " + TutorialParagraph.lastKnownError);
            return false;
        }

        // 4. Return true if any objects remain in the filtered collection
        return objectsWithRigidbody.Any();
    }


    // Implement the logic to automatically complete the criterion here, if wanted/needed.
    public bool AutoComplete()
    {
        var foo = GameObject.Find("Foo");
        if (!foo)
            foo = new GameObject("Foo");
        return foo != null;
    }

    //Exercises
    public bool SceneCalledInvaderoids()
    {
        string[] guids = AssetDatabase.FindAssets("t:Scene");

        Debug.Log($"Found {guids.Length} scenes in the project:");

        var correctNameFound = false;
        foreach (string guid in guids)
        {
            // 2. Get the full path (e.g., "Assets/Scenes/Menu.unity")
            string path = AssetDatabase.GUIDToAssetPath(guid);

            // 3. Extract just the name (e.g., "Menu")
            string name = Path.GetFileNameWithoutExtension(path);

            var isCorrectName = name.ToLower().TrimEnd().TrimStart() == "invaderoids";

            if (isCorrectName) correctNameFound = true;
        }

        if (!correctNameFound)
        {
            Criterion.globalLastKnownError = "Scene named exactly \"Invaderoids\" not found. Founds scenes called:";
            bool first = true;
            foreach (string guid in guids)
            {
                // 2. Get the full path (e.g., "Assets/Scenes/Menu.unity")
                string path = AssetDatabase.GUIDToAssetPath(guid);

                if (path.StartsWith("Assets/"))
                {
                    // 3. Extract just the name (e.g., "Menu")
                    string name = Path.GetFileNameWithoutExtension(path);

                    if (!first) Criterion.globalLastKnownError += ",";
                    Criterion.globalLastKnownError += " \"" + name + "\"";
                    first = false;
                }
            }
            Criterion.globalLastKnownError += ".<br><br>Check your spelling!";
            return false;
        }

        //var allScenes = UnityEditor.SceneManagement.sceneCount;
        Debug.Log("found: " + correctNameFound);
        var sceneName = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name;
        var isCurrentSceneCorrectName = sceneName.ToLower().TrimStart().TrimEnd() == "invaderoids";

        if (!isCurrentSceneCorrectName)
        {
            Criterion.globalLastKnownError = "\"Invaderoids\" scene found, but it's not the current open scene.";
            return false;
        }

        return true;
    }

    static List<GameObject> list = new List<GameObject>();
    List<GameObject> FiveObjectsStartingWith(string name)
    {
        list.Clear();
        var all = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        for (int i = 0; i < all.Length; i++)
        {
            if (all[i].name.StartsWith(name))
            {
                list.Add(all[i]);
            }
        }

        if (list.Count != 5) return null;
        else return list;
    }

    List<GameObject> FiveObjectsContaining(string name)
    {
        list.Clear();
        var all = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        for (int i = 0; i < all.Length; i++)
        {
            if (all[i].name.Contains(name))
            {
                list.Add(all[i]);
            }
        }

        if (list.Count != 5) return null;
        else return list;
    }

    static List<Color> colorlist = new List<Color>();
    public bool ColouredInvaders()
    {
        var invaders = FiveObjectsContaining("Invader");
        if (invaders == null) return false;

        colorlist.Clear();
        foreach (var invader in invaders)
        {
            var sr = invader.GetComponent<SpriteRenderer>();
            if (sr == null) return false;

            var color = sr.color;
            if (colorlist.Contains(color)) return false;

            colorlist.Add(color);
        }

        return true;
    }

    public bool AllInvadersHaveColliders()
    {
        var invaders = FiveObjectsContaining("Invader");
        if (invaders == null) return false;

        foreach (var invader in invaders)
        {
            if (invader.GetComponent<Collider2D>() == null) return false;
        }

        return true;
    }

    List<Vector3> positionList = new List<Vector3>();
    public bool AllInvadersHaveRandomScripts()
    {
        var invaders = FiveObjectsContaining("Invader");
        if (invaders == null) return false;

        foreach (var invader in invaders)
        {
            if (invader.GetComponent("StartMovingInRandomDirection") == null) return false;
            if (invader.GetComponent("StartRandomRotation") == null) return false;
        }

        return true;
    }
    public bool AllInvadersDifferentPositions()
    {
        var invaders = FiveObjectsContaining("Invader");
        if (invaders == null) return false;

        positionList.Clear();
        foreach (var invader in invaders)
        {
            var position = invader.transform.position;
            if (positionList.Contains(position)) return false;
            positionList.Add(position);
        }

        return true;
    }
    public bool AllInvadersHaveWrapScript()
    {
        var invaders = FiveObjectsContaining("Invader");
        if (invaders == null) return false;

        foreach (var invader in invaders)
        {
            if (invader.GetComponent("WrapAround") == null) return false;
        }

        return true;
    }


    public bool AllBallsDifferentSizes()
    {
        var invaders = FiveObjectsContaining("Ball");
        if (invaders == null) return false;

        positionList.Clear();
        foreach (var invader in invaders)
        {
            var position = invader.transform.localScale;
            if (positionList.Contains(position)) return false;
            positionList.Add(position);
        }

        return true;
    }
    public bool AllBallsDifferentRotations()
    {
        var invaders = FiveObjectsContaining("Ball");
        if (invaders == null) return false;

        positionList.Clear();
        foreach (var invader in invaders)
        {
            var position = invader.transform.localEulerAngles;
            if (positionList.Contains(position)) return false;
            positionList.Add(position);
        }

        return true;
    }

    public bool AllBallsHaveColliders()
    {
        var invaders = FiveObjectsContaining("Ball");
        if (invaders == null) return false;

        foreach (var invader in invaders)
        {
            if (invader.GetComponent<CircleCollider2D>() == null) return false;
        }

        return true;
    }
    public bool AllBallsHaveCollideScript()
    {
        var invaders = FiveObjectsContaining("Ball");
        if (invaders == null) return false;

        foreach (var invader in invaders)
        {
            if (invader.GetComponent("DestroyOnCollision") == null) return false;
        }

        return true;
    }
}

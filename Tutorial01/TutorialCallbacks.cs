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
    // Helper for case-insensitive GameObject finding
    GameObject FindObject(string name)
    {
        var all = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach(var go in all)
        {
            if (go.name.Equals(name, System.StringComparison.OrdinalIgnoreCase))
                return go;
        }
        return null;
    }
    
    // Finds any object containing "Ball" (case-insensitive)
    GameObject FindAnyBall()
    {
        var all = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach(var go in all)
        {
            if (go.name.IndexOf("ball", System.StringComparison.OrdinalIgnoreCase) >= 0)
                return go;
        }
        Criterion.globalLastKnownError = "Could not find any GameObject with 'Ball' in its name.";
        return null;
    }

    // Example callbacks for ArbitraryCriterion's BoolCallback
    public bool DoesFooExist()
    {
        return FindObject("Foo") != null;
    }

    //step 1-
    public bool SceneCalledMain()
    {
        var sceneName = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name;
        if (!sceneName.Equals("Main", System.StringComparison.OrdinalIgnoreCase))
        {
            Criterion.globalLastKnownError = $"The active scene is named \"{sceneName}\", but it should be named \"Main\".";
            return false;
        }
        return true;
    }

    // step 12
    public bool ThreeImagesExistAndOneIsNotBallOrInvader()
    {
        var guids = AssetDatabase.FindAssets("t:Texture");
        var projectImages = new List<string>();
        foreach (var g in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(g);
            // Only count assets in the user's Assets folder, avoiding Packages etc.
            if (path.StartsWith("Assets/", System.StringComparison.OrdinalIgnoreCase))
            {
                projectImages.Add(path);
            }
        }

        if (projectImages.Count < 3)
        {
            Criterion.globalLastKnownError = $"No new images detected in the Assets folder.";
            return false;
        }

        // Check if at least one is NOT named "Ball"
        bool foundNonBallOrInvader = false;
        foreach (var p in projectImages)
        {
            var name = Path.GetFileNameWithoutExtension(p);
            if (!name.Equals("Ball", System.StringComparison.OrdinalIgnoreCase) && !name.Equals("Invader", System.StringComparison.OrdinalIgnoreCase))
            {
                foundNonBallOrInvader = true;
                break;
            }
        }

        if (!foundNonBallOrInvader)
        {
            Criterion.globalLastKnownError = "You have enough images, but all of them are (somehow) named \"Ball\" or \"Invader\". Please rename your imported image.";
            return false;
        }

        return true;
    }

    //step 17, step 27
    //step 17, step 27
    public bool DoesGameObjectWithNameExist(string name)
    {
        if (FindObject(name) == null)
        {
            Criterion.globalLastKnownError = $"Scene is missing a GameObject named \"{name}\".";
            return false;
        }
        return true;
    }
    //step 20
    public bool ObjectSelected(string name)
    {
        if (Selection.activeGameObject == null)
        {
            Criterion.globalLastKnownError = "No GameObject is currently selected.";
            return false;
        }
        if (!Selection.activeGameObject.name.Equals(name, System.StringComparison.OrdinalIgnoreCase))
        {
            Criterion.globalLastKnownError = $"The selected object is \"{Selection.activeGameObject.name}\", but it should be \"{name}\".";
            return false;
        }
        return true;
    }

    //step 23
    Vector3 initialPos;
    public void SetBallInitialPos()
    {
        var ball = FindAnyBall();
        if (ball) initialPos = ball.transform.position;
    }
    public bool BallMovedDistance()
    {
        var ball = FindAnyBall();
        if (ball == null)
        {
             // error set in FindAnyBall
            return false;
        }
        
        var dist = (ball.transform.position - initialPos).magnitude;
        if (dist <= 1)
        {
            Criterion.globalLastKnownError = $"The Ball has moved {dist:F2} units, but needs to move at least 1 unit.";
            //if (dist == 0)
            //{
            //    Criterion.globalLastKnownError = "The ball hasn't been moved.";
            //} 
            //else if (dist <= 1)
            //{
            //    Criterion.globalLastKnownError = "The ball has moved a little bit, but move it move.";
            //}
            return false;
        }
        return true;
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
        var ball = FindAnyBall();
        if (ball == null)
        {
             // error set in FindAnyBall
            return false;
        }

        var sr = ball.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Criterion.globalLastKnownError = "The \"Ball\" object is missing a SpriteRenderer component.";
            return false;
        }

        if (sr.color.Equals(Color.white))
        {
            Criterion.globalLastKnownError = "The Ball's color is still white. Please change it to something else.";
            return false;
        }

        if (!sr.flipX && !sr.flipY)
        {
            Criterion.globalLastKnownError = "The Ball sprite is not flipped. Please flip it on either the X or Y axis.";
            return false;
        }

        return true;
    }

    //step 26
    public bool BallCount(int requiredCount)
    {
        var all = GameObject.FindObjectsByType <GameObject>(FindObjectsSortMode.None);
        int ballCount = 0;
        foreach (var obj in all) {
            if (obj.name.IndexOf("Ball", System.StringComparison.OrdinalIgnoreCase) >= 0) {
                ballCount++;
            }
        }
        
        if (ballCount < requiredCount)
        {
            Criterion.globalLastKnownError = $"Found {ballCount} objects with \"Ball\" in the name, but at least {requiredCount} are required.";
            return false;
        }
        return true;
    }
    public bool BallsInDifferentLocations()
    {
        var balls = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None)
                              .Where(obj => obj.name.IndexOf("Ball", System.StringComparison.OrdinalIgnoreCase) >= 0)
                              .ToArray();

        if (balls.Length < 2)
        {
             // If fewer than 2 balls, trivial success or failure depending on requirement. 
             // Original function didn't check count explicitely but implied multiple.
             // We'll skip error if < 2 as separation implies comparison.
             // But if specific tutorial step requires multiple balls, BallCount should have caught it.
        }

        for(int i=0; i<balls.Length; i++)
        {
             for(int j=i+1; j<balls.Length; j++)
             {
                 if (balls[i].transform.position == balls[j].transform.position)
                 {
                      Criterion.globalLastKnownError = $"Two balls are at the same position {balls[i].transform.position}: \"{balls[i].name}\" and \"{balls[j].name}\". Move them apart."; 
                      return false;
                 }
             }
        }
        return true;
    }

    //step 27
    public bool AllBallsGreen()
    {
        var all = GameObject.FindObjectsByType <GameObject>(FindObjectsSortMode.None);
        var balls = all.Where(x => x.name.IndexOf("Ball", System.StringComparison.OrdinalIgnoreCase) >= 0).ToList();
        
        if (balls.Count == 0)
        {
             // If no balls, strict check would fail, logic depends on if a ball is expected.
             // Original loop would return true if empty list (loop 0 times, return true).
             // We will assume "All existing balls must be green", so 0 balls is effectively true? 
             // Or likely user has balls by now.
             // I'll keep it true if empty to match original logic, but usually step implies existence.
             // But let's check if we should error. Usually there's a BallCount check before.
             return true;
        }

        foreach (var ball in balls) {
            var sr = ball.GetComponent<SpriteRenderer>();
            if (sr == null)
            {
                 Criterion.globalLastKnownError = $"The object \"{ball.name}\" is missing a SpriteRenderer.";
                 return false;
            }
            if (!sr.color.Equals(Color.green))
            {
                 Criterion.globalLastKnownError = $"The object \"{ball.name}\" is not green. Please change its color.";
                 return false;
            }
        }
        return true;
    }
    //step 29
    public bool AtLeastOneRigidbody()
    {
        var all = GameObject.FindObjectsByType <Rigidbody2D>(FindObjectsSortMode.None);
        if (all.Length == 0)
        {
            Criterion.globalLastKnownError = "The Scene needs at least one GameObject with a Rigidbody2D component.";
            return false;
        }
        return true; 
    }

    //step 34
    public bool OneBallAboveTheOther()
    {
        var all = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        var balls = all.Where(x => x.name.IndexOf("Ball", System.StringComparison.OrdinalIgnoreCase) >= 0).ToList();

        if (balls.Count < 2)
        {
            Criterion.globalLastKnownError = "You need at least two balls for this step.";
            return false;
        }

        var movingBalls = balls.Where(b => b.GetComponent<Rigidbody2D>() != null).ToList();
        var staticBalls = balls.Where(b => b.GetComponent<Rigidbody2D>() == null).ToList();

        if (movingBalls.Count == 0)
        {
            Criterion.globalLastKnownError = "At least one Ball must have a Rigidbody2D component (to be the moving ball).";
            return false;
        }

        if (staticBalls.Count == 0)
        {
            Criterion.globalLastKnownError = "At least one Ball must NOT have a Rigidbody2D component (to be the static obstacle).";
            return false;
        }

        foreach (var moving in movingBalls)
        {
            var sr = moving.GetComponent<SpriteRenderer>();
            if (!sr) continue; // Should have been caught by previous steps, or we ignore.

            foreach (var stat in staticBalls)
            {
                // Check if moving is above static
                // Original logic: ball.transform.position.y - sr.bounds.size.y > other.transform.position.y
                // And X alignment check
                
                bool isAbove = (moving.transform.position.y - sr.bounds.size.y) > stat.transform.position.y;
                
                // X alignment: (ball.x - size.x < other.x) && (ball.x + size.x > other.x)
                // This checks if the moving ball's horizontal extent INCLUDES the static ball's center.
                // It simplifies "is strictly above".
                
                bool xAligned = (moving.transform.position.x - sr.bounds.size.x < stat.transform.position.x) && 
                                (moving.transform.position.x + sr.bounds.size.x > stat.transform.position.x);

                if (isAbove && xAligned)
                {
                    return true;
                }
            }
        }

        Criterion.globalLastKnownError = "Position a moving Ball (with Rigidbody2D) above a static Ball (no Rigidbody2D) so that it will fall onto it.";
        return false;
    }

    //step 36
    public bool AllBallsHaveCircleColliders() 
    {
        var all = GameObject.FindObjectsByType <GameObject>(FindObjectsSortMode.None);
        var balls = all.Where(x => x.name.IndexOf("Ball", System.StringComparison.OrdinalIgnoreCase) >= 0);
        
        foreach (var ball in balls)
        {
             if (ball.GetComponent<CircleCollider2D>() == null)
             {
                  Criterion.globalLastKnownError = $"The object \"{ball.name}\" needs a CircleCollider2D component.";
                  return false;
             }
        }
        return true;
    }
    
    //step 37
    public bool ColliderSizeLessThanOriginal()//bleh, hard coded the original size as we only need it for this tute
    {
        var all = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        var balls = all.Where(x => x.name.IndexOf("Ball", System.StringComparison.OrdinalIgnoreCase) >= 0);

        foreach (var ball in balls)
        {
             var c = ball.GetComponent<CircleCollider2D>();
             if (c == null)
             {
                  Criterion.globalLastKnownError = $"The object \"{ball.name}\" needs a CircleCollider2D component.";
                  return false;
             }
             if (c.radius >= 1.28f)
             {
                  Criterion.globalLastKnownError = $"The CircleCollider2D on \"{ball.name}\" is too large (radius: {c.radius:F2}). Please reduce the size by editing the Collider in the Scene view.";
                  return false;
             }
        }
        return true;
    }

    //step 41
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
        var ballObjects = allObjects.Where(obj => obj.name.IndexOf("Ball", System.StringComparison.OrdinalIgnoreCase) >= 0);

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
            Criterion.globalLastKnownError = "Balls with the WrapAround script need to also have a RigidBody2D component (so they can move). Maybe you've added the script to the wrong ball?";
            //Debug.Log("From callback: " + TutorialParagraph.lastKnownError);
            return false;
        }

        // 4. Return true if any objects remain in the filtered collection
        return objectsWithRigidbody.Any();
    }


    // Implement the logic to automatically complete the criterion here, if wanted/needed.
    public bool AutoComplete()
    {
        var foo = FindObject("Foo");
        if (!foo)
            foo = new GameObject("Foo");
        return foo != null;
    }

    //Exercises
    public bool SceneCalledInvaderoids()
    {
        string[] guids = AssetDatabase.FindAssets("t:Scene");

        //Debug.Log($"Found {guids.Length} scenes in the project:");

        var correctNameFound = false;
        foreach (string guid in guids)
        {
            // 2. Get the full path (e.g., "Assets/Scenes/Menu.unity")
            string path = AssetDatabase.GUIDToAssetPath(guid);

            // 3. Extract just the name (e.g., "Menu")
            string name = Path.GetFileNameWithoutExtension(path);

            var isCorrectName = name.Trim().Equals("Invaderoids", System.StringComparison.OrdinalIgnoreCase);

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
        //Debug.Log("found: " + correctNameFound);
        var sceneName = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name;
        var isCurrentSceneCorrectName = sceneName.Trim().Equals("Invaderoids", System.StringComparison.OrdinalIgnoreCase);

        if (!isCurrentSceneCorrectName)
        {
            Criterion.globalLastKnownError = "\"Invaderoids\" scene found, but it's not the current open scene.";
            return false;
        }

        return true;
    }

    List<GameObject> FiveObjectsStartingWith(string name)
    {
        var list = new List<GameObject>();
        var all = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        for (int i = 0; i < all.Length; i++)
        {
            if (all[i].name.StartsWith(name, System.StringComparison.OrdinalIgnoreCase))
            {
                list.Add(all[i]);
            }
        }

        if (list.Count != 5)
        {
             Criterion.globalLastKnownError = $"Found {list.Count} objects starting with \"{name}\", but exactly 5 are required.";
             // We return list anyway so caller can inspect what we found, or return null if we want to enforce strictness.
             // Original returned null.
             return null;
        }
        else return list;
    }

    List<GameObject> FiveObjectsContaining(string name)
    {
        var list = new List<GameObject>();
        var all = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        for (int i = 0; i < all.Length; i++)
        {
            if (all[i].name.IndexOf(name, System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                list.Add(all[i]);
            }
        }

        if (list.Count != 5)
        {
            Criterion.globalLastKnownError = $"Found {list.Count} objects containing \"{name}\", but exactly 5 are required.";
            return null;
        }
        else return list;
    }

    public bool ColouredInvaders()
    {
        var invaders = FiveObjectsContaining("Invader");
        if (invaders == null) return false;

        var colorlist = new List<Color>();
        foreach (var invader in invaders)
        {
            var sr = invader.GetComponent<SpriteRenderer>();
            if (sr == null)
            {
                Criterion.globalLastKnownError = $"The object <go>{invader.name}</go> is missing a SpriteRenderer.";
                return false;
            }

            var color = sr.color;
            if (colorlist.Contains(color))
            {
                Criterion.globalLastKnownError = $"The object <go>{invader.name}</go> has the same color {color} as another Invader. Each Invader must have a unique color.";
                return false;
            }

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
            if (invader.GetComponent<Collider2D>() == null)
            {
                Criterion.globalLastKnownError = $"The object <go>{invader.name}</go> needs a Collider2D component (e.g. PolygonCollider2D).";
                return false;
            }
        }

        return true;
    }

    public bool AllInvadersHaveRandomScripts()
    {
        var invaders = FiveObjectsContaining("Invader");
        if (invaders == null) return false;

        foreach (var invader in invaders)
        {
            if (invader.GetComponent("StartMovingInRandomDirection") == null)
            {
                Criterion.globalLastKnownError = $"The object <go>{invader.name}</go> is missing the \"StartMovingInRandomDirection\" script.";
                return false;
            }
            if (invader.GetComponent("StartRandomRotation") == null)
            {
                Criterion.globalLastKnownError = $"The object <go>{invader.name}</go> is missing the \"StartRandomRotation\" script.";
                return false;
            }
        }

        return true;
    }
    public bool AllInvadersDifferentPositions()
    {
        var invaders = FiveObjectsContaining("Invader");
        if (invaders == null) return false;

        var positionList = new List<Vector3>();
        foreach (var invader in invaders)
        {
            var position = invader.transform.position;
            if (positionList.Contains(position))
            {
                Criterion.globalLastKnownError = $"The object <go>{invader.name}</go> is at the same position {position} as another Invader. Spread them out.";
                return false;
            }
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
            if (invader.GetComponent("WrapAround") == null)
            {
                Criterion.globalLastKnownError = $"The object <go>{invader.name}</go> is missing the script that would make it wrap around the screen.";
                return false;
            }
        }

        return true;
    }


    public bool AllBallsDifferentSizes()
    {
        var balls = FiveObjectsContaining("Ball");
        if (balls == null) return false;

        var sizeList = new List<Vector3>();
        foreach (var ball in balls)
        {
            var size = ball.transform.localScale;
            if (sizeList.Contains(size))
            {
                Criterion.globalLastKnownError = $"The object <go>{ball.name}</go> has the same size ({size}) as another Ball. Please change the scale of each ball.";
                return false;
            }
            sizeList.Add(size);
        }

        return true;
    }
    public bool AllBallsDifferentRotations()
    {
        var balls = FiveObjectsContaining("Ball");
        if (balls == null) return false;

        var rotList = new List<Vector3>();
        foreach (var ball in balls)
        {
            var rot = ball.transform.localEulerAngles;
            if (rotList.Contains(rot))
            {
                Criterion.globalLastKnownError = $"The object <go>{ball.name}</go> has the same rotation ({rot}) as another Ball. Please rotate them differently.";
                return false;
            }
            rotList.Add(rot);
        }
        foreach (var ball in balls)
        {
            var rot = ball.transform.localEulerAngles;
            if (rot.x != 0 || rot.y != 0)
            {
                Criterion.globalLastKnownError = $"We are working in 2D, but <go>{ball.name}</go> has rotations on more than just the Z-axis: {rot}. Please set the X- and Y-axis rotations to zero.";
                return false;
            }
        }

        return true;
    }

    public bool AllBallsHaveColliders()
    {
        var balls = FiveObjectsContaining("Ball");
        if (balls == null) return false;

        foreach (var ball in balls)
        {
            if (ball.GetComponent<CircleCollider2D>() == null)
            {
                Criterion.globalLastKnownError = $"The object <go>{ball.name}</go> is missing a <asset>CircleCollider2D</asset> component.";
                return false;
            }
        }

        return true;
    }
    public bool AllBallsHaveCollideScript()
    {
        var balls = FiveObjectsContaining("Ball");
        if (balls == null) return false;

        foreach (var ball in balls)
        {
            if (ball.GetComponent("DestroyOnCollision") == null)
            {
                Criterion.globalLastKnownError = $"The object <go>{ball.name}</go> is missing the script that would make it disappear if anything collides with it.";
                return false;
            }
        }

        return true;
    }

}

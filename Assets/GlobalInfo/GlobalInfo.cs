using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Modes of interaction:
// 1. Instancing and manipulating cubes
// 2. Interacting with force
// 3. Instancing prefabs (e.g., imported objects)
// 4. Building environment (e.g., walls)

public class GlobalInfo : MonoBehaviour
{
    private static GlobalInfo _global_info;

    public static GlobalInfo global_info
    {
        get 
        {
            return _global_info;
        }
    }

    public Dictionary<string, string> interaction_mode_types = new Dictionary<string, string>();
    public List<string> existing_interaction_modes = new List<string>();
    public string current_interaction_mode;

    private void Awake()
    {
        if (_global_info != null && _global_info != this)
        {
            Destroy(this.gameObject);
        } 
        else 
        {
            _global_info = this;
        }

        interaction_mode_types.Add("CUBE_INTERACTION", "Spawning and managing cubes");
        interaction_mode_types.Add("FORCE_INTERACTION", "Applying force on objects");
        interaction_mode_types.Add("PREFAB_INSTANCING", "Instancing objects");
        interaction_mode_types.Add("BUILD_ENV", "Building walls, floors, etc.");
        foreach( KeyValuePair<string, string> kvp in interaction_mode_types )
        {
            existing_interaction_modes.Add(kvp.Value);
        }
        current_interaction_mode = existing_interaction_modes[0];
    }


    void OnGUI()
    {
        GUI.Box(new Rect(600, 50, 300, 20), "GAME MODE (1,2,3,4): "+current_interaction_mode);
    }

    void Update()
    {
        if (Input.GetKeyDown("1"))
        {
            current_interaction_mode = existing_interaction_modes[0];
        }

        if (Input.GetKeyDown("2"))
        {
            current_interaction_mode = existing_interaction_modes[1];
        }

        if (Input.GetKeyDown("3"))
        {
            current_interaction_mode = existing_interaction_modes[2];
        }

        if (Input.GetKeyDown("4"))
        {
            current_interaction_mode = existing_interaction_modes[3];
        }
    }

}

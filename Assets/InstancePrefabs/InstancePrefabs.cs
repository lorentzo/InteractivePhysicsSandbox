using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstancePrefabs : MonoBehaviour
{
    public GameObject[] prefabs;
    public GameObject[] prefab_previews;
    bool inst_preview_created;
    GameObject inst_preview;
    float inst_dist;
    float inst_scale;

    // Start is called before the first frame update
    void Start()
    {
        inst_dist = 30.0f;
        inst_scale = 1.0f;
        inst_preview_created = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (GlobalInfo.global_info.current_interaction_mode == GlobalInfo.global_info.interaction_mode_types["PREFAB_INSTANCING"])
        {
            instance_from_space();
        }
    }

    void instance_from_space()
    {
        if (Input.GetMouseButton(0))
        {
            if(Input.mouseScrollDelta.y < 0.0f)
            {
                inst_dist -= 1.0f;
            }

            if(Input.mouseScrollDelta.y > 0.0f)
            {
                inst_dist += 1.0f;
            }

            if(Input.GetKeyDown("q"))
            {
                inst_scale += 1.0f;
            }
            if(Input.GetKeyDown("e"))
            {
                inst_scale -= 1.0f;
                if (inst_scale < 1.0f)
                {
                    inst_scale = 1.0f;
                }
            }

            int screen_x = Screen.width / 2;
            int screen_y = Screen.height / 2;
            Vector2 screen_center = new Vector2(screen_x, screen_y);
            Ray ray = Camera.main.ScreenPointToRay(screen_center);
            Vector3 point_in_space = ray.origin + ray.direction * inst_dist;

            if(!inst_preview_created)
            {
                inst_preview = Instantiate(prefab_previews[0], point_in_space, Quaternion.identity);
                inst_preview_created = true;
            }
            else
            {
                // Update transform.
                inst_preview.transform.position = point_in_space;
                inst_preview.transform.localScale = new Vector3(inst_scale,inst_scale,inst_scale);
            }
        }

        if(Input.GetMouseButtonUp(0))
        {
            GameObject inst = Instantiate(prefabs[0], inst_preview.transform.position, inst_preview.transform.rotation);
            inst.transform.localScale = new Vector3(inst_scale,inst_scale,inst_scale);
            Destroy(inst_preview);
            inst_preview_created = false;
        }
    }
}

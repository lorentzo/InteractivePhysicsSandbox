using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildWalls : MonoBehaviour
{
    public GameObject wall_preview;
    public GameObject wall_final;
    private GameObject wall_preview_inst;
    private bool preview_created;
    private float rotate_angle;
    private float scale_factor;
    private Vector3 scale_vector;

    // Start is called before the first frame update
    void Start()
    {
        preview_created = false;
        rotate_angle = 90.0f;
        scale_factor = 3.0f;
        scale_vector = new Vector3(scale_factor, scale_factor, scale_factor);
    }

    // Update is called once per frame
    void Update()
    {
        if (GlobalInfo.global_info.current_interaction_mode == GlobalInfo.global_info.interaction_mode_types["BUILD_ENV"])
        {
            if (Input.GetMouseButton(0))
            {
                // Create preview.
                int screen_x = Screen.width / 2;
                int screen_y = Screen.height / 2;
                Vector2 screen_center = new Vector2(screen_x, screen_y);
                Ray ray = Camera.main.ScreenPointToRay(screen_center);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100)) // objects to be instanced must have "Ignore Raycast" layer!
                {
                    if(!preview_created)
                    {
                        wall_preview_inst = Instantiate(wall_preview, hit.point, Quaternion.identity);
                        preview_created = true;
                    }
                    else
                    {
                        wall_preview_inst.transform.position = hit.point;
                    }
                }

                // Rotate preview.
                if (Input.GetMouseButtonDown(1))
                {
                    wall_preview_inst.transform.RotateAround(wall_preview_inst.transform.position, Vector3.up, rotate_angle*Time.deltaTime);
                }
            }
            if(Input.GetMouseButtonUp(0))
            {
                Instantiate(wall_final, wall_preview_inst.transform.position, wall_preview_inst.transform.rotation);
                Destroy(wall_preview_inst);
                preview_created = false;
            }
        }
    }
}

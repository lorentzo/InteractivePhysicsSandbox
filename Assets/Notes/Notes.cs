using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Physics sandbox: rigid bodies

// git setup: https://kbroman.org/github_tutorial/pages/init.html

// Inspiration:
// + https://learn.unity.com/search/?k=%5B%22lang%3Aen%22%2C%22tag%3A5813f57532b30600250d6e0d%22%2C%22q%3A%22%2C%22tag%3A5807188409091500644028e8%22%5D
// + https://learn.unity.com/tutorial/creating-a-physics-sandbox-workshop-video?uv=2019.3&projectId=5efc99d1edbc2a0020112688#

// Main interaction modes:
// 1. Instancing and manipulating cubes
//   + VFX + GUI for guiding the eye
//   + materials for cubes
//   + selective cubes grow/shrink/destroy
//     + TODO: layers are needed for correct hit: https://docs.unity3d.com/ScriptReference/LayerMask.html
// 2. Instancing prefabs from Blender
//   + rotate inst preview
//   + add range of prefabs to choose from
//   + add vertical line to the ground guiding the eye where the prefab will be created
// 3. Building environment
//   + VFX + GUI for guiding the eye
//   + preview transformation
//   + materials (tilable textures)
// 4. Interacting with force
//   + VFX: push/pull effect of moving to hit point and exploding/imploding
//   + VFX: laser https://docs.unity3d.com/Manual/class-LineRenderer.html
//   + VFX + GUI for guiding the eye
//   + global gravity dir and strength
//   + global forces?
//   + time stop

// Environment:
// Blurred HDRI image as distant environment
// Table or some other geometry where our sandbox is placed
// Sandbox which is interactive and can be built
// + tilable textures for walls
// + static objects for aesthetics
// + Post processing: https://docs.unity3d.com/Manual/PostProcessingOverview.html

// Player:
// + add crosshair
// + portals
// + depth of field





public class Notes : MonoBehaviour
{
}

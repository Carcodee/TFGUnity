using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerVFXController : NetworkBehaviour
{
    public StateMachineController stateMachineController;
    public PlayerStatsController playerStatsController;
    [Header("MeshTrail")]
    public float meshTrailTime= 0.5f;
    public float meshTrailTick= 0.8f;
    public float destroyTime= 0.3f;
    public bool meshTrailActive = false;
    public SkinnedMeshRenderer[] skinnedMeshRenderers;
    public Material mat;
    public string shaderVariableName;
    public float shaderVariableRate=0.1f;
    public float shaderVariableRefreshRate=0.05f;

    [Header("LevelUpGlow")]
    public Material levelUpMat;
    public float levelUpGlowTime= 0.05f;
    public float shaderVariableGlowRate = 0.1f;
    public float glowGoalValue= -7.0f;
    public string shaderVariableNameGlow;

    
    
    void Start()
    {

    }
    private void OnEnable()
    {
        if (IsOwner)
        {
            stateMachineController = GetComponent<StateMachineController>();
            playerStatsController = GetComponent<PlayerStatsController>();
            playerStatsController.OnLevelUp += AnimateGlowMaterial;
        }


    }
    private void OnDisable()
    {
        if (IsOwner)
        {
            playerStatsController.OnLevelUp -= AnimateGlowMaterial;
        }
    }
    void Update()
    {
   
    }
    private void FixedUpdate()
    {
        if (IsOwner)
        {
            if (stateMachineController.currentState.stateName == "Jetpack")
            {
                StartCoroutine(ActiveTrail(meshTrailTime));
            }
        }
        
    }

    public void AnimateGlowMaterial()
    {
        levelUpMat.GetFloat("_FresnelIntensity");
        StartCoroutine(AnimateMaterial(levelUpMat, glowGoalValue, shaderVariableGlowRate, levelUpGlowTime, shaderVariableNameGlow));
    }
    public IEnumerator ActiveTrail(float timeActive)
    {

            timeActive -= meshTrailTick;

            if (skinnedMeshRenderers == null)
            {
                skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            }
            for (int i = 0; i < skinnedMeshRenderers.Length; i++)
            {
                Debug.Log("ActiveTrail");
                GameObject meshObj = new GameObject();
                meshObj.transform.SetPositionAndRotation(transform.position, transform.rotation);
                MeshRenderer mr= meshObj.AddComponent<MeshRenderer>();
                MeshFilter mf=meshObj.AddComponent<MeshFilter>();
                Mesh mesh= new Mesh();
                skinnedMeshRenderers[i].BakeMesh(mesh);
                mf.mesh=mesh;
                mr.material=mat;
                StartCoroutine(AnimateMaterial(mr.material, 0, shaderVariableRate, shaderVariableRefreshRate, shaderVariableName));
                Destroy(meshObj, destroyTime);

            }

            yield return new WaitForSeconds(meshTrailTick);
           
        
        meshTrailActive=false;
    }
    IEnumerator AnimateMaterial(Material mat, float goal, float rate, float refreshRate, string variableName)
    {
        float valueToAnim = mat.GetFloat(variableName);
        while (valueToAnim>goal)
        {
            valueToAnim -= rate;
            mat.SetFloat(shaderVariableName, valueToAnim);
            yield return new WaitForSeconds(refreshRate);
        }
    }
   
}

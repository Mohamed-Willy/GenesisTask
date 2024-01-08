using System.Collections.Generic;
using UnityEngine;

# if UNITY_EDITOR
using UnityEditor;
#endif

public class SequancingSystem : MonoBehaviour
{
    // Choice
    [HideInInspector] public bool useAudio;
    // Transitions
    [HideInInspector] public bool audioWait;
    [HideInInspector] public List<float> audioTime;
    [HideInInspector] public List<int> audioIndex;
    // Sources
    [HideInInspector] public AudioSource audioSource;
    [HideInInspector] public List<AudioClip> audioClips;


    // Choice
    [HideInInspector] public bool useAnimation;
    // Transitions
    [HideInInspector] public bool animWait;
    [HideInInspector] public List<float> animTime;
    [HideInInspector] public List<int> animIndex;
    // Sources
    [HideInInspector] public Animator animator;
    [HideInInspector] public List<string> parameterNames;


    private float timer;

#if UNITY_EDITOR
    [CustomEditor(typeof(SequancingSystem))]
    public class SequancingSystemEditor : Editor
    {
        SerializedProperty useAudio;
        
        SerializedProperty audioWait;
        SerializedProperty audioTime;
        SerializedProperty audioIndex;
        
        SerializedProperty audioSource;
        SerializedProperty audioClips;
        
        SerializedProperty useAnimation;

        SerializedProperty animWait;
        SerializedProperty animTime;
        SerializedProperty animIndex;

        SerializedProperty animator;
        SerializedProperty parameters;
        private void OnEnable()
        {
            useAudio = serializedObject.FindProperty(nameof(SequancingSystem.useAudio));

            audioWait = serializedObject.FindProperty(nameof(SequancingSystem.audioWait));
            audioTime = serializedObject.FindProperty(nameof(SequancingSystem.audioTime));
            audioIndex = serializedObject.FindProperty(nameof(SequancingSystem.audioIndex));
            
            audioSource = serializedObject.FindProperty(nameof(SequancingSystem.audioSource));
            audioClips = serializedObject.FindProperty(nameof(SequancingSystem.audioClips));
            
            useAnimation = serializedObject.FindProperty(nameof(SequancingSystem.useAnimation));
            
            animWait = serializedObject.FindProperty(nameof(SequancingSystem.animWait));
            animTime = serializedObject.FindProperty(nameof(SequancingSystem.animTime));
            animIndex = serializedObject.FindProperty(nameof(SequancingSystem.animIndex));

            animator = serializedObject.FindProperty(nameof(SequancingSystem.animator));
            parameters = serializedObject.FindProperty(nameof(SequancingSystem.parameterNames));
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            SequancingSystem sequancingSystem = (SequancingSystem)target;

            EditorGUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(useAudio);
            EditorGUILayout.PropertyField(useAnimation);
            GUILayout.EndHorizontal();

            if (sequancingSystem.useAudio)
            {
                EditorGUILayout.Space(20);
                EditorGUILayout.PropertyField(audioWait);
                EditorGUILayout.Space(10);
                EditorGUILayout.PropertyField(audioSource);
                EditorGUILayout.Space(10);
                EditorGUILayout.PropertyField(audioClips);
                EditorGUILayout.Space(10);
                if(sequancingSystem.audioWait == false)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(audioIndex, GUILayout.Width(180));
                    EditorGUILayout.Space(10);
                    EditorGUILayout.PropertyField(audioTime, GUILayout.Width(180));
                    GUILayout.EndHorizontal();
                }
                else
                {
                    EditorGUILayout.PropertyField(audioIndex);
                }
                EditorGUILayout.Space(10);
            }

            if (sequancingSystem.useAnimation)
            {
                EditorGUILayout.Space(20);
                EditorGUILayout.PropertyField(animWait);
                EditorGUILayout.Space(10);
                EditorGUILayout.PropertyField(animator);
                EditorGUILayout.Space(10);
                EditorGUILayout.PropertyField(parameters);
                EditorGUILayout.Space(10);
                if (sequancingSystem.animWait == false)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(animIndex, GUILayout.Width(180));
                    EditorGUILayout.Space(10);
                    EditorGUILayout.PropertyField(animTime, GUILayout.Width(180));
                    GUILayout.EndHorizontal();
                }
                else
                {
                    EditorGUILayout.PropertyField(animIndex);
                }
                EditorGUILayout.Space(10);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif

    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (useAudio)
            audioManger();

        if (useAnimation)
            animationManger();

    }

    private void audioManger()
    {
        if (audioWait == false)
        {
            for (int cur = 0; cur < audioTime.Count; cur++)
            {
                if (timer >= audioTime[cur])
                {
                    audioSource.clip = audioClips[audioIndex[cur]];
                    audioSource.Play();
                    audioTime.RemoveAt(cur);
                    audioIndex.RemoveAt(cur);
                }
            }
        }
        else
        {
            if(audioSource.isPlaying == false && audioIndex.Count > 0)
            {
                audioSource.clip = audioClips[audioIndex[0]];
                audioSource.Play();
                audioIndex.RemoveAt(0);
            }
        }
    }

    private void animationManger()
    {
        if (animWait == false)
        {
            for (int cur = 0; cur < animTime.Count; cur++)
            {
                if (timer >= animTime[cur])
                {
                    for(int i = 0; i < parameterNames.Count; i++)
                    {
                        animator.SetBool(parameterNames[i], false);
                    }
                    if (animIndex[cur] != -1)
                        animator.SetBool(parameterNames[animIndex[cur]], true);
                    animTime.RemoveAt(cur);
                    animIndex.RemoveAt(cur);
                }
            }
        }
        else
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f && animIndex.Count > 0)
            {
                for (int i = 0; i < parameterNames.Count; i++)
                {
                    animator.SetBool(parameterNames[i], false);
                }
                if (animIndex[0] != -1)
                    animator.SetBool(parameterNames[animIndex[0]], true);
                animIndex.RemoveAt(0);
                animator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0f);
            }
        }
    }
}

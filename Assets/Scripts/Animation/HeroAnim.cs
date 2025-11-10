using UnityEngine;

public class HeroAnim : MonoBehaviour
{
    // Hit �ִϸ��̼��� ���� �ð�
    [Header("Animation Hit Times")]
    [field: SerializeField] public float[] SkillTimes { get; private set; }
    [field: SerializeField] public float CriticalTime { get; private set; }

    [Header("Animator")]
    [field: SerializeField] public Animator Anim { get; private set; }
}

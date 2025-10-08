using UnityEngine;
using Cysharp.Threading.Tasks;

public static class EffectUtil
{
    #region Public Method
    public static async UniTask StartShake(float shakeAmount, float shakeTime)
    {
        if (shakeAmount == 0)
            return;

        if (shakeTime == 0)
            return;

        await Shake(shakeAmount, shakeTime);
    }
    #endregion

    #region Private Method
    private static async UniTask Shake(float shakeAmount, float shakeTime)
    {
        if (Camera.main == null)
            return;

        Transform camTrans = Camera.main.transform;
        Vector3 originalPos = camTrans.position;
        float timer = 0f;

        // 흔들리는 동안 매 프레임마다 position 갱신
        while (timer < shakeTime)
        {
            Vector3 randomOffset = Random.insideUnitSphere * shakeAmount;

            camTrans.position = originalPos + randomOffset;
            timer += Time.deltaTime;

            await UniTask.Yield();
        }

        camTrans.position = originalPos;
    }
    #endregion
}

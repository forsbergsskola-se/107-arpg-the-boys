using System.Collections;
using UnityEngine;
using static DrawBoxCast;

public class GroundScatterScript : MonoBehaviour
{
    public GameObject[] groundScatterPoints;
    public Vector3 hitBoxSize;
    private float _delayBetween = 0.175f;
    private float _timer;
    public LayerMask hitLayer;
    public float groundScatterDamage;

    private void Start()
    {
        StartCoroutine(CO_GroundScatterHitBox());
    }

    private IEnumerator CO_GroundScatterHitBox()
    {
        for (var i = 0; i < groundScatterPoints.Length; i++)
        {
            DrawBoxCastBox(groundScatterPoints[i].transform.position, hitBoxSize, groundScatterPoints[i].transform.rotation, Color.magenta);
            Collider[] hits = Physics.OverlapBox(groundScatterPoints[i].transform.position, hitBoxSize,
                groundScatterPoints[i].transform.rotation, hitLayer);
            for (var i1 = 0; i1 < hits.Length; i1++)
            {
                if(hits[i1].TryGetComponent(out IDamageable damageable))
                    damageable.TakeDamage(groundScatterDamage);
            }
            while (_timer < _delayBetween)
            {
                _timer += Time.deltaTime;
                yield return null;
            }
            _timer = 0;
        }
    }
    //Add lingering damage with takedamage * Time.DeltaTime;
}

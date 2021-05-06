using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BzKovSoft.ObjectSlicer.EventHandlers
{
	/// <summary>
	/// Fixes weight and center of the mass of sliced objects. It works correct only for closed objects.
	/// </summary>
	[DisallowMultipleComponent]
	class BzSmoothDepenetration : MonoBehaviour, IBzObjectSlicedEvent
	{
        public void ObjectSliced(GameObject original, GameObject resultNeg, GameObject resultPos)
        {
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(SmoothDepenetration(resultNeg));
                StartCoroutine(SmoothDepenetration(resultPos));
            } else
            {
                SmoothDepenetrationNew(resultNeg);
                SmoothDepenetrationNew(resultPos);

            }
		}

		public static IEnumerator SmoothDepenetration(GameObject go)
		{
			var rigids = go.GetComponentsInChildren<Rigidbody>();
			var maxVelocitys = new float[rigids.Length];
			for (int i = 0; i < rigids.Length; i++)
			{
				var rigid = rigids[i];
				maxVelocitys[i] = rigid.maxDepenetrationVelocity;
				rigid.maxDepenetrationVelocity = 0.1f;
			}

			yield return new WaitForSeconds(1);

			for (int i = 0; i < rigids.Length; i++)
			{
				var rigid = rigids[i];
				if (rigid == null)
					continue;

				float maxVel = maxVelocitys[i];
				rigid.maxDepenetrationVelocity = maxVel;
			}
		}


        public static void SmoothDepenetrationNew(GameObject go)
        {
            var rigids = go.GetComponentsInChildren<Rigidbody>();
            var maxVelocitys = new float[rigids.Length];
            for (int i = 0; i < rigids.Length; i++)
            {
                var rigid = rigids[i];
                maxVelocitys[i] = rigid.maxDepenetrationVelocity;
                rigid.maxDepenetrationVelocity = 0.1f;
            }
            
          //  yield return new WaitForSeconds(1);

            for (int i = 0; i < rigids.Length; i++)
            {
                var rigid = rigids[i];
                if (rigid == null)
                    continue;

                float maxVel = maxVelocitys[i];
                rigid.maxDepenetrationVelocity = maxVel;
            }
        }
    }
}
